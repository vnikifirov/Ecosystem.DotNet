namespace SwiftCode.Core.Extentions
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using SwiftCode.Core.Utilities;

    public enum FileStatus
    {
        Valid,
        Null,
        Empty,
        InvalidType
    }

    public static class IFormFileExtentions
    {
        public static FileStatus IsValid(this IFormFile file, string[] acceptedFileTypes)
        {
            if (file == null)
            {
                return FileStatus.Null;
            }

            if (file.Length <= 0)
            {
                return FileStatus.Empty;
            }

            if (string.IsNullOrWhiteSpace(file.Name))
            {
                return FileStatus.Null;
            }

            if (!FileValidator.IsSupported(file.Name, acceptedFileTypes))
            {
                return FileStatus.InvalidType;
            }

            return FileStatus.Valid;
        }

        public async static Task<string> SaveFileAsync(this IFormFile file, string uploadPath)
        {
            //Generate a new file name in orde r to protect from hackers, e.g
            // var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fileName = Path.GetFileName(file.FileName);
            // Generate an upload file path
            var filePath = Path.Combine(uploadPath, fileName);

            // Store file to file system
            // TODO: log Error Cannot save file ...
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
