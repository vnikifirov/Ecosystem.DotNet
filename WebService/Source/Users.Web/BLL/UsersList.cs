using System.Collections.Generic;
using Users.Web.MVC.Models;

namespace Users.Web.MVC.BLL
{
    /// <summary>
    /// Users List Class to store retrieved random users from Service
    /// </summary>
    public class UsersList
    {
        /// <summary>
        /// Users collection
        /// </summary>
        public List<User> Users
        {
            get;
            private set;
        }

        /// <summary>
        /// Initialize instance of UserList class
        /// </summary>
        public UsersList()
        {
            Users = new List<User>();
        }

        /// <summary>
        /// Get random users from provided
        /// </summary>        
        public List<User> GetUsers(IUserService service, int quantity = 10)
        {
            for (int i = 0; i < quantity; i++)
            {
                // Getting a random user from service
                User user = service.GetRandomUser();

                if (user != null)
                {
                    // Add a radom user to collection
                    Users.Add(user);
                }

            }
            
            return Users;
        }
    }
}