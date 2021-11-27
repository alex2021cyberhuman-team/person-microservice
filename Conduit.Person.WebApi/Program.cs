using Conduit.Shared.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

#region ServicesConfiguration

var services = builder.Services;
var environment = builder.Environment;
var configuration = builder.Configuration;

services.AddControllers();
services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc(
            "v1",
            new() { Title = "Conduit.Person.WebApi", Version = "v1" });
    });

services
    .AddJwtServices(configuration.GetSection("Jwt").Bind)
    .AddW3CLogging(configuration.GetSection("W3C").Bind)
    .AddHttpClient()
    .AddHttpContextAccessor();

#endregion

var app = builder.Build();

#region AppConfiguration

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Conduit.Person.WebApi v1"));
    IdentityModelEventSource.ShowPII = true;
}

app.UseW3CLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
