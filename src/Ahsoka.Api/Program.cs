using Ahsoka.Kernel;
using Ahsoka.Application.Dto;
using MediatR;
using Mapster;
using Ahsoka.Application;
using Ahsoka.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddOpenTelemetry();
builder.AddInfrastructureServices();
builder.AddApplicationExtensionServices();
builder.AddApiExtensionServices();

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
                opts.SerializerOptions.Converters.Add(new ErrorCodeConverter()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapPost("/api/v1/configurations", async (RegisterConfigurationInput input,
    IMediator _mediator,
    CancellationToken cancellationToken ) =>
{
    if (input == null)
    {
        return Results.UnprocessableEntity(new DefaultResponse<object>());
    }
    
    var entity = input.Adapt<RegisterConfigurationCommand>();

    var output = await _mediator.Send(entity, cancellationToken);

    if(output is null)
    {    
        return Results.BadRequest(new DefaultResponse<object>());
    }
    
    return Results.Created($"/todoitems/{output.Id}",new DefaultResponse<object>(output));
})
.WithName("CreateConfiguration")
.WithOpenApi();

app.MapPrometheusScrapingEndpoint();

app.Run();
