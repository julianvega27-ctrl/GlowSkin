using Microsoft.AspNetCore.Http;

namespace AlaiaStore.Web.Services;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile imageFile, string folderName);
}
