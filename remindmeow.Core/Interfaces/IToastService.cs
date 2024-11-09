//remindmeow.Core/Interfaces/IToastService.cs
namespace remindmeow.Core.Interfaces
{
    public interface IToastService
    {
        void ShowSuccess(string message);
        void ShowError(string message);
        void ShowWarning(string message);
        void ShowInfo(string message);
    }
}