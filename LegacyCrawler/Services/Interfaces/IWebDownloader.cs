namespace LegacyCrawler.Services.Interfaces
{
    public interface IWebDownloader
    {
        Task<string> GetHtmlAsync(string url);
    }
}
