using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyVilla_Web.Models;
using MyVilla_Web.Models.Dto;
using MyVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;

        public HomeController(ILogger<HomeController> logger,
            IMapper mapper,
            IVillaService villaService)
        {
            _logger = logger;
            _mapper = mapper;
            _villaService = villaService;
        }

        public async Task<IActionResult> Index()
        {
            var villaList = new List<VillaDTO>();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            return View(villaList);
        }
    }
}
