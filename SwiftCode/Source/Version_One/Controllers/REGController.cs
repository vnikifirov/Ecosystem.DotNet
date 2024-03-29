﻿
namespace bank_identification_code.Controllers
{
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

    [Route("api/reg")]
    public class REGController : Controller
    {
        private readonly IEncoder converter;
        private readonly IRepository<REGEntity> repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public REGController(IRepository<REGEntity> repository, IEncoder converter,  IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.converter = converter;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<REGResource>> Index()
        {
            var entitis = await repository.GetAllAsync();
            var apiResources = mapper.Map<List<REGEntity>, List<REGResource>>(entitis);
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            apiResources = TrimStrings.TrimProps(apiResources).ToList();
            return apiResources;
        }

        [HttpGet("{VKEY}")]
        public async Task<IActionResult> GetById(string VKEY)
        {
            var decodedVKEY = converter.Convert(VKEY);
            var entity = await repository.GetByVKEYAsync(decodedVKEY, true);
            if (entity == null)
            {
                return NotFound();
            }

            var apiResource = mapper.Map<REGEntity, REGResource>(entity);
            // ? Trim trailing spaces from char fields
            // ? url: https://community.dynamics.com/gp/b/gpdynland/archive/2017/06/10/asp-net-core-and-ef-core-with-dynamics-gp-trim-trailing-spaces-from-char-fields
            apiResource = TrimStrings.TrimProps<REGResource>(apiResource);

            return Ok(mapper.Map<REGEntity, REGResource>(entity));
        }
    }
}