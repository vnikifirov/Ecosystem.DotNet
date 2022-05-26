using System;
using Auth.JWT.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth.JWT.Controllers
{
    [ApiController]
    [Route("weather/login")]
    public class JWTController : ControllerBase
	{

        private readonly ILogger<JWTController> _logger;

        public JWTController(ILogger<JWTController> logger)
        {
            _logger = logger;
        }

        [Route("/token")]
        [HttpPost]
        public ActionResult<string> GetToken(string userName, string password)
        {
            var tokenService = new TokenGeneratorService(userName, password);
            return tokenService.IsValidUser() ? tokenService.Generate() : new UnauthorizedResult();
        }
    }
}

