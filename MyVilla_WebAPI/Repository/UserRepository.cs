using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyVilla_WebAPI.Data;
using MyVilla_WebAPI.Models;
using MyVilla_WebAPI.Models.Dto;
using MyVilla_WebAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UserRepository(ApplicationDbContext db,
            IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string userName)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
                return true;
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.LocalUsers.FirstOrDefault(
                x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
                && x.Password == loginRequestDTO.Password);
            if (user == null)
            {
                var response = new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
            //if user is found, generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { 
                    new Claim(ClaimTypes.Name, user.Id.ToString()), 
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };

            return loginResponseDTO;
        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            LocalUser user = new LocalUser()
            {
                UserName = registrationRequestDTO.UserName,
                Password = registrationRequestDTO.Password,
                Name = registrationRequestDTO.Name,
                Role = registrationRequestDTO.Role
            };

            _db.Add(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;
        }
    }
}
