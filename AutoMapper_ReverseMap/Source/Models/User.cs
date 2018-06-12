namespace ConfAutoMapper.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Location Location { get; set; }
        public bool isPrivate { get; set; }
    }
}