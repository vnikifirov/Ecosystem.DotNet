namespace bank_identification_code.Core.Interfaces
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.IO;
    using NDbfReader;
    using System.Text;

    public interface IReader
    {
        DataTable ReadDTFrom(string fileName);
    }

    public class DBFFileReader : IReader
    {
        public DataTable ReadDTFrom(string fileName)
        {
            var tableName = Path.GetFileNameWithoutExtension(fileName);
            var datatable = new DataTable(tableName);

            // Attempting to read data from BNKSEEK.DBF file
            try
            {
                // TODO: Ovewwrite using concurrency way
                using (var table = Table.Open(fileName))
                {
                    var reader = table.OpenReader(Encoding.GetEncoding(866));

                    // Mapping - Create a table columns and defining a data type
                    foreach (var col in reader.Table.Columns)
                    {
                        var type = col.Type;

                        // Check type if DateTime or String
                        if (type == typeof(string))
                        {
                            datatable.Columns.Add(col.Name, typeof(string));
                        }
                        else
                        {
                            datatable.Columns.Add(col.Name, typeof(DateTime));
                        }
                    }

                    // Reading records
                    while (reader.Read())
                    {
                        // Create a blank row
                        var row = datatable.NewRow();

                        foreach (var col in reader.Table.Columns)
                        {
                            var type = col.Type;

                            // Check type if DateTime or String
                            if (type == typeof(string))
                            {
                                row[col.Name] = reader.GetString(col.Name);
                            }
                            else
                            {
                                row[col.Name] = reader.GetDateTime(col.Name) ?? DateTime.MinValue;
                            }
                        }

                        // Add a new row
                        datatable.Rows.Add(row);
                    }
                }
            }
            catch (Exception)
            {
                // Handling exception...
                // TODO: log reader exception
                throw;
            }

            return datatable;
        }
    }
}