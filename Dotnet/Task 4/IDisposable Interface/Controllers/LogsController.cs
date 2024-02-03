// Title        : Food Ordering System ( FoodOrderingSystem API )
// Description  : Manages user authentication, account creation, login, profile management, cart operations,
//                and order handling for a food ordering system
// Author       : Arularasi J
// Created at   : 21/07/2023
// Updated at   : 20/12/2023
// Reviewed by  : 
// Reviewed at  : 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using FoodOrderingSystemAPI.Models;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogger<LogsController> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, LogLevel> _logLevelMappings;

    public LogsController(ILogger<LogsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        _logLevelMappings = _configuration.GetSection("LogLevelMappings")
                                          .Get<Dictionary<string, LogLevel>>();
        // _logLevelMappings = new Dictionary<string, LogLevel>
        // {
        //     {"Information", LogLevel.Information},
        //     {"Warning", LogLevel.Warning},
        //     {"Error", LogLevel.Error},
        //     {"Critical", LogLevel.Critical},
        //     {"Trace", LogLevel.Trace},
        //     {"Debug", LogLevel.Debug},
        //     {"None", LogLevel.None}
        // };

    }

    [HttpPost]
    public IActionResult LogMessage([FromBody] LogModel logmodel)
    {
        try
        {
            // Check if the provided log level is valid
            if (!_logLevelMappings.ContainsKey(logmodel.logLevel))
            {
                // return BadRequest("Invalid log level.");
                var errorMessage = _configuration["ErrorMessages:InvalidLogLevel"];
                return BadRequest(errorMessage);
            }

            LogLevel selectedLogLevel = _logLevelMappings[logmodel.logLevel];

            if (!string.IsNullOrEmpty(logmodel.error))
            {
                var logTemplate = _configuration["LogMessageTemplates:WithErrorMessage"];
                _logger.Log(selectedLogLevel, logTemplate, logmodel.logMessage, selectedLogLevel.ToString(), logmodel.error);
            }
            else
            {
                var logTemplate = _configuration["LogMessageTemplates:WithoutErrorMessage"];
                _logger.Log(selectedLogLevel, logTemplate, logmodel.logMessage, selectedLogLevel.ToString());
            }

            return Ok();
        }
        catch (Exception exception)
        {
            // Log the exception and return a 500 Internal Server Error
            var internalServerErrorSettings = _configuration.GetSection("Errors:InternalServerError");
            var statusCode = int.Parse(internalServerErrorSettings["StatusCode"]);
            var errorMessage = internalServerErrorSettings["Message"];
            _logger.LogError(exception, errorMessage);
            return StatusCode(statusCode, errorMessage);
        }
    }
}
