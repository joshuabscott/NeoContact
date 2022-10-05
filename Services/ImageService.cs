using NeoContact.Services.Interfaces;

namespace NeoContact.Services
{
    //MODIFY
    public class ImageService : IImageService
    {
        //ADD
        private readonly string[] suffixes = {"Bytes", "KB", "MB", "GB", "TB", "PB"};
        private readonly string defaultImage = "img/DefaultContactImage.png";

        public string ConvertByteArrayToFile(byte[] fileData, string extention)
        {
            //ADD
            if (fileData == null) return defaultImage;
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extention};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            //ADD
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                return byteFile;
            }
            catch (Exception) 
            {
                throw;
            }
        }
    }
}



