using FCG.Domain.Entities;

namespace FCG.Domain.Repositories
{
    public interface ILoggerRepository
    {
        Task LogTraceAsync(Trace log);
        Task LogRequestAsync(RequestLog log);
        Task UpdateRequestLogAsync(RequestLog log);
    }
}
