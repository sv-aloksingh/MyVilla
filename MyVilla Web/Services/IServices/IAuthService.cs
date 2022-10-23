using MyVilla_Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO model);
        Task<T> RegisterAsync<T>(RegistrationRequestDTO model);
    }
}
