// remindmeow.Infrastructure/Data/LocalStorageService.cs
using SQLite;
using remindmeow.core.Interfaces;
using Xamarin.Essentials;
using remindmeow.core.Models;

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
            
            if (reminder.Id == null)
            {
                reminder.Id = Guid.NewGuid().ToString();
            }

            reminder.LastModifiedAt = DateTime.UtcNow;

            if (await GetByIdAsync(reminder.Id) == null)
            {
                return await _database.InsertAsync(reminder);
            }
            
            return await _database.UpdateAsync(reminder);
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