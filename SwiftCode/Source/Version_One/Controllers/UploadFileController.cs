namespace bank_identification_code.Controllers
{
    using System;
    using System.IO;
    using AutoMapper;
    using System.Linq;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Hosting;
    using bank_identification_code.Core;
    using bank_identification_code.Mapping;
    using bank_identification_code.Core.Models;
    using bank_identification_code.Core.Utility;
    using bank_identification_code.Core.Interfaces;
    using bank_identification_code.Persistence.Repositories;
    using bank_identification_code.Persistence;

    [Route("api/upload-file")]
    public class UploadFileController : Controller
    {
        private readonly IMapper mapper;
        private readonly IReader reader;
        private readonly IListEncoder converter;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHostingEnvironment host;
        private readonly IRepository<BNKSEEKEntity> repository;
        private readonly AppSettings appSettings;

        public UploadFileController(
            IRepository<BNKSEEKEntity> repository,
            IMapper mapper,
            IReader reader,
            IListEncoder listEncoder,
            IUnitOfWork unitOfWork,
            IHostingEnvironment host,
            IOptionsSnapshot<AppSettings> options)
        {
            this.repository = repository;
            this.appSettings = options.Value;
            this.converter = listEncoder;
            this.host = host;
            this.reader = reader;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file) // I should use other of uploading files instead
        {
            // e.g wwwroot/uploads (will be created if it didn't exist)
            var uploadPath = Path.Combine(host.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Checks
            if (file == null) return BadRequest("Null File");
            if (file.Length <= 0) return BadRequest("Empty File");
            if (!FileValidator.IsSupported(appSettings.AcceptedFileTypes, file.FileName)) return BadRequest("Invalid File Type");

            //Generate a new file name in orde r to protect from hackers, e.g
            // var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fileName = Path.GetFileName(file.FileName);
            // Generate an upload file path
            var filePath = Path.Combine(uploadPath, fileName);

            // Store file to file system
            // TODO: log Error Cannot save file ...
            using(var stream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            // Get Data from file
            // TODO: log Error Invalid File or Data Corrupted
            ICollection<BNKSEEKEntity> uploaded = await GetDataAsync<BNKSEEKEntity>(
                filePath,
                // Mapper Configuration
                new MapperConfiguration(cfg => { cfg.AddProfile(new DataTableToBNKSEEKProfile()); }
            ));

            if (uploaded.Any())
            {
                // NOTE: Didn't work for me!
                // Update entities with AutoMapper in DB Context REF: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
                // var updated = mapper.Map(bnkSeekRecords, context.BNKSEEKRecords);

                var source = await repository.GetAllAsync(true);
                var updated = mapper.Map<ICollection<BNKSEEKEntity>, ICollection<BNKSEEKEntity>>(
                    source: uploaded,
                    destination: source
                );

                // Trying to Add records to DataBase
                try
                {
                    repository.AddAsync(uploaded);
                    await unitOfWork.CompleteAsync();
                }
                catch (Exception)
                {
                    // Handling exception...
                    // TODO: log DataBase save exception
                    throw;
                }
            }

            // Send Http status code 200
            return Ok();
        }

        private Task<ICollection<T>> GetDataAsync<T>(string filePath, MapperConfiguration config)
            where T : class
        {
            return Task.Run(() =>
            {
                // Read Data From file
                var table = reader.ReadDTFrom(filePath);
                // Mapper config for DataTable Profile
                var dTableMapper = config.CreateMapper();
                // Mapping DataTable to BNKSEEK Entity
                // TODO: log Error mapping DT to POCO/Model object
                var records = dTableMapper.Map<List<DataRow>, List<T>>(table.AsEnumerable().ToList());

                // // Encoding Algorithm
                var isNeedDecoding = appSettings.isNeedDecoding;

                // If true in app sittings
                if (isNeedDecoding)
                {
                    // Mapping Encoding
                    ((IEncoderBase)converter).BaseEncoding = appSettings.BaseEncoding;
                    ((IEncoderBase)converter).DestEncoding = appSettings.DestEncoding;
                    // Change records Encoding
                    // TODO: log Error encoding DT to POCO/Model object
                    return converter.Convert<T>(records);
                }

                return records;
            });
        }
    }
}
