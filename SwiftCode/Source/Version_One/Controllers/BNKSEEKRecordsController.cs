
namespace bank_identification_code.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using bank_identification_code.Controllers.Resources;
    using bank_identification_code.Core;
    using bank_identification_code.Core.Interfaces;
    using bank_identification_code.Core.Models;
    using bank_identification_code.Core.Utility;
    using bank_identification_code.Persistence;
    using bank_identification_code.Persistence.Repositories;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/records")]
    public class BNKSEEKRecordsController : Controller
    {
        private readonly IEncoder converter;
        // TODO Refactor repositories in order to use UnitOfWork pattern a more effective way
        private readonly IRepository<BNKSEEKEntity> repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public BNKSEEKRecordsController(IRepository<BNKSEEKEntity> repository, IEncoder converter,  IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.converter = converter;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<BNKSEEKResource>> Index()
        {
            var records = await repository.GetAllAsync(true);
            if (records == null) return null;

            var apiResources = mapper.Map<List<BNKSEEKEntity>, List<BNKSEEKResource>>(records.Take(5).ToList());

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            apiResources = TrimStrings.TrimProps(apiResources).ToList();
            return apiResources;
        }

        [HttpGet("{VKEY}")]
        public async Task<IActionResult> Details(string VKEY)
        {
            var decodedVKEY = converter.Convert(VKEY);
            var entity = await repository.GetByVKEYAsync(decodedVKEY, true);
            if (entity == null)
            {
                return NotFound();
            }

            var apiResource = mapper.Map<BNKSEEKEntity, SaveBNKSEEKResource>(entity);

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            apiResource = TrimStrings.TrimProps<SaveBNKSEEKResource>(apiResource);
            return Ok(apiResource);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveBNKSEEKResource newRecord)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newRecord == null)
            {
                return BadRequest();
            }

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            newRecord = TrimStrings.TrimProps<SaveBNKSEEKResource>(newRecord);

            var created = mapper.Map<SaveBNKSEEKResource, BNKSEEKEntity>(newRecord);
            created.DT_IZM = DateTime.Today;

            // ? Save changes to the DB
            repository.AddAsync(created);
            await unitOfWork.CompleteAsync();

            return Ok(newRecord);
        }

        [HttpPut("{vkey}")]
        public async Task<IActionResult> Update(string vkey, [FromBody] SaveBNKSEEKResource record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (record == null)
            {
                return BadRequest();
            }

            // TODO Make a catch exception if apiResources is NULL
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            record = TrimStrings.TrimProps<SaveBNKSEEKResource>(record);

            var decodedVKEY = converter.Convert(vkey);
            var source = await repository.GetByVKEYAsync(decodedVKEY, true);
            var updated = mapper.Map<SaveBNKSEEKResource, BNKSEEKEntity>(
                source: record,
                destination: source
            );
            updated.DT_IZM = DateTime.Today;

            // ? Save changes to the DB
            repository.Update(updated);
            await unitOfWork.CompleteAsync();

            return Ok(record);
        }

        [HttpDelete("{VKEY}")]
        public async Task<IActionResult> Delete(string VKEY)
        {
            // ? Decoded vkey from Base64 string
            var decodedVKEY = converter.Convert(VKEY);
            var entity = await repository.GetByVKEYAsync(decodedVKEY);
            if (entity == null)
            {
                return NotFound();
            }

            repository.Remove(entity);
            // ? Remove record from DB
            await unitOfWork.CompleteAsync();

            return Ok(VKEY);
        }
    }
}