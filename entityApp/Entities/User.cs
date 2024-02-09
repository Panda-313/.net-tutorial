namespace EntityApp.Entities
{
    public class User
    {
        public Guid Id{get;set;}
        public string FullName { get; set; }
        public string Email { get; set; }
        public Address Address{get;set;}
        public List<WorkItem> WorkItems = new List<WorkItem>();
        public List<Comment> Comments = new List<Comment>();
    }
}