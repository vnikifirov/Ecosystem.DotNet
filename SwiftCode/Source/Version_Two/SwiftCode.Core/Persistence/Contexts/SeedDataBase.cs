namespace SwiftCode.Core.Persistence.Contexts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using SwiftCode.Core.Properties;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Persistence.Entities;

    public static class SeedDataBase
    {

        public static async void Seed(IFileService fileService, IDecoder decoder)
        {            
            try
            {
                using (var init = new Init(decoder, fileService)) await init.SeedAsync();
            }
            catch (Exception)
            {
                //_logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }

    internal class Init : IDisposable
    {
        //private ILogger _logger;
        private IFileService _fileService;
        private IDecoder _decoder;

        private readonly string root;

        private readonly string pznPath;
        private readonly string regPath;
        private readonly string tnpPath;
        private readonly string uerPath;

        public Init(IDecoder decoder, IFileService fileService)
        {
            _decoder = decoder;
            _fileService = fileService;

            root = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();

            // Paths
            pznPath = Path.Combine(root, Resources.pzn ?? string.Empty);
            regPath = Path.Combine(root, Resources.reg ?? string.Empty);
            tnpPath = Path.Combine(root, Resources.tnp ?? string.Empty);
            uerPath = Path.Combine(root, Resources.uer ?? string.Empty);
        }

        //public Init(ILogger logger, IDecoder decoder, IFileService fileService)
        //{
        //    _decoder = decoder;
        //    _logger = logger;
        //    _fileService = fileService;

        //    root = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();

        //    // Paths
        //    pznPath = Path.Combine(root, Resources.pzn ?? string.Empty);
        //    regPath = Path.Combine(root, Resources.reg ?? string.Empty);
        //    tnpPath = Path.Combine(root, Resources.tnp ?? string.Empty);
        //    uerPath = Path.Combine(root, Resources.uer ?? string.Empty);
        //}


        internal async Task SeedAsync()
        {
            // ? Initialize PZN
            try
            {
                // ? Fetch Data from files
                var pznTask = _fileService.FetchDataAsync<PznEntity>(pznPath);
                var regTask = _fileService.FetchDataAsync<RegEntity>(regPath);
                var tnpTask = _fileService.FetchDataAsync<TnpEntity>(tnpPath);
                var uerTask = _fileService.FetchDataAsync<UerEntity>(uerPath);                
                                
                await Task.WhenAll(pznTask, regTask, tnpTask, uerTask);

                // ? Change Data Encoding, it's parallel programming 
                var pznData = _decoder.EnumerableDecoder.Decode(pznTask.Result);
                var regData = _decoder.EnumerableDecoder.Decode(regTask.Result);
                var tnpData = _decoder.EnumerableDecoder.Decode(tnpTask.Result);
                var uerData = _decoder.EnumerableDecoder.Decode(uerTask.Result);

                // Save Data to DataBase
                var pznSaveTask = _fileService.SaveAsync(pznData);
                var regSaveTask = _fileService.SaveAsync(regData);
                var tnpSaveTask = _fileService.SaveAsync(tnpData);
                var uerSaveTask = _fileService.SaveAsync(uerData);

                await Task.WhenAll(pznSaveTask, regSaveTask, tnpSaveTask, uerSaveTask);
            }
            catch (Exception)
            {
                //_logger.LogError(ex, "An error occurred while initialization the database.");
            }      
        }

        private string GetParent(string path)
        {
            return Directory.GetParent(path).FullName;
        }

        public void Dispose()
        {
            //_fileService.Dispose();
        }
    }
}
