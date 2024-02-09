using Microsoft.EntityFrameworkCore;

namespace EntityApp.Entities
{
    public class MyBoardsContext : DbContext
    {

        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options)
        {

        }

        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<WorkItemState> WorkItemStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<WorkItemState>().Property(s => s.Value).IsRequired().HasMaxLength(60);

            modelBuilder.Entity<Epic>(eb =>
            {
                eb.Property(epic => epic.EndDate).HasPrecision(3);
            });

            modelBuilder.Entity<Task>(eb =>
            {
                eb.Property(task => task.RemainingWork).HasPrecision(14, 2);
                eb.Property(task => task.Activity).HasMaxLength(200);
            });

            modelBuilder.Entity<Issue>(eb =>
            {
                eb.Property(issue => issue.Efford).HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(workItem => workItem.Area).HasColumnType("varchar(200)");
                eb.Property(workItem => workItem.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(workItem => workItem.Priority).HasDefaultValue(1);
                eb.HasMany(workItem => workItem.Comments).WithOne(comment => comment.WorkItem).HasForeignKey(comment => comment.WorkItemId);
                eb.HasOne(workItem => workItem.Author).WithMany(user => user.WorkItems).HasForeignKey(workItem => workItem.AuthorId);
                eb.HasMany(workItem => workItem.Tags).WithMany(tag => tag.WorkItems).UsingEntity<WorkItemTag>(
                    workItemTag => workItemTag.HasOne(workITag => workITag.Tag).WithMany().HasForeignKey(wit => wit.TagId),
                    workItemTag => workItemTag.HasOne(workITag => workITag.WorkItem).WithMany().HasForeignKey(wit => wit.WorkItemId),
                    wit =>
                    {
                        wit.HasKey(x => new { x.TagId, x.WorkItemId });
                        wit.Property(x => x.PublicationDate).HasDefaultValueSql("GETUTCDATE()");
                    }
                );

                eb.HasOne(workItem => workItem.State).WithMany().HasForeignKey(workItem => workItem.StateId);
            });



            modelBuilder.Entity<Comment>(eb =>
            {
                eb.Property(comment => comment.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                eb.Property(comment => comment.UpdatedDate).ValueGeneratedOnUpdate();
                eb.HasOne(comment => comment.Author)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>().HasOne(user => user.Address).WithOne(address => address.User).HasForeignKey<Address>(address => address.UserId);

            modelBuilder.Entity<WorkItemState>()
            .HasData(
                new WorkItemState() { Id =1 ,Value = "To Do" },
                new WorkItemState() { Id = 2,Value = "Doing" },
                new WorkItemState() { Id = 3,Value = "Done" });


            modelBuilder.Entity<Tag>().HasData(
                new Tag(){Id = 1, Value = "Web"},
                new Tag(){Id = 2, Value = "UI"},
                new Tag(){Id = 3, Value = "Desktop"},
                new Tag(){Id = 4, Value = "API"},
                new Tag(){Id = 5, Value = "Service"}
            );
        }
    }
}