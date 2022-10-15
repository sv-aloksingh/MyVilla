using MyVilla_Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaNumberCreateDTO villaNumberCreateDTO);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDTO villaNumberUpdateDTO);
        Task<T> DeleteAsync<T>(int id);
    }
}
