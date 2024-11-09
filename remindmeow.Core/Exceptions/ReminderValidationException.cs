// remindmeow.Core/Exceptions/ReminderValidationException.cs
namespace remindmeow.Core.Exceptions
{
    public class ReminderValidationException : Exception
    {
        public ReminderValidationException(string message) : base(message) { }
    }
}