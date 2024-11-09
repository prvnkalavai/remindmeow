// remindmeow.Infrastructure/Data/LocalStorageService.cs
using SQLite;
using remindmeow.core.Interfaces;
using remindmeow.core.Models;
using remindmeow.Core.Exceptions;

namespace remindmeow.Infrastructure.Data
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _isInitialized;

        public LocalStorageService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "reminders.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                await _database.CreateTableAsync<Reminder>();
                _isInitialized = true;
            }
        }

        public async Task<IEnumerable<Reminder>> GetAllAsync()
        {
            await InitializeAsync();
            return await _database.Table<Reminder>().ToListAsync();
        }

        public async Task<Reminder> GetByIdAsync(string id)
        {
            await InitializeAsync();
            return await _database.Table<Reminder>()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<int> SaveAsync(Reminder reminder)
        {
            await InitializeAsync();

            ValidateReminder(reminder);

            if (string.IsNullOrEmpty(reminder.Id))
            {
                reminder.Id = Guid.NewGuid().ToString();
            }

            reminder.LastModifiedAt = DateTime.UtcNow;

            try
            {
                if (await GetByIdAsync(reminder.Id) == null)
                {
                    return await _database.InsertAsync(reminder);
                }
                return await _database.UpdateAsync(reminder);
            }
            catch (SQLiteException ex)
            {
                throw new InvalidOperationException("Failed to save reminder", ex);
            }
        }

        private void ValidateReminder(Reminder reminder)
        {
            if (string.IsNullOrEmpty(reminder.Question))
                throw new ReminderValidationException("Question is required");

            if (reminder.Question.Length > 500)
                throw new ReminderValidationException("Question must not exceed 500 characters");

            if (string.IsNullOrEmpty(reminder.UserId))
                throw new ReminderValidationException("UserId is required");

            if (reminder.NextDueDate.HasValue && reminder.NextDueDate.Value < DateTime.UtcNow.AddYears(-1))
                throw new ReminderValidationException("Next due date cannot be more than 1 year in the past");
        }

        public async Task<int> DeleteAsync(string id)
        {
            await InitializeAsync();
            return await _database.DeleteAsync<Reminder>(id);
        }
        public async Task<IEnumerable<Reminder>> GetActiveRemindersAsync()
        {
            await InitializeAsync();
            return await _database.Table<Reminder>()
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reminder>> GetDueRemindersAsync()
        {
            await InitializeAsync();
            var now = DateTime.UtcNow;
            return await _database.Table<Reminder>()
                .Where(r => r.IsActive && r.NextDueDate <= now)
                .OrderBy(r => r.NextDueDate)
                .ToListAsync();
        }
    }
}