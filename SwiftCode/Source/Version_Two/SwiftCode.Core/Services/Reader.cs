namespace SwiftCode.Core.Services
{
    using System;
    using System.IO;
    using NDbfReader;
    using AutoMapper;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Interfaces.Models.Common;

    public sealed class Reader : IReader
    {
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public Reader(IMapper mapper)
        {
            _mapper = mapper;
        }

        //public Reader(IMapper mapper, ILogger logger) 
        //{
        //    _mapper = mapper;
        //    _logger = logger;
        //}

        public async Task<IEnumerable<T>> ReadAsync<T>(string fileName, Encoding encoding = null) 
            where T : BaseModel
        {
            //_logger.LogInformation($"Starting reading data from { fileName }");
            // ? UTF-8 is the default encoding for the reader
            encoding = encoding ?? Encoding.UTF8;

            // ? Make checks
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var ex = new ArgumentNullException($"File name is empty {fileName}");
                //_logger.LogError($"Reading file error { fileName }", ex);
            }

            if (!File.Exists(fileName))
            {
                var ex = new ArgumentNullException($"File doesn't exist {fileName}");
                //_logger.LogError($"Reading file error { fileName }", ex);
            }

            var tableName = Path.GetFileNameWithoutExtension(fileName);

            // ? Attempting to read data from BNKSEEK.DBF file
            DataTable dtData = null;
            try
            {
                // ? Read records from file
                using (var table = await Table.OpenAsync(fileName))
                {
                    dtData = await table.AsDataTableAsync(encoding);
                }
                // ? Set a table name which's bassed on File Name w/o extention
                dtData.TableName = tableName;
            }
            catch (Exception)
            {                
                //_logger.LogError($"Error occured while trying to read a file { fileName }", ex);
            }

            return _mapper.Map<List<DataRow>, List<T>>(dtData.AsEnumerable().ToList());            
        }
    }
}
