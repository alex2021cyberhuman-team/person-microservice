using System.Globalization;
using Conduit.Person.BusinessLogic;
using Conduit.Person.DataAccessLayer;
using Conduit.Person.WebApi;
using Conduit.Person.WebApi.Consumers;
using Conduit.Shared.Events.Models.Profiles.CreateFollowing;
using Conduit.Shared.Events.Models.Profiles.RemoveFollowing;
using Conduit.Shared.Events.Models.Users.Register;
using Conduit.Shared.Events.Models.Users.Update;
using Conduit.Shared.Events.Services.RabbitMQ;
using Conduit.Shared.Localization;
using Conduit.Shared.Startup;
using Conduit.Shared.Tokens;
using Conduit.Shared.Validations;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

#region ServicesConfiguration

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration;

var supportedCultures = new CultureInfo[] { new("ru"), new("en") };
services.AddControllers().Localize<SharedResource>(supportedCultures);
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new() { Title = "Conduit.Person.WebApi", Version = "v1" });
});

services.AddJwtServices(configuration.GetSection("Jwt").Bind)
    .DisableDefaultModelValidation()
    .AddW3CLogging(configuration.GetSection("W3C").Bind).AddHttpClient()
    .AddHttpContextAccessor()
    .RegisterRabbitMqWithHealthCheck(configuration.GetSection("RabbitMQ").Bind)
    .RegisterConsumer<UpdateUserEventModel,
        UpdateUserEventConsumer>(ConfigureConsumer)
    .RegisterConsumer<RegisterUserEventModel,
        RegisterUserEventConsumer>(ConfigureConsumer)
    .RegisterProducer<CreateFollowingEventModel>()
    .RegisterProducer<RemoveFollowingEventModel>()
    .RegisterNeo4JWithHealthCheck(configuration.GetSection("Neo4J").Bind)
    .AddScoped<IProfileViewer, ProfileViewer>()
    .AddScoped<IFollowingsManager, FollowingsManager>();

#endregion

var app = builder.Build();

#region AppConfiguration

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    IdentityModelEventSource.ShowPII = true;
}

app.UseRouting();
app.UseCors(options =>
    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseW3CLogging();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var initializationScope = app.Services.CreateScope();

await initializationScope.WaitHealthyServicesAsync(TimeSpan.FromHours(1));
await initializationScope.InitializeQueuesAsync();
await initializationScope.InitializeNeo4JAsync();

#endregion

app.Run();

static void ConfigureConsumer<T>(
    RabbitMqSettings<T> settings)
{
    settings.Consumer = "person";
}
