namespace bank_identification_code.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using bank_identification_code.Core.Interfaces;
    using bank_identification_code.Core.Models;
    using bank_identification_code.Core.Utility;
    using bank_identification_code.Mapping;
    using bank_identification_code.Persistence;
    using Microsoft.Extensions.Options;
    public class DBInitializer
    {
        private readonly IMapper mapper;
        private readonly IReader reader;
        private readonly IListEncoder converter;
        private readonly BNKSEEKDbContext context;
        private readonly AppSettings appSettings;
        private readonly string pznPath;
        private readonly string regPath;
        private readonly string tnpPath;
        private readonly string uerPath;

        public DBInitializer(
                IMapper _mapper,
                IReader _reader,
                IListEncoder _listEncoder,
                IOptionsSnapshot<AppSettings> _options,
                BNKSEEKDbContext _context)
        {
            this.mapper = _mapper;
            this.reader = _reader;
            this.converter = _listEncoder;
            this.context = _context;
            this.appSettings = _options.Value;

            this.pznPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "PZN.DBF");
            this.regPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "REG.DBF");
            this.tnpPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "TNP.DBF");
            this.uerPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "UER.DBF");
        }

        public async Task Seed()
        {
            try
            {
                if(!context.PZNRecords.Any())
                {
                    // TODO: PZN
                    var pznRecords = await GetDataAsync<PZNEntity>(this.pznPath, new MapperConfiguration(cfg => { cfg.AddProfile(new DataTableToPZNProfile()); }));

                    // NOTE: It didn't work for me!
                    // Update entities with AutoMapper in DB Context REF: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
                    // var pznUpdated = mapper.Map<ICollection<PZNEntity>, ICollection<PZNEntity>>(pznRecords, context.PZNRecords.ToList());

                    await context.PZNRecords.AddRangeAsync(pznRecords);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log DataBaseInitializer exception
                throw; // An error occurred seeding the PZN
            }

            try
            {
                if (!context.REGRecords.Any())
                {
                    var regRecords = await this.GetDataAsync<REGEntity>(this.regPath, new MapperConfiguration(cfg => { cfg.AddProfile(new DataTableToREGProfile()); }));

                    // NOTE: It didn't work for me!
                    // Update entities with AutoMapper in DB Context REF: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
                    // var regUpdated = mapper.Map(regRecords, context.REGRecords);

                    await context.REGRecords.AddRangeAsync(regRecords);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log DataBaseInitializer exception
                throw; // An error occurred seeding the PZN
            }

            try
            {
                if (!context.TNPRecords.Any())
                {
                    var tnpRecords = await this.GetDataAsync<TNPEntity>(this.tnpPath, new MapperConfiguration(cfg => { cfg.AddProfile(new DataTableToTNPProfile()); }));

                    // NOTE: It didn't work for me!
                    // Update entities with AutoMapper in DB Context REF: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
                    // var tnpUpdated = mapper.Map(tnpRecords, context.TNPRecords);

                    await context.TNPRecords.AddRangeAsync(tnpRecords);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log DataBaseInitializer exception
                throw; // An error occurred seeding the PZN
            }

            try
            {
                if (!context.UERRecords.Any())
                {
                    var uerRecords = await this.GetDataAsync<UEREntity>(this.uerPath, new MapperConfiguration(cfg => { cfg.AddProfile(new DataTableToUERProfile()); }));

                    // NOTE: It didn't work for me!
                    // Update entities with AutoMapper in DB Context REF: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
                    // var uerUpdated = mapper.Map(uerRecords, context.UERRecords);

                    await context.AddRangeAsync(uerRecords);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log DataBaseInitializer exception
                throw; // An error occurred seeding the PZN
            }
        }

        private string GetParent(string path)
        {
            return System.IO.Directory.GetParent(path).FullName;
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
                var records = dTableMapper.Map<List<DataRow>, List<T>>(table.AsEnumerable().ToL‌​ist());

                // // Encoding Algorithm
                var isNeedDecoding = appSettings.isNeedDecoding;

                // If true in app sittings
                if (isNeedDecoding)
                {
                    // Mapping Encoding
                    ((IEncoderBase)converter).BaseEncoding = appSettings.BaseEncoding;
                    ((IEncoderBase)converter).DestEncoding = appSettings.DestEncoding;
                    // Change records Encoding
                    return converter.Convert<T>(records);
                }

                return records;
            });
        }
    }
}