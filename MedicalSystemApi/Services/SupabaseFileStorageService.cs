using MedicalSystemApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MedicalSystemApi.Services
{
    public class SupabaseFileStorageService : IFileStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SupabaseFileStorageService> _logger;

        public SupabaseFileStorageService(IConfiguration configuration, ILogger<SupabaseFileStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            _logger.LogInformation("=== STORAGE UPLOAD STARTED ===");
            _logger.LogInformation("File: {FileName}, Folder: {Folder}", file.FileName, folderName);

            try
            {
                var supabaseUrl = _configuration["Supabase:Url"];
                var supabaseKey = _configuration["Supabase:Key"];

                _logger.LogInformation("Supabase URL: {Url}", supabaseUrl);
                _logger.LogInformation("Supabase Key: {Key}", supabaseKey?.Substring(0, 10) + "...");

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = $"{folderName}/{fileName}";

                _logger.LogInformation("Generated file path: {FilePath}", filePath);

                // Use HttpClient for direct upload
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", supabaseKey);

                var uploadUrl = $"{supabaseUrl}/storage/v1/object/medical-files/{filePath}";
                _logger.LogInformation("Upload URL: {UploadUrl}", uploadUrl);

                // Read file content
                using var fileStream = file.OpenReadStream();
                using var content = new StreamContent(fileStream);

                if (!string.IsNullOrEmpty(file.ContentType))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                }

                _logger.LogInformation("Sending upload request...");
                var response = await httpClient.PostAsync(uploadUrl, content);
                _logger.LogInformation("Upload response status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Upload failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    throw new Exception($"Upload failed: {response.StatusCode} - {errorContent}");
                }

                _logger.LogInformation("=== STORAGE UPLOAD SUCCESSFUL ===");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== STORAGE UPLOAD FAILED ===");
                throw;
            }
        }





        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var supabaseUrl = _configuration["Supabase:Url"];
                var supabaseKey = _configuration["Supabase:Key"];

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", supabaseKey);

                var deleteUrl = $"{supabaseUrl}/storage/v1/object/medical-files/{filePath}";
                var response = await httpClient.DeleteAsync(deleteUrl);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<byte[]> DownloadFileAsync(string filePath)
        {
            try
            {
                var supabaseUrl = _configuration["Supabase:Url"];
                var supabaseKey = _configuration["Supabase:Key"];

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);

                var downloadUrl = $"{supabaseUrl}/storage/v1/object/public/medical-files/{filePath}";
                return await httpClient.GetByteArrayAsync(downloadUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {FilePath}", filePath);
                throw;
            }
        }
    }
}