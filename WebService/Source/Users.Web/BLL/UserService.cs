using System;
using System.IO;
using System.Net;
using UserClient.MVC.Utility;
using Users.Web.MVC.BLL;
using Users.Web.MVC.Models;

namespace UserClient.MVC.BLL
{
    /// <summary>
    /// Implemented Serivce returns a random user
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Request Url for external API
        /// </summary>
        private readonly string requestUrl = "https://randomuser.me/api";


        /// <summary>
        /// GET: Make a request to an external resource and returning a random User
        /// </summary>    
        public User GetRandomUser()
        {
            try
            {
                // Create a request for the URL https://randomuser.me/api .
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;

                // Get the response for the URL https://randomuser.me/api .
                using (var response = request.GetResponse() as HttpWebResponse)
                {

                    if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).", response.StatusCode,
                    response.StatusDescription));

                    // Get the stream containing content returned by the server.  
                    Stream dataStream = response.GetResponseStream();

                    var reader = new StreamReader(dataStream);

                    // Read the content.  
                    string responseFromServer = reader.ReadToEnd();

                    // Convert the JSON file to User 
                    User user = JSONUserDeserializer.Deserialize(responseFromServer);

                    // Clean up the streams and the response.  
                    reader.Dispose();

                    return user;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return null;
            }
        }
    }

}