using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
namespace Demo.PL.Helpers
{
    public static class DocumentSettings
    {
        public static async Task<string> UploadFile(IFormFile file, string folderName)
        {
            //1. Get Located File Path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);

            // 2. Get File Name and Make It Unique\

            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            // 3. Get File Path = folderPath + fileName
            string filePath = Path.Combine(folderPath, fileName);

            // 4. Save file as Stream [Data Per Time]
            using var fs = new FileStream(filePath, FileMode.Create); // this is what will be stored in the server

            await file.CopyToAsync(fs); // 

            return  fileName; // This is what will be stored in the db
        }



        public static void DeleteFile(string fileName, string folderName)
        {

         var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\files",folderName, fileName);

            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}