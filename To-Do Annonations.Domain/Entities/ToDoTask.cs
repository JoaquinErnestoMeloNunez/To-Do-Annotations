using System.ComponentModel.DataAnnotations;
using TaskStatus = To_Do_Annonations.Domain.Enums.TaskStatus;

namespace To_Do_Annonations.Domain.Entities
{
    public class ToDoTask
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must add a title. It's mandatory.")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
    }
}
