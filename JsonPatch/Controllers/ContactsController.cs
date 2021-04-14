using System;
using System.Collections.Generic;
using JsonPatch.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace JsonPatch.Controllers
{
    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private List<Contact> contacts = new List<Contact>
        {
            new Contact
            {
                UserId = Guid.Parse("62f95c51-318f-424e-92a5-d2c81ba35452"),
                FirstName = "Olga",
                LastName = "Mokalovskya",
                Age = 32,
                Email = "foo@test.com",
                Links = new List<string> { "http://benfoster.io" }
            },
            new Contact
            {
                UserId = Guid.Parse("d70359ef-1c59-4858-9d2f-17d9556f17de"),
                FirstName = "Ben",
                LastName = "Foster",
                Age = 30,
                Email = "replace@test.com",
                Links = new List<string> { "http://benfoster.io" }
            },
            new Contact
            {
                UserId = Guid.Parse("48a62da1-2c6c-45bd-8be8-3816990b370c"),
                FirstName = "Bot",
                LastName = "Facer",
                Age = 35,
                Email = "bar@test.com",
                Links = new List<string> { "http://benfoster.io" }
            }
        };

        // GET api/values
        [HttpGet]
        public IEnumerable<Contact> Get()
        {
            return contacts;
        }

        [HttpPatch("{userId}")]
        public IActionResult Patch([FromRoute(Name = "userId")] Guid userId, [FromBody]JsonPatchDocument<Contact> userProperties)
        {
            var index = contacts.FindIndex( u => u.UserId == userId );

            if (index == -1)
            {
                return BadRequest($"user with {userId} not found.");
            }

            var original = contacts[index];

            var patched = (Contact)original.Clone();
            userProperties.ApplyTo(patched, ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var model = new
            {
                original = original,
                patched = patched
            };

            return Ok(model);
        }
    }
}