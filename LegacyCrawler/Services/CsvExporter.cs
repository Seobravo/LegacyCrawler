using CsvHelper;
using CsvHelper.Configuration;
using LegacyCrawler.Models;
using LegacyCrawler.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace LegacyCrawler.Services
{
    public class CsvExporter(IOptions<CrawlerSettings> options) : IDataExporter
    {
        private readonly CrawlerSettings _settings = options.Value;

        public void ExportToCsv(List<BoardItem> items, string filePath)
        {
            try
            {
                // 디렉토리 확인 및 생성
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // CSV 파일 생성
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // CSV 설정
                    HasHeaderRecord = true,
                    Delimiter = ",",
                    Encoding = Encoding.UTF8
                }))
                {
                    // 헤더 작성
                    csv.WriteHeader<BoardItem>();
                    csv.NextRecord();

                    // 데이터 작성
                    foreach (var item in items)
                    {
                        csv.WriteRecord(item);
                        csv.NextRecord();
                    }
                }

                Console.WriteLine($"{items.Count}개의 항목이 {filePath}에 저장되었습니다.");
                Console.WriteLine($"파일 경로: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSV 저장 오류: {ex.Message}");
                throw;
            }
        }
    }
}
