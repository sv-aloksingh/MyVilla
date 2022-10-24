using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyVilla_WebAPI.Models;
using MyVilla_WebAPI.Models.Dto;
using MyVilla_WebAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Controllers.v2
{
    //{version: apiVersion}
    [Route("api/v2/VillaNumberAPI")]
    [ApiVersion("2.0")] //, Deprecated = true
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<VillaNumberAPIController> _logger;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        protected APIResponse _response;

        public VillaNumberAPIController(IMapper mapper,
            ILogger<VillaNumberAPIController> logger,
            IVillaNumberRepository villaNumberRepository,
            IVillaRepository villaRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _villaNumberRepository = villaNumberRepository;
            _villaRepository = villaRepository;
            this._response = new APIResponse();
        }

        [HttpGet("GetString")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Mata Rani", "Venkateshwara", "Lakshmi Ji"};
        }
    }
}
