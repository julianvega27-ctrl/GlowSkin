using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AlaiaStore.Web.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveImageAsync(IFormFile imageFile, string folderName)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return string.Empty;
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        // Devolvemos la ruta relativa para la base de datos, usando slash normal para URLs
        return $"/images/{folderName}/{uniqueFileName}";
    }
}
