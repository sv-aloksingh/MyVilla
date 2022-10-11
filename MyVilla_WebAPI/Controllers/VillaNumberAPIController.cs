using AutoMapper;
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

namespace MyVilla_WebAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllVillaNumber()
        {
            try
            {
                var villaNo = await _villaNumberRepository.GetAllVillaAsync();
                if (villaNo == null)
                    return NoContent();
                var response = _mapper.Map<IEnumerable<VillaNumberDTO>>(villaNo);
                _response.Result = response;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result =
                    new List<string> { ex.ToString()};
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }
                var villa = await _villaNumberRepository.GetAllVillaAsync(x => x.VillaNo == id);
                if (villa.Count == 0)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }
                else
                {
                    var response = _mapper.Map<VillaNumberDTO>(villa.FirstOrDefault());
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = response;
                    return _response;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result =
                    new List<string> { ex.ToString()};
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody]VillaNumberCreateDTO villaNumberCreateDTO)
        {
            try
            {
                if (villaNumberCreateDTO == null)
                    return BadRequest();
                var exist = await _villaNumberRepository.GetAllVillaAsync();
                var eleExist = await _villaNumberRepository.GetAllVillaAsync(x => x.VillaNo == villaNumberCreateDTO.VillaNo);
                if (exist.Any() && eleExist.Any())
                {
                    ModelState.AddModelError("CustomError", "Villa number already exist.");
                    return BadRequest(ModelState);
                }
                if ((await _villaRepository.GetAllVillaAsync(x=>x.Id == villaNumberCreateDTO.VillaID)) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id is invalid.");
                    return BadRequest(ModelState);
                }

                var villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);
                await _villaNumberRepository.CreateVillaAsync(villaNumber);
                _response.Result = villaNumber;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo}, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result =
                    new List<string> { ex.ToString()};
            }
            return _response;
        }


        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int Id,[FromBody]VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            try
            {
                if (villaNumberUpdateDTO == null || villaNumberUpdateDTO.VillaNo <= 0)
                    return BadRequest();
                if ((await _villaRepository.GetAllVillaAsync(x => x.Id == villaNumberUpdateDTO.VillaID)) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id is invalid.");
                    return BadRequest(ModelState);
                }
                var response = _mapper.Map<VillaNumber>(villaNumberUpdateDTO);
                await _villaNumberRepository.UpdateAsync(response);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage =
                    new List<string>() { ex.ToString()};
            }
            return _response;
        }

        [HttpDelete("{id}",Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();
                var villaNo = await _villaNumberRepository.GetAllVillaAsync(x => x.VillaNo == id);
                if (villaNo == null) 
                    return NotFound();
                
                await _villaNumberRepository.RemoveVillaAsync(villaNo.FirstOrDefault());
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
