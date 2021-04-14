using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SwiftCode.Core.Interfaces.Repositories;
using SwiftCode.Core.Interfaces.Services;
using SwiftCode.Core.Models.Common;
using SwiftCode.Core.Persistence.Entities;
using SwiftCode.Core.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwiftCode.Web.Controllers
{
    [Route("api/records/[controller]")]
    public class PznController : Controller
    {
        private readonly IDecoder _decoder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PznController(IDecoder decoder, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _decoder = decoder;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<PznDTO>> Index()
        {
            var entitis = await _unitOfWork.PZN.GetAllAsync();
            var resources = _mapper.Map<IEnumerable<PznEntity>, IEnumerable<PznDTO>>(entitis);
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            resources = TrimStrings.TrimProps(resources);
            return resources;
        }

        [HttpGet("{VKEY}")]
        public async Task<IActionResult> GetById(string VKEY)
        {
            var decodedVKEY = _decoder.FromBase64.Decode(VKEY);
            var entity = await _unitOfWork.PZN.GetByVKEYAsync(decodedVKEY, true);
            if (entity == null) return NotFound();

            var resource = _mapper.Map<PznEntity, PznDTO>(entity);
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            resource = TrimStrings.TrimProps<PznDTO>(resource);

            return Ok(resource);
        }
    }
}
