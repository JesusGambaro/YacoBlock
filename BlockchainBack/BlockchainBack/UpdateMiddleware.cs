namespace BlockchainBack;

public class UpdateMiddleware
{
    private readonly RequestDelegate _next;

    public UpdateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("UpdateMiddleware");
        var mongoDbRepository = new MongoDbRepository();
        var remoteLength = mongoDbRepository.GetChainLength();
        var localLength = Program._blockchainServices.BlockchainLength();
        if (remoteLength > localLength)
        {
            httpContext.Response.StatusCode = 404;
            httpContext.Response.ContentType = "text/plain";
            httpContext.Response.WriteAsync("Please update your blockchain");
            return Task.FromResult(0);
        }

        Console.ResetColor();

        return _next(httpContext);
    }
}