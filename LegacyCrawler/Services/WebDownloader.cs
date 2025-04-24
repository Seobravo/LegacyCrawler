using LegacyCrawler.Services.Interfaces;

namespace LegacyCrawler.Services
{
    public class WebDownloader : IWebDownloader, IDisposable
    {
        private readonly HttpClient _httpClient;

        public WebDownloader()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<string> GetHtmlAsync(string url)
        {
            try
            {
                Console.WriteLine($"다운로드 중: {url}");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"HTML 다운로드 오류: {ex.Message}");
                throw;
            }
        }
    }
}
