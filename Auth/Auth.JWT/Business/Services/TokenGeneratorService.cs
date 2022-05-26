using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.JWT.Business.Services
{
	public class TokenGeneratorService
	{
		private readonly string _userName;
		private readonly string _password;
			
        public TokenGeneratorService(string userName, string password)
		{
			_userName = userName;
			_password = password;
		}

		public string Generate()
        {
			var encrypted = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes("My-complicated-key!23456789")),
				SecurityAlgorithms.HmacSha256);
			var header = new JwtHeader(encrypted);
			var claims = new Claim[]
			{
				new Claim(ClaimTypes.Name, _userName), // Name for level accesss or credentials
				new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()), // Beginning of level accesss or credentials
				new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()), // Experation of level accesss or credentials
			};
			var payload = new JwtPayload(claims);
			var token = new JwtSecurityToken(header, payload);

			return new JwtSecurityTokenHandler().WriteToken(token);
        }


		public bool IsValidUser() => !string.IsNullOrWhiteSpace(_userName) && !string.IsNullOrWhiteSpace(_password);
	}
}

