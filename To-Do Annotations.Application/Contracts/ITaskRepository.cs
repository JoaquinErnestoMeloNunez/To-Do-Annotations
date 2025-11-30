using To_Do_Annonations.Domain.Entities;

namespace To_Do_Annotations.Application.Contracts
{
    public interface ITaskRepository
    {
        Task<IEnumerable<ToDoTask>> GetAllAsync();
        Task<ToDoTask?> GetByIdAsync(int id);
        Task AddAsync(ToDoTask task);
        Task UpdateAsync(ToDoTask task);
        Task DeleteAsync(int id);
    }
}
