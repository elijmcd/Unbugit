using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Services.Interfaces;

namespace Unbugit.Services
{
    public class BTFileService : IBTFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public string ContentType(IFormFile file)
        {
            return file?.ContentType.Split('/')[1];
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            if (fileData is null || extension is null) return null;

            string imageBase64Data = Convert.ToBase64String(fileData);
            return string.Format($"data:file/{extension};base64,{imageBase64Data}");
        }//

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            if (file is null) return null;

            MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var byteFile = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();

            return byteFile;
        }//
        public async Task<byte[]> ConvertFileToByteArrayAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/img/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }

        public string GetFileIcon(string file)
        {
            string ext = Path.GetExtension(file).Replace(".", "");
            return $"/img/png/{ext}.png";
        }//
    }
}
