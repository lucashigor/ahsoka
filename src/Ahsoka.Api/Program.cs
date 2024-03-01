using Ahsoka.Api.Common.Middlewares;
using Ahsoka.Api.Common.Swagger;
using Ahsoka.Application.Common;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Kernel.Extensions;
using Ahsoka.Kernel.Extensions.Api;
using Ahsoka.Kernel.Extensions.Infrastructures;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenTelemetry();

builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
    })
    .AddNewtonsoftJson();

builder.AddDbExtension()
    .AddDbMessagingExtension();
builder.AddApplicationExtensionServices();
builder.AddApiExtensionServices();


builder.ConfigureHealthChecks();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
                opts.SerializerOptions.Converters.Add(new ErrorCodeConverter()));
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

builder.Services
    .ConfigureJWT(builder.Configuration);

builder.Services.AddSwagger(builder.Configuration);
builder.Services.Configure<ApplicationSettings>(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.ConfigureHealthChecks();

var configs = app.Services.GetRequiredService<IOptions<ApplicationSettings>>();

app.UseCustomSwagger(configs,
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public static class MyJPIF
{
    public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}