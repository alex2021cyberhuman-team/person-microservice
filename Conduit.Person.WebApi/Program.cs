using System.Text.Json;
using Conduit.Person.BusinessLogic;
using Conduit.Person.DataAccessLayer;
using Conduit.Person.WebApi.Consumers;
using Conduit.Shared.Events.Models.Users.Register;
using Conduit.Shared.Events.Models.Users.Update;
using Conduit.Shared.Events.Services.RabbitMQ;
using Conduit.Shared.Startup;
using Conduit.Shared.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;

var x = new Neo4JOptions();
var j = JsonSerializer.Serialize(x);

var builder = WebApplication.CreateBuilder(args);

#region ServicesConfiguration

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration;

services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new() { Title = "Conduit.Person.WebApi", Version = "v1" });
});

services.AddJwtServices(configuration.GetSection("Jwt").Bind)
    .AddW3CLogging(configuration.GetSection("W3C").Bind).AddHttpClient()
    .AddHttpContextAccessor()
    .RegisterRabbitMqWithHealthCheck(configuration.GetSection("RabbitMQ").Bind)
    .RegisterConsumer<UpdateUserEventModel, UpdateUserEventConsumer>()
    .RegisterConsumer<RegisterUserEventModel, RegisterUserEventConsumer>()
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
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
            "Conduit.Person.WebApi v1"));
    IdentityModelEventSource.ShowPII = true;
}

app.UseW3CLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var initializationScope = app.Services.CreateScope();

await initializationScope.WaitHealthyServicesAsync(TimeSpan.FromHours(1));
await initializationScope.InitializeQueuesAsync();
await initializationScope.InitializeNeo4JAsync();

#endregion

app.Run();
