using Newtonsoft.Json;
using Users.Web.MVC.Models;

namespace UserClient.MVC.Utility {
    /// <summary>
    /// Utility class Deserialize JSON object to User type
    /// </summary>
    public class JSONUserDeserializer
    {
        public static User Deserialize(string data)
        {
            dynamic results = JsonConvert.DeserializeObject(data);

            // if results contains more than one user cloud be used foreach or etc
            dynamic userJson = results["results"][0];

            return new User
            {
                Picture = (string)userJson["picture"]["thumbnail"],
                Name = GetName(userJson),
                Email = (string)userJson["email"],
                Phone = (string)userJson["phone"],
                Dob = (string)userJson["dob"]
            };
        }

        /// <summary>
        /// Helper Method in order to get user full name
        /// </summary>    
        private static string GetName(dynamic userJson)
        {
            return string.Format(
                "{0} {1} {2}",
                userJson["name"]["title"],
                userJson["name"]["first"],
                userJson["name"]["last"]
            );
        }
    }
}