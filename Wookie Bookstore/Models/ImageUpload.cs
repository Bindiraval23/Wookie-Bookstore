using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Wookie_Bookstore.Models
{
    public static class ImageUpload
    {
        public static string WriteFile(string OriginalFileName, byte[] Image)
        {
            string FileName;

            var Extension = "." + OriginalFileName.Split('.')[OriginalFileName.Split('.').Length - 1];
            FileName = Guid.NewGuid().ToString() + Extension; //Create a new Name 


            var Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "CoverPhotos", FileName);
            File.WriteAllBytes(Path, Image);

            return FileName;
        }

        public static void DeleteFile(string FileName)
        {
            var FilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "CoverPhotos", FileName);

            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

    }
}
