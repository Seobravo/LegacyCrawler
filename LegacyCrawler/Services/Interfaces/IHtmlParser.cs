using LegacyCrawler.Models;

namespace LegacyCrawler.Services.Interfaces
{
    public interface IHtmlParser
    {
        int DetectLastPageNumber(string html);
        List<BoardItem> ParseBoardItems(string html, string pageUrl, BoardSettings boardSettings);
    }
}
