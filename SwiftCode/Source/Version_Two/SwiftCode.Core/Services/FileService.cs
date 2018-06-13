
namespace SwiftCode.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using SwiftCode.Core.Extentions;
    using SwiftCode.Core.Interfaces.Models.Common;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Utilities;

    public sealed class FileService : IFileService
    {
        private IMapper Mapper;
        private IContextFactory _contextFactory;
        private IHostingEnvironment _host;
        private AppSettings _appSettings;
        private IReader _reader;
        //private volatile ILogger _logger;

        private readonly object locker = new object();

        public FileService(
            IMapper mapper,
            IReader reader,
            IContextFactory contextFactory,
            IHostingEnvironment host,
            IOptionsSnapshot<AppSettings> options)
        {
            _host = host;
            _reader = reader;
            Mapper = mapper;
            _contextFactory = contextFactory;
            _appSettings = options.Value;
        }

        //public FileService(
        //    IMapper mapper,
        //    IReader reader,
        //    IUnitOfWork unitOfWork,
        //    IHostingEnvironment host,
        //    ILogger logger,
        //    IOptionsSnapshot<AppSettings> options)
        //{
        //    _host = host;
        //    _reader = reader;
        //    _mapper = mapper;
        //    _unitOfWork = unitOfWork;
        //    _appSettings = options.Value;
        //    _logger = logger;
        //}

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            //_logger.LogInformation($"Store file with name { file.FileName } to file system");
            // ? Get destination folder name from app config

            var uploadFolder = _appSettings.UploadFolder;
            // ? Check out file folder isn't empty
            if (string.IsNullOrWhiteSpace(uploadFolder))
            {
                var ex = new DirectoryNotFoundException("Could not find a part of the path");
                //_logger.LogError("Error saving file", ex);
            }

            // ? e.g wwwroot/uploads (will be created if it didn't exist)
            var uploadPath = Path.Combine(_host.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // ? Check out file is valid
            var status = file.IsValid(_appSettings.AcceptedFileTypes);
            if (status != FileStatus.Valid)
            {
                var ex = new NotSupportedException($"File is not valid and has status {status.ToString()}");
                //_logger.LogError($"Error file is not valid", ex);
            }

            // ? Store file to file system
            //_logger.LogInformation($"Store file with name { file.FileName } to directory { uploadPath }");
            var filePath = await file.SaveFileAsync(uploadPath);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                var ex = new IOException($"Unable to upload file with name {file.FileName} to directory { uploadPath }");
                //_logger.LogError($"Error uploading file", ex);
            }

            //_logger.LogInformation($"File stored. File name { file.FileName }, Directory name { uploadPath }");
            return uploadPath;
        }

        public async Task<IEnumerable<T>> FetchDataAsync<T>(string filePath) where T : BaseModel            
        {
            //_logger.LogInformation($"Fetching data from { filePath }");
            // ? Get File Encoding from app config
            var baseEncoding = _appSettings.FromEncoding;
            if (string.IsNullOrWhiteSpace(baseEncoding))
            {
                var ex = new ArgumentNullException($"File encoding was not specified file name {filePath}");
                //_logger.LogError($"File encoding error", ex);
            }

            // ? Read Data From file
            IEnumerable<T> data = null;
            try
            {
                //_logger.LogInformation($"Reading data from { filePath }");
                data = await _reader.ReadAsync<T>(filePath, Encoding.GetEncoding(baseEncoding));
            }
            catch (Exception)
            {
                //_logger.LogError($"Unable to read file {filePath} { filePath }", ex);
            }

            // ? If nothing was downloaded then return null
            if (data == null)
            {
                //_logger.LogInformation($"Data was not found in { filePath }");
                return null;
            }

            // ? If fail to map records then return null
            if (data == null)
            {
                var ex = new InvalidOperationException(string.Format($"Unable to map records {data.ToString()}, file path {filePath}"));
                //_logger.LogError($"Mapping data error { filePath }", ex);
            }

            return data;
        }

        public async Task SaveAsync<T>(IEnumerable<T> data)
            where T : BaseModel
        {
            //_logger.LogInformation($"Saving new data to DataBase, DataType { data.GetType() }");

            // ? Creating a new DataBase Context
            var unitOfWork = _contextFactory.GetUnitOfWork();
            // ? Trying to get context of IRepository based on a given data type
            var context = _contextFactory.GetContextBasedOn<T>(unitOfWork);

            if (context == null)
            {
                var ex = new NotSupportedException($"Given type is not supported by ContextFactory. Type {typeof(T)}");
                //_logger.LogError("Saving to DB Error", ex);
                return; // ? Reject operation
            }

            // ? Get records from DB and converting to IEntityBase instances in order to make scanning by VKEY/Primary key
            var sourceBase = await context.GetAllAsync(includeReleted: true);
            //if (sourceBase == null)
            //{
            //    var ex = new NotSupportedException($"DataBase hasn't records according to given data type - {typeof(T)}");
            //    //_logger.LogError("Saving to DB Error", ex);
            //}

            // ? Filter out new records if they exsit
            if (sourceBase.Any()) data = data.AsParallel().Where(d => sourceBase.All(s => d.VKEY != s.VKEY)).ToList();
            // ? Trying to Add new records to DataBase
            try
            {
                // ? Converting back to TEntity in order to save a result to DB                
                context.AddAsync(data);
                await unitOfWork.CompleteAsync();

                //_logger.LogInformation($"The Data was stored in DB. Argument metadata { data.GetType() }");
            }
            catch (Exception)
            {
                //_logger.LogError($"Unable to save data to DataBase", ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
