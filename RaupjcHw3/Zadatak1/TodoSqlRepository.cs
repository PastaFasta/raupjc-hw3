using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaupjcHw3.Database;
using RaupjcHw3.Interfaces;

namespace RaupjcHw3
{
    class TodoSqlRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }

        #region ITodoRepository

        public async Task<TodoItem> GetAsync(Guid todoId, Guid userId)
        {
            TodoItem todoItem = await _context.TodoItems.Include(t => t.Labels).SingleOrDefaultAsync(t => t.Id == todoId);
            if (todoItem != null && todoItem.UserId != userId)
            {
                throw new TodoAccessDeniedException();
            }
            return todoItem;
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            if (await _context.TodoItems.AnyAsync(t => t.Id == todoItem.Id))
            {
                throw new TodoAccessDeniedException();
            }
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Guid todoId, Guid userId)
        {
            TodoItem item = await GetAsync(todoId, userId);
            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task UpdateAsync(TodoItem todoItem, Guid userId)
        {
            if (todoItem == null || todoItem.UserId != userId)
            {
                throw new TodoAccessDeniedException();
            }
            if (await _context.TodoItems.AnyAsync(t => t.Id != todoItem.Id))
            {
                await AddAsync(todoItem);
            }
            else
            {
                _context.Entry(todoItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> MarkAsCompletedAsync(Guid todoId, Guid userId)
        {
            TodoItem item = await GetAsync(todoId, userId);
            if (item != null)
            {
                if (item.MarkAsCompleted())
                {
                    await UpdateAsync(item, userId);
                    return true;
                }
            }
            return false;
        }

        public async Task<List<TodoItem>> GetAllAsync(Guid userId)
        {
            return await _context.TodoItems.Include(t => t.Labels)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<List<TodoItem>> GetActiveAsync(Guid userId)
        {
            return await _context.TodoItems.Include(t => t.Labels).Where(t => t.UserId == userId && !t.IsCompleted)
                .ToListAsync();
        }

        public async Task<List<TodoItem>> GetCompletedAsync(Guid userId)
        {
            return await _context.TodoItems.Include(t => t.Labels).Where(t => t.UserId == userId && t.IsCompleted)
                .ToListAsync();
        }

        public async Task<List<TodoItem>> GetFilteredAsync(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            return await _context.TodoItems.Include(t => t.Labels).Where(t => t.UserId == userId).Where(t => filterFunction(t))
                .ToListAsync();
        }

        #endregion

    }
}
