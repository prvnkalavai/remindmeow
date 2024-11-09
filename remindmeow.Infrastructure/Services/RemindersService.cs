//remindmeow.Infrastructure/Services/RemindersService.cs
using Microsoft.Extensions.Logging;
using remindmeow.core.Interfaces;
using remindmeow.core.Models;
using remindmeow.Core.Extensions;

namespace remindmeow.Infrastructure.Services
{
    public class RemindersService : IRemindersService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ILogger<RemindersService> _logger;

        public RemindersService(ILocalStorageService localStorage, ILogger<RemindersService> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            try
            {
                _logger.LogInformation("Creating new reminder: {Question}", reminder.Question);

                reminder.NextDueDate = CalculateNextDueDate(reminder);
                await _localStorage.SaveAsync(reminder);

                _logger.LogInformation("Successfully created reminder {Id}", reminder.Id);
                return reminder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create reminder");
                throw;
            }
        }

        public async Task<bool> DeleteReminderAsync(string id)
        {
            var result = await _localStorage.DeleteAsync(id);
            return result > 0;
        }

        public async Task<IEnumerable<Reminder>> GetActiveRemindersAsync()
        {
            return await _localStorage.GetActiveRemindersAsync();
        }

        public async Task<IEnumerable<Reminder>> GetDueRemindersAsync()
        {
            return await _localStorage.GetDueRemindersAsync();
        }

        public async Task<Reminder> GetReminderByIdAsync(string id)
        {
            return await _localStorage.GetByIdAsync(id);
        }

        public async Task<Reminder> UpdateReminderAsync(string id, Reminder reminder)
        {
            try
            {
                _logger.LogInformation("Updating reminder {Id}", id);

                var existing = await _localStorage.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Reminder {Id} not found", id);
                    throw new KeyNotFoundException($"Reminder with ID {id} not found");
                }

                reminder.Id = id;
                reminder.NextDueDate = CalculateNextDueDate(reminder);
                await _localStorage.SaveAsync(reminder);

                _logger.LogInformation("Successfully updated reminder {Id}", id);
                return reminder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update reminder {Id}", id);
                throw;
            }
        }

        private void UpdateNextDueDate(Reminder reminder)
        {
            reminder.NextDueDate = reminder.CalculateNextDueDate();
        }

        public DateTime? CalculateNextDueDate(Reminder reminder, DateTime? baseDate = null)
        {
            try
            {
                if (!reminder.IsActive || reminder.Recurrence == RecurrenceType.None)
                {
                    return null;
                }

                baseDate ??= reminder.NextDueDate ?? reminder.CreatedAt;

                // Ensure we're not calculating from a date too far in the past
                if (baseDate < DateTime.UtcNow.AddYears(-1))
                {
                    baseDate = DateTime.UtcNow;
                }

                DateTime? nextDueDate = reminder.Recurrence switch
                {
                    RecurrenceType.Daily => baseDate.Value.AddDays(1),
                    RecurrenceType.Weekly => baseDate.Value.AddDays(7),
                    RecurrenceType.Monthly => baseDate.Value.AddMonths(1),
                    RecurrenceType.Custom => CalculateCustomRecurrence(reminder, baseDate.Value),
                    _ => null
                };

                // If the calculated date is in the past, keep adding intervals until we get a future date
                while (nextDueDate.HasValue && nextDueDate.Value <= DateTime.UtcNow)
                {
                    nextDueDate = CalculateNextDueDate(reminder, nextDueDate);
                }

                return nextDueDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating next due date for reminder {Id}", reminder.Id);
                throw;
            }
        }

        private DateTime? CalculateCustomRecurrence(Reminder reminder, DateTime baseDate)
        {
            // Example implementation for custom recurrence using metadata
            if (reminder.Metadata.TryGetValue("RecurrencePattern", out var pattern))
            {
                // Pattern format: "interval:unit" (e.g., "2:days", "3:weeks", "1:months")
                var parts = pattern.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out var interval))
                {
                    return parts[1].ToLower() switch
                    {
                        "days" => baseDate.AddDays(interval),
                        "weeks" => baseDate.AddDays(interval * 7),
                        "months" => baseDate.AddMonths(interval),
                        _ => null
                    };
                }
            }
            return null;
        }
    }
}