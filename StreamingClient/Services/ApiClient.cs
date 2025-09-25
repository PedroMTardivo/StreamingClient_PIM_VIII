using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StreamingClient.Models;

namespace StreamingClient.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public async Task<List<CriadorDto>> GetCriadoresAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<CriadorDto>>("api/criadores");
            return result ?? new List<CriadorDto>();
        }

        public async Task<CriadorDto?> CreateCriadorAsync(string nome)
        {
            var resp = await _httpClient.PostAsJsonAsync("api/criadores", new { nome });
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<CriadorDto>();
        }

        public async Task<int> CountConteudosDoCriadorAsync(int criadorId)
        {
            var list = await GetConteudosByCriadorAsync(criadorId);
            return list.Count;
        }

        public async Task<bool> DeleteCriadorAsync(int id)
        {
            var resp = await _httpClient.DeleteAsync($"api/criadores/{id}");
            return resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public async Task<List<ConteudoDto>> GetConteudosByCriadorAsync(int criadorId)
        {
            var list = await _httpClient.GetFromJsonAsync<List<ConteudoDto>>($"api/criadores/{criadorId}/conteudos");
            return list ?? new List<ConteudoDto>();
        }

        public async Task<ConteudoDto?> CreateConteudoAsync(string titulo, string tipo, int criadorId)
        {
            var resp = await _httpClient.PostAsJsonAsync("api/conteudos", new { titulo, varchar = tipo, criadorId });
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ConteudoDto>();
        }

        public async Task<ConteudoDto?> UpdateConteudoAsync(int id, string? titulo, string? tipo)
        {
            var payload = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(titulo)) payload["titulo"] = titulo!;
            if (!string.IsNullOrWhiteSpace(tipo)) payload["varchar"] = tipo!;
            var resp = await _httpClient.PutAsJsonAsync($"api/conteudos/{id}", payload);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ConteudoDto>();
        }

        public async Task<bool> DeleteConteudoAsync(int id)
        {
            var resp = await _httpClient.DeleteAsync($"api/conteudos/{id}");
            return resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public async Task<bool> UploadArquivoAsync(int conteudoId, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                using var content = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);
                var fileName = Path.GetFileName(filePath);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentType(fileName));
                content.Add(fileContent, "arquivo", fileName);

                var response = await _httpClient.PostAsync($"api/arquivos/upload/{conteudoId}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Upload failed: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload exception: {ex.Message}");
                return false;
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".mp4":
                    return "video/mp4";
                case ".mp3":
                    return "audio/mp3";
                case ".avi":
                    return "video/avi";
                case ".mov":
                    return "video/mov";
                case ".wav":
                    return "audio/wav";
                default:
                    return "application/octet-stream";
            }
        }
    }
}

