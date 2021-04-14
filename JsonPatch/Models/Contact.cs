using System;
using System.Collections.Generic;

namespace JsonPatch.Models
{
    public class Contact : ICloneable
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public List<string> Links { get; set; }


        public Contact()
        {
        }

        public object Clone()
        {
            return new Contact {
                UserId = this.UserId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Age = this.Age,
                Links = new List<string>(this.Links)
            };
        }
    }
}