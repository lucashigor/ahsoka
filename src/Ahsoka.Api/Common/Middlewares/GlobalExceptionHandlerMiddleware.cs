namespace Ahsoka.Api.Common.Middlewares;

using Ahsoka.Application.Common.Exceptions;
using Ahsoka.Application.Dto.Common.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Application.Dto.Common.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error | {request_path}", context.Request.Path.Value);
            await HandleExceptionAsync(context, ex);
        }
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        JsonSerializerOptions jsonOptions = new() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        jsonOptions.Converters.Add(new ErrorCodeConverter());

        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        string serialized;
        int statusCode;

        switch (exception)
        {
            case IApiException apiException:
                statusCode = (int)apiException.Status;
                serialized = JsonSerializer.Serialize(new DefaultResponse<object>(null!, apiException.Errors, traceId),
                    jsonOptions);
                break;

            case DbUpdateException:
            case DBConcurrencyException:
                statusCode = (int)HttpStatusCode.InternalServerError;
                serialized = JsonSerializer.Serialize(
                    new DefaultResponse<object>(null!, Errors.GenericDataBaseError().ChangeInnerMessage(exception.Message), traceId),
                    jsonOptions);
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                serialized = JsonSerializer.Serialize(
                    new DefaultResponse<object>(null!, Errors.Generic().ChangeInnerMessage(exception.Message), traceId),
                    jsonOptions);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(serialized);
    }
}