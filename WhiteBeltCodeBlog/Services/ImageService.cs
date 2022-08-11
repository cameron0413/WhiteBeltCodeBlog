using WhiteBeltCodeBlog.Services.Interfaces;

namespace WhiteBeltCodeBlog.Services
{
    public class ImageService : IImageService
    {

        public string? ConvertByteArrayToFile(byte[]? fileData, string? extension)
        {

            try
            {
                string? imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64, {imageBase64Data}");
                //^^^^^ Interpolated code
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                return byteArray;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

