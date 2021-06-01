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
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            if (fileData is null || extension is null) return null;
            return $"data:file/{extension};base64,{Convert.ToBase64String(fileData)}";
        }//

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            if (file is null) return null;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }//

        public string FormatFileSize(long bytes)
        {
            throw new NotImplementedException();
        }

        public string GetFileIcon(string file)
        {
            throw new NotImplementedException();
        }//
    }
}
