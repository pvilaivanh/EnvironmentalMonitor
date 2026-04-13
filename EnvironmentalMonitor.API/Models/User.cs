namespace EnvironmentalMonitor.API.Models
{
    public class User
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string StartingLocation { get; set; }
        public string Theme { get; set; }


    }
}
