using System;
using System.Data.Entity;

namespace RaupjcHw3.Database
{
    public class TodoDbContext : DbContext
    {
        public IDbSet<TodoItem> TodoItems { get; set; }
        public IDbSet<TodoItemLabel> TodoItemLabels { get; set; }

        public TodoDbContext(String s) : base(s)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>().HasKey(t => t.Id);
            modelBuilder.Entity<TodoItem>().Property(t => t.Text).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(t => t.IsCompleted).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(t => t.DateCompleted).IsOptional();
            modelBuilder.Entity<TodoItem>().Property(t => t.DateCreated).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(t => t.UserId).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(t => t.DateDue).IsOptional();
            modelBuilder.Entity<TodoItem>().Property(t => t.IsCompleted).IsRequired();
            modelBuilder.Entity<TodoItemLabel>().HasKey(t => t.Id);
            modelBuilder.Entity<TodoItemLabel>().Property(t => t.Value).IsRequired();

            modelBuilder.Entity<TodoItem>().HasMany(t => t.Labels).WithMany(t => t.LabelTodoItems);
            modelBuilder.Entity<TodoItemLabel>().HasMany(t => t.LabelTodoItems).WithMany(t => t.Labels);
        }
    }
}