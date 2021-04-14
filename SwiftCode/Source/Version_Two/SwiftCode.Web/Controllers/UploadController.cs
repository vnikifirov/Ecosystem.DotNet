namespace SwiftCode.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Persistence.Entities;
    using System.Threading.Tasks;

    [Route("api/records/[controller]")]
    public class UploadController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IDecoder _decoder;

        public UploadController(
            IMapper mapper,
            IFileService fileService,
            IDecoder decoder)
        {
            _mapper = mapper;
            _fileService = fileService;
            _decoder = decoder;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            // TODO: log Error Cannot save file ...
            string filePath = await _fileService.SaveFileAsync(file);

            // ? Fetch Data from file
            // TODO: log Error exception unable to upload file
            var data = await _fileService.FetchDataAsync<BnkseekEntity>(filePath);

            // ? Change Data Encoding
            data = _decoder.EnumerableDecoder.Decode(data);

            // TODO: log DataBase save operation exception
            await _fileService.SaveAsync(data);

            // ? Send Http status code 200
            return Ok();
        }
    }
}
