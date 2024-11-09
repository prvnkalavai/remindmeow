// remindmeow.Tests/RemindersServiceTests.cs
using remindmeow.core.Interfaces;
using remindmeow.core.Models;
using remindmeow.Infrastructure.Services;
using Moq;
using Microsoft.Extensions.Logging;

namespace remindmeow.Tests
{
    public class RemindersServiceTests
    {
        private readonly Mock<ILocalStorageService> _mockStorage;
        private readonly Mock<ILogger<RemindersService>> _mockLogger;
        private readonly RemindersService _service;

        public RemindersServiceTests()
        {
            _mockStorage = new Mock<ILocalStorageService>();
            _mockLogger = new Mock<ILogger<RemindersService>>();
            _service = new RemindersService(_mockStorage.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateReminderAsync_ValidReminder_SavesAndReturnsReminder()
        {
            // Arrange
            var reminder = new Reminder { Question = "Test?" };
            _mockStorage.Setup(x => x.SaveAsync(It.IsAny<Reminder>()))
                       .ReturnsAsync(1);

            // Act
            var result = await _service.CreateReminderAsync(reminder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reminder.Question, result.Question);
            _mockStorage.Verify(x => x.SaveAsync(It.IsAny<Reminder>()), Times.Once);
        }

        [Fact]
        public async Task UpdateReminderAsync_NonexistentReminder_ThrowsKeyNotFoundException()
        {
            // Arrange
            var reminder = new Reminder { Id = "test", Question = "Test?" };
            _mockStorage.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                       .ReturnsAsync((Reminder)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateReminderAsync(reminder.Id, reminder));
        }

        [Theory]
        [InlineData(RecurrenceType.Daily, 1)]
        [InlineData(RecurrenceType.Weekly, 7)]
        [InlineData(RecurrenceType.Monthly, 30)] // Approximate
        public void CalculateNextDueDate_ValidRecurrence_ReturnsCorrectDate(RecurrenceType recurrenceType, int expectedDays)
        {
            // Arrange
            var baseDate = DateTime.UtcNow;
            var reminder = new Reminder
            {
                IsActive = true,
                Recurrence = recurrenceType,
                CreatedAt = baseDate
            };

            // Act
            var result = _service.CalculateNextDueDate(reminder, baseDate);

            // Assert
            Assert.NotNull(result);
            if (recurrenceType == RecurrenceType.Monthly)
            {
                Assert.Equal(baseDate.AddMonths(1).Date, result.Value.Date);
            }
            else
            {
                Assert.Equal(baseDate.AddDays(expectedDays).Date, result.Value.Date);
            }
        }

        [Fact]
        public void CalculateNextDueDate_InactiveReminder_ReturnsNull()
        {
            // Arrange
            var reminder = new Reminder
            {
                IsActive = false,
                Recurrence = RecurrenceType.Daily
            };

            // Act
            var result = _service.CalculateNextDueDate(reminder);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CalculateNextDueDate_PastDate_ReturnsFutureDate()
        {
            // Arrange
            var pastDate = DateTime.UtcNow.AddDays(-5);
            var reminder = new Reminder
            {
                IsActive = true,
                Recurrence = RecurrenceType.Daily,
                CreatedAt = pastDate,
                NextDueDate = pastDate
            };

            // Act
            var result = _service.CalculateNextDueDate(reminder);

            // Assert
            Assert.NotNull(result);
            Assert.True(result > DateTime.UtcNow);
        }
    }
}