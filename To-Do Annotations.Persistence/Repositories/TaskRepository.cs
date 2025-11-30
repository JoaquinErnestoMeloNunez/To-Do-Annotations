using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using To_Do_Annonations.Domain.Entities;
using To_Do_Annotations.Application.Contracts;
using To_Do_Annotations.Persistence.Database;
using AppContext = To_Do_Annotations.Persistence.Database.AppContext;

namespace To_Do_Annotations.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppContext _context;
        private readonly ILogger<TaskRepository> _logger; 

        public TaskRepository(AppContext context, ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(ToDoTask task)
        {
            try
            {
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating the task {TaskTitle}", task.Title);
                throw new Exception("Error while creating the task in the Database", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task != null)
                {
                    _context.Tasks.Remove(task);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting de task with ID {TaskId}", id);
                throw new Exception($"Error while deleting de task with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<ToDoTask>> GetAllAsync()
        {
            try
            {
                return await _context.Tasks.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting the task list.");
                throw new Exception("Error while getting the task list.", ex);
            }
        }

        public async Task<ToDoTask?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Tasks.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while searching the task with ID {TaskId}", id);
                throw new Exception($"Error while searching the task with ID {id}.", ex);
            }
        }

        public async Task UpdateAsync(ToDoTask task)
        {
            try
            {
                _context.Entry(task).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the task with ID {TaskId}", task.Id);
                throw new Exception($"Error updating the task with ID {task.Id}.", ex);
            }
        }
    }
}
