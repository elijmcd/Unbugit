﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Services.Interfaces
{
    public interface IBTFileService
    {
        public string ContentType(IFormFile file);
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
        public Task<byte[]> ConvertFileToByteArrayAsync(string fileName);

        public string ConvertByteArrayToFile(byte[] fileData, string extension);

        public string GetFileIcon(string file);

        public string FormatFileSize(long bytes);
    }
}
