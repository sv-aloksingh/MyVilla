﻿using Microsoft.Extensions.Configuration;
using MyVilla_Utility;
using MyVilla_Web.Models.Dto;
using MyVilla_Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyVilla_Web.Services
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private readonly IHttpClientFactory _httpClient;
        private string villaNumberUrl;

        public VillaNumberService(IHttpClientFactory httpClient,
            IConfiguration configuration)
            : base(httpClient)
        {
            _httpClient = httpClient;
            villaNumberUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateAsync<T>(VillaNumberCreateDTO villaNumberCreateDTO)
        {
            return SendAsync<T>(new Models.APIRequest() 
            { 
                ApiType = SD.ApiType.POST,
                Data = villaNumberCreateDTO,
                Url = villaNumberUrl + "/api/VillaNumberAPI"
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaNumberUrl + "/api/VillaNumberAPI/" + id
            }); 
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaNumberUrl + "/api/VillaNumberAPI"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaNumberUrl + "/api/VillaNumberAPI/" + id
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            return SendAsync<T>(new Models.APIRequest() 
            { 
                ApiType = SD.ApiType.PUT,
                Data = villaNumberUpdateDTO,
                Url = villaNumberUrl + "/api/VillaNumberAPI/" + villaNumberUpdateDTO.VillaNo
            });
        }
    }
}