using Microsoft.EntityFrameworkCore;
using Thinktecture;
using ToDoList.Task;
using ToDoList.User;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace ToDoList.Infrastructure;

public class ToDoContext: DbContext
{
    
    public ToDoContext (DbContextOptions<ToDoContext> options): base(options)
    {
    }
    public virtual DbSet<User.User> Users { get; set; }
    public virtual DbSet<Task.Task> Tasks { get; set; }
    public virtual DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddThinktectureValueConverters();
        modelBuilder.HasPostgresEnum<TaskStatus>("task_status");
        modelBuilder.HasPostgresEnum<TaskPriority>("task_priority");
        modelBuilder.Entity<User.User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.UserId);
            entity.Property(e => e.UserId).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.PassHash).HasColumnName("pass_hash").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(s => s.SessionId);
            entity.Property(s => s.SessionId).HasColumnName("session_id").ValueGeneratedOnAdd();
            entity.Property(s => s.UserId).HasColumnName("user_id").ValueGeneratedOnAdd();
            entity.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .IsRequired();
            entity.Property(s => s.RefreshToken).HasColumnName("refresh_token").IsRequired();
            entity.Property(s => s.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<Task.Task>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(t => t.TaskId);
            entity.Property(t => t.TaskId).HasColumnName("task_id").ValueGeneratedOnAdd();
            entity.Property(t => t.OwnerId).HasColumnName("owner_id");
            entity.HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .IsRequired();
            entity.Property(t => t.Title).HasColumnName("title").IsRequired();
            entity.Property(t => t.Description).HasColumnName("description").IsRequired();
            entity.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(t => t.DueDate).HasColumnName("due_date").IsRequired();
            entity.Property(t => t.UpdatedAt).HasColumnName("updated_at");
            entity.Property(t => t.DeletedAt).HasColumnName("deleted_at");
            entity.Property(t => t.Status).HasColumnName("status").IsRequired();
            entity.Property(t => t.Priority).HasColumnName("priority").IsRequired();
            
            entity.HasQueryFilter(t => t.DeletedAt == null);
        });
    }
}