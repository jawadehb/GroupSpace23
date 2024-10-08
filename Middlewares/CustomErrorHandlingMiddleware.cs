﻿using GroupSpace23;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

public class CustomErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomErrorHandlingMiddleware> _logger;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public CustomErrorHandlingMiddleware(RequestDelegate next, ILogger<CustomErrorHandlingMiddleware> logger, IStringLocalizer<SharedResources> localizer)
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Call the next middleware in the pipeline

            // Handle status codes that aren't 200 (OK)
            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                _logger.LogWarning("404 error occurred.");
                await HandleExceptionAsync(context, 404);
            }
            else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600)
            {
                _logger.LogError($"An unexpected error occurred. Status Code: {context.Response.StatusCode}");
                await HandleExceptionAsync(context, context.Response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, 500); // 500 Internal Server Error for unhandled exceptions
        }
    }

    private Task HandleExceptionAsync(HttpContext context, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/html";

        // Set the appropriate error message based on the status code
        string errorMessage = statusCode switch
        {
            404 => _localizer["Sorry, de pagina die u zoekt, kon niet worden gevonden."],
             _ => _localizer["Er is een onverwachte fout opgetreden. Probeer het later opnieuw."]
        };;

        // Redirect to the Error view with the appropriate message
        context.Items["ErrorMessage"] = errorMessage;
        context.Items["StatusCode"] = statusCode;
        string goBackHomeText = _localizer["Ga terug naar home pagina"];

        var result = $"<html><body><h1>Error {statusCode}</h1><p>{errorMessage}</p><a href='/'>{goBackHomeText}</a></body></html>";
        return context.Response.WriteAsync(result);
    }
}