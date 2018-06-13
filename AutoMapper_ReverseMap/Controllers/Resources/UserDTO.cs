using ConfAutoMapper.Models;

namespace ConfAutoMapper.Controllers.Resources
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Location Location { get; set; }
        public bool isPrivate { get; set; }
    }
}