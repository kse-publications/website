﻿using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Publications.API.BackgroundJobs;
using Publications.API.Models;
using Publications.API.Repositories.Requests;

namespace Publications.API.Extensions;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapSyncEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/EB292BF0-E995-491A-A98E-6121601E1069/sync", 
            (ILogger<Program> logger, IScheduler scheduler) => 
            {
                logger.LogInformation("/sync endpoint hit");
                scheduler.Schedule<SyncWithNotionBackgroundTask>()
                    .EverySecond()
                    .Once()
                    .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
            });

        endpoints.MapGet("/views", async ([FromServices] IRequestsRepository requestsRepository) =>
        {
            Dictionary<int, int> views = await requestsRepository
                .GetResourceDistinctViews<Publication>();
            
            return views;
        });
        
        return endpoints;
    }
}