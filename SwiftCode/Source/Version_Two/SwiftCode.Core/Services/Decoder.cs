
namespace SwiftCode.Core.Services
{
    using Microsoft.Extensions.Options;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Models.Common;
    using SwiftCode.Core.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Decoder : IDecoder
    {
        //private readonly ILogger _logger;
        public Encodings Encodings { get; private set; }
        //protected readonly AppSettings _appSettings;

        public Decoder(IOptionsSnapshot<AppSettings> options)
        {
            var appSettings = options.Value;
            Encodings = new Encodings(appSettings.FromEncoding, appSettings.ToEncoding);
            StringDecoder = new StringDecoder(Encodings);
            EnumerableDecoder = new EnumerableDecoder(StringDecoder);
            FromBase64 = new FromBase64Decoder();
        }

        //public Decoder(ILogger logger, AppSettings appSettings)
        //{
        //    // _appSettings = appSettings;
        //    Encodings = new Encodings(appSettings.FromEncoding, appSettings.ToEncoding);
        //    StringDecoder = new StringDecoder(logger, Encodings);
        //    EnumerableDecoder = new EnumerableDecoder(logger, StringDecoder);
        //    FromBase64 = new FromBase64Decoder();
        //    _logger = logger;
        //}

        public IStringDecoder StringDecoder { get; private set; }

        public IStringDecoder FromBase64 { get; private set; }

        public IEnumerableDecoder EnumerableDecoder { get; private set; }
    }

    public sealed class FromBase64Decoder : IStringDecoder
    {
        Encodings IStringDecoder.Encodings => null;

        private object locked = new object();

        public string Decode(string source)
        {
            lock (locked)
            {
                byte[] decodedBytes = Convert.FromBase64String(source);
                return Encoding.UTF8.GetString(decodedBytes);
            }
        }
    }

    public sealed class StringDecoder : IStringDecoder
    {
        //private readonly ILogger _logger;
        public Encodings Encodings { get; private set; }

        private object strDecodeLocked = new object();

        public StringDecoder(Encodings encodings)
        {            
            Encodings = encodings;
        }

        //public StringDecoder(ILogger logger, Encodings encodings)
        //{
        //    _logger = logger;
        //    Encodings = encodings;
        //}

        public string Decode(string source)
        {           
            if (Monitor.TryEnter(strDecodeLocked))
            {
                try
                {
                    var decoded = string.Empty;
                    //_logger.LogInformation($"Starting decoding source - { source }");

                    var from = Encoding.GetEncoding(this.Encodings.FromEncoding);
                    var to = Encoding.GetEncoding(this.Encodings.ToEncoding);

                    // ? Convert Source Data to bytes array
                    var bytes = from.GetBytes(source);
                    // ? Encoded data
                    decoded = to.GetString(Encoding.Convert(from, to, bytes));

                    //_logger.LogInformation($"Decoding completed - { source }");
                    return decoded;
                }
                catch (Exception)
                {
                    //_logger.LogError($"Error occurred while attempting to decode source - { source }", ex);
                }
                finally
                {
                    Monitor.Exit(strDecodeLocked);
                }
            }
            
            return null;
        }
    }

    public sealed class EnumerableDecoder : IEnumerableDecoder
    {
        //private readonly ILogger _logger;
        public IStringDecoder Decoder { get; private set; }

        private object listEncodeLocked = new object();

        public EnumerableDecoder(IStringDecoder stringDecoder)
        {
            Decoder = stringDecoder;
        }

        //public EnumerableDecoder(ILogger logger, IStringDecoder stringDecoder)
        //{
        //    _logger = logger;
        //    Decoder = stringDecoder;
        //}

        public IEnumerable<T> Decode<T>(IEnumerable<T> items)  where T : class
        {
            if (Monitor.TryEnter(listEncodeLocked))
            {
                try
                {
                    //_logger.LogInformation($"Starting decoding items - {items.GetType().FullName}");
                    // ? Parallelize converting encoding of records

                    //int maxDegre = (Environment.ProcessorCount > 4) ? Environment.ProcessorCount / 2 * 10 : Environment.ProcessorCount * 10;
                    Parallel.ForEach(items,
                        //new ParallelOptions
                        //{
                        //    MaxDegreeOfParallelism = maxDegre
                        //}, 
                        // The delegate that is invoked once per iteration.
                        (item) =>
                        {
                            // ? Using reflection to loop through properties
                            var properties = item.GetType().GetProperties();
                            foreach (var property in properties)
                            {
                                // ? Get a property
                                object propValue = property.GetValue(item, null);

                                // ? Whenever a property is null, skipe it
                                if (propValue == null) continue;

                                // ? If property is a string, trying to encode
                                var stringProp = propValue as string;
                                if (string.IsNullOrWhiteSpace(stringProp)) continue;

                                // ? Trying to decode cp866 to cp1251 encoding
                                string encoded = Decoder.Decode(stringProp);
                                property.SetValue(item, encoded);
                            }
                        });

                    return items;
                }
                catch (Exception)
                {
                    //_logger.LogError($"Error occurred while attempting to cecode items - {items.GetType().FullName}", ex);
                }
                finally
                {
                    Monitor.Exit(listEncodeLocked);
                }
            }

            return null;
        }
    }
}
