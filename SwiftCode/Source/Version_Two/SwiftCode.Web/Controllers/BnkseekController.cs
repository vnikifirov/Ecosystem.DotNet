using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SwiftCode.Core.Interfaces.Repositories;
using SwiftCode.Core.Interfaces.Services;
using SwiftCode.Core.Models.Common;
using SwiftCode.Core.Models.Request;
using SwiftCode.Core.Models.Response;
using SwiftCode.Core.Persistence.Entities;
using SwiftCode.Core.Utilities;
using System;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwiftCode.Web.Controllers
{
    [Route("api/records")]
    public class BnkseekController : Controller
    {
        private readonly IDecoder _decoder;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BnkseekController(IDecoder decoder, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _decoder = decoder;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<QueryResult<BnkseekDTO>> Index(QueryOject queryOject)
        {
            var queryResult = await _unitOfWork.Bnkseek.GetByQueryAsync(queryOject, true);
            if (queryResult == null) return null;

            var resorces = _mapper.Map<QueryResult<BnkseekEntity>, QueryResult<BnkseekDTO>>(queryResult);

            // ? Trim trailing spaces from char fields
            // ? More info: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            resorces.Items = TrimStrings.TrimProps(resorces.Items);
            return resorces;
        }

        [HttpGet("{VKEY}")]
        public async Task<IActionResult> Details(string VKEY)
        {
            var decodedVKEY = _decoder.FromBase64.Decode(VKEY);
            var entity = await _unitOfWork.Bnkseek.GetByVKEYAsync(decodedVKEY, true);

            if (entity == null) return NotFound();

            var resource = _mapper.Map<BnkseekEntity, SaveBnkseekDTO>(entity);

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            resource = TrimStrings.TrimProps(resource);
            return Ok(resource);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBnkseekDTO resource)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (resource == null) return BadRequest();

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            resource = TrimStrings.TrimProps(resource);

            var created = _mapper.Map<CreateBnkseekDTO, BnkseekEntity>(resource);
            created.DT_IZM = DateTime.Today;

            // ? Save changes to the DB
            _unitOfWork.Bnkseek.AddAsync(created);
            await _unitOfWork.CompleteAsync();

            return Ok(resource);
        }

        [HttpPut("{vkey}")]
        public async Task<IActionResult> Update(string vkey, [FromBody] SaveBnkseekDTO resource)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (resource == null) return BadRequest();

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            resource = TrimStrings.TrimProps(resource);

            var decodedVKEY = _decoder.FromBase64.Decode(vkey);
            var source = await _unitOfWork.Bnkseek.GetByVKEYAsync(decodedVKEY, true);
            var updated = _mapper.Map(
                source: resource,
                destination: source
            );
            updated.DT_IZM = DateTime.Today;

            // ? Save changes to the DB
            _unitOfWork.Bnkseek.Update(updated);
            await _unitOfWork.CompleteAsync();

            return Ok(resource);
        }

        [HttpDelete("{VKEY}")]
        public async Task<IActionResult> Delete(string VKEY)
        {
            // ? Decoded vkey from Base64 string
            var decodedVKEY = _decoder.FromBase64.Decode(VKEY);
            var entity = await _unitOfWork.Bnkseek.GetByVKEYAsync(decodedVKEY);

            if (entity == null) return NotFound();

            _unitOfWork.Bnkseek.Remove(entity);
            // ? Remove record from DB
            await _unitOfWork.CompleteAsync();

            return Ok(VKEY);
        }
    }
}
