using MyVilla_WebAPI.Models;
using MyVilla_WebAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string userName);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
