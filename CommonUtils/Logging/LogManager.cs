namespace Common.Utils.Logging;

public sealed class LogManager : IDisposable//ILoggerProvider, 
{
    private bool disposedValue;

    //public ILogger CreateLogger(string categoryName)
    //{
    //    ILoggerFactory loggerFactory = new LoggerFactory();
    //    return loggerFactory.CreateLogger(categoryName);
    //}

    //public void Log(ILogger logger, object logMessage)
    //{
    //    logger.LogInformation("Hello World! Logging is {Description}.", logMessage.ToString());
    //}

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
