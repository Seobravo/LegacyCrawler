using LegacyCrawler.Models;

namespace LegacyCrawler.Services.Interfaces
{
    public interface IDataExporter
    {
        void ExportToCsv(List<BoardItem> allItems, string outputFileName);
    }
}
