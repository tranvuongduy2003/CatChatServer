using ILogger = Serilog.ILogger;

namespace CatChatServer.Filters;

public sealed class AggregateErrorFilter(ILogger logger) : IErrorFilter
{
    public IError OnError(IError error)
    {
        logger.Error(error.Exception, "Unhandled Exception: {@Message}", error.Message);

        return error;
    }
}
