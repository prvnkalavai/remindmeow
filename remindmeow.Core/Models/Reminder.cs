// remindmeow.core/Models/Reminder.cs
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace remindmeow.core.Models
{
    [Table("Reminders")]
    public class Reminder
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public string Question { get; set; } = string.Empty;

        public bool? Answer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedAt { get; set; }

        [Indexed]
        public string UserId { get; set; } = string.Empty;

        public RecurrenceType Recurrence { get; set; }

        public DateTime? NextDueDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Ignore]
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    public enum RecurrenceType
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Custom
    }
}