using FCG.Domain.Entities;
using FCG.Domain.Repositories;
using FCG.Infrastructure.Context;

namespace FCG.Infrastructure.Repositories
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly FiapCloudGamesDbContext _context;

        // Injeta o DbContext via construtor
        public LoggerRepository(FiapCloudGamesDbContext context)
        {
            _context = context;
        }

        public async Task LogTraceAsync(Trace log)
        {
            await _context.Traces.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogRequestAsync(RequestLog log)
        {
            await _context.RequestLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestLogAsync(RequestLog log)
        {
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }
}
