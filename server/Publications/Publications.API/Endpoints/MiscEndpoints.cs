

using Microsoft.AspNetCore.Mvc;
using Publications.Application.Repositories;

class EmailDto
{
    public string Email { get; set; }
}

internal static class MiscEndpoints
{
    public static void MapMiscEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/subscriptions", AddSubscriptionAsync);
        builder.MapGet("/subscriptions", GetSubscriptionsAsync);
    }
    
    private static async Task<IResult> AddSubscriptionAsync(
        [FromBody]EmailDto email,
        [FromServices]IRequestsRepository requestsRepository,
        [FromServices]ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("/subscriptions (POST) endpoint hit");
        
        if (!IsValidEmail(email.Email))
        {
            logger.LogWarning("Invalid email address provided");
            return Results.BadRequest(new { error = "Invalid email address" });
        }

        var subscription = new Subscription(email.Email);

        try
        {
            await requestsRepository.AddSubscriptionAsync(subscription);
        }
        catch
        {
            logger.LogError("Failed to add subscription");
            //todo: add error handling
        }
        
        return Results.Ok();
    }
    
    private static async Task<IResult> GetSubscriptionsAsync(
        [FromQuery]string key,
        [FromServices]IRequestsRepository requestsRepository,
        [FromServices]ILogger logger,
        [FromServices]IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (key != configuration["DbSync:Key"])
        {
            logger.LogWarning("Unauthorized access to /subscriptions endpoint");
            return Results.Unauthorized();
        }
                
        logger.LogInformation("/subscriptions endpoint hit");
        
        var subscriptions = await requestsRepository.GetSubscriptionsAsync(cancellationToken);
        var onlyEmails = subscriptions.Select(s => s.Email).ToArray();
        
        return Results.Ok(onlyEmails);
    }
    
    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith('.')) {
            return false;
        }
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch {
            return false;
        }
    }
}