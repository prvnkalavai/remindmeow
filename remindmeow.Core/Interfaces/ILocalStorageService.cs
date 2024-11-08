// remindmeow.core/Interfaces/ILocalStorageService.cs
using remindmeow.core.Models;

namespace remindmeow.core.Interfaces
{
    public interface ILocalStorageService
    {
        Task<IEnumerable<Reminder>> GetAllAsync();
        Task<Reminder> GetByIdAsync(string id);
        Task<int> SaveAsync(Reminder reminder);
        Task<int> DeleteAsync(string id);
        Task<IEnumerable<Reminder>> GetActiveRemindersAsync();
        Task<IEnumerable<Reminder>> GetDueRemindersAsync();
    }
}