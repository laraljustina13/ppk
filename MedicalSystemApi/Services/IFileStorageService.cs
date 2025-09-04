using Microsoft.AspNetCore.Http;

namespace MedicalSystemApi.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        Task<bool> DeleteFileAsync(string filePath);
        Task<byte[]> DownloadFileAsync(string filePath);
    }
}