namespace bank_identification_code.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using bank_identification_code.Core.Models;

    public interface IEncoderBase
    {
        string BaseEncoding { get; set; }
        string DestEncoding { get; set; }
    }
    public interface IEncoder
    {
        string Convert(string source);
    }

    public interface IListEncoder
    {
        ICollection<T> Convert<T>(ICollection<T> bnkSeekrecords) where T : class;
    }

    public class FromBase64Converter : IEncoder
    {
        // Convert from Base64String
        public string Convert(string source)
        {
            byte[] decodedBytes = System.Convert.FromBase64String(source);
            return System.Text.Encoding.UTF8.GetString(decodedBytes);
        }
    }

    public sealed class ListEncoder : IEncoder, IEncoderBase, IListEncoder
    {
        public string BaseEncoding { get; set; }
        public string DestEncoding { get; set; }

        public ICollection<T> Convert<T>(ICollection<T> records) where T : class
        {
            try
            {
                foreach (var record in records)
                {
                    // Using reflection to loop through properties of an object and encoding them
                    PropertyInfo[] properties = record.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        // If property is a string that try to encoding that
                        if (property.PropertyType == typeof(string) && property.GetValue(record) != null)
                        {
                            // Decoding cp866 to cp1251 encoding
                            string encoded = Convert(property.GetValue(record).ToString());
                            property.SetValue(record, encoded);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log Decoder exception
                throw;
            }

            return records;
        }

        public string Convert(string source)
        {
            try
            {
                var sourceEncoding = Encoding.GetEncoding(BaseEncoding);
                var destEncoding = Encoding.GetEncoding(DestEncoding);

                // Convert Source Data to bytes array
                var bytes = sourceEncoding.GetBytes(source);

                // Encoded data
                return destEncoding.GetString(Encoding.Convert(sourceEncoding, destEncoding, bytes));
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log Decoder exception
                throw;
            }
        }
    }
}