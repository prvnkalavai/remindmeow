// remindmeow.Core/Services/LoggerService.cs
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace remindmeow.Core.Services
{
    public class LoggerService<T> : ILogger<T>
    {
        private readonly ConcurrentQueue<LogEntry> _logs = new();
        private readonly string _categoryName;
        private const int MaxLogEntries = 1000;

        public LoggerService()
        {
            _categoryName = typeof(T).Name;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception);
            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = logLevel,
                Category = _categoryName,
                Message = message,
                Exception = exception,
                EventId = eventId
            };

            _logs.Enqueue(entry);

            // Trim old logs if queue gets too large
            while (_logs.Count > MaxLogEntries && _logs.TryDequeue(out _)) { }

            // Also write to debug output for development
#if DEBUG
            Debug.WriteLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {entry.Level}: {entry.Message}");
            if (exception != null)
            {
                Debug.WriteLine($"Exception: {exception}");
            }
#endif
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return new LogScope<TState>(state);
        }

        public IEnumerable<LogEntry> GetRecentLogs(int count = 100)
        {
            return _logs.Take(count).OrderByDescending(l => l.Timestamp);
        }

        public void Clear()
        {
            while (_logs.TryDequeue(out _)) { }
        }
    }

    public class LogScope<TState> : IDisposable
    {
        private readonly TState _state;
        private bool _disposed;

        public LogScope(TState state)
        {
            _state = state;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public EventId EventId { get; set; }
    }
}