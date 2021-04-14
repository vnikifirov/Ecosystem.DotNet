using System.Threading.Tasks;
using AutoMapper;
using ConfAutoMapper.Controllers.Resources;
using ConfAutoMapper.Mapping;
using ConfAutoMapper.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfAutoMapper.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;

        private User entity = new User
        {
            Id = 0,
            FirstName = "First Name Test",
            LastName = "Last Name Test",
            Location = new Location
            {
                Id = 0,
                City = "TestTown",
                Country = "Test"
            },
            isPrivate = true
        };

        public UsersController()
        {
            var config =   new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
        }

        [HttpGet]
        public UserDTO Index()
        {
            var resource = _mapper.Map<User, UserDTO>(entity);
            return resource;
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UserDTO recieved)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (recieved == null)
            {
                return BadRequest();
            }

            //var source = await _unitOfWork.Users.GetByIdAsync(id);
            var source = _mapper.Map(
                source:recieved,
                destination:entity
            );

            // Update state
            //repository.Update(changed);
                // Here's an example how to implement a method update in the Repsitory
                // --> _context.Entry(changed).State = EntityState.Modified;

            // Save changes
            // await unitOfWork.CompleteAsync();

            return Ok(recieved);
        }
    }
}