using LegacyCrawler.Models;
using LegacyCrawler.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LegacyCrawler.Services
{
    /// <summary>
    /// 다중 게시판 크롤링 서비스
    /// </summary>
    public class MultiCrawlerService(
        IWebDownloader webDownloader,
        IHtmlParser htmlParser,
        IDataExporter dataExporter,
        IOptions<CrawlerSettings> options
            ) : ICrawlerService
    {
        private readonly IWebDownloader _webDownloader = webDownloader;
        private readonly IHtmlParser _htmlParser = htmlParser;
        private readonly IDataExporter _dataExporter = dataExporter;
        private readonly CrawlerSettings _settings = options.Value;

        public async Task CrawlAsync()
        {
            Console.WriteLine("게시판 크롤링을 시작합니다...");

            // 출력 디렉토리 확인 및 생성
            if (!Directory.Exists(_settings.OutputDirectory))
            {
                Directory.CreateDirectory(_settings.OutputDirectory);
                Console.WriteLine($"출력 디렉토리 생성: {Path.GetFullPath(_settings.OutputDirectory)}");
            }

            // 각 게시판에 대해 크롤링을 수행
            foreach (var board in _settings.Boards)
            {
                Console.WriteLine($"===== '{board.Name}' 게시판 크롤링 시작 =====");
                await CrawlBoardAsync(board);
                Console.WriteLine($"===== '{board.Name}' 게시판 크롤링 완료 =====");

                // 각 게시판 크롤링 후 대기
                await Task.Delay(_settings.DelayMilliseconds * 2);
            }

            Console.WriteLine("모든 게시판 크롤링이 완료되었습니다.");
        }

        private async Task CrawlBoardAsync(BoardSettings board)
        {
            var allItems = new List<BoardItem>();
            var endPage = _settings.MaxPages;

            try
            {
                // 첫 페이지를 로드하여 마지막 페이지 번호 감지
                var firstPageUrl = board.Url;
                Console.WriteLine($"첫 페이지 URL: {firstPageUrl}");
                var html = await _webDownloader.GetHtmlAsync(firstPageUrl);

                if(_settings.DetectLastPage)
                {
                    // 마지막 페이지 감지
                    endPage = _htmlParser.DetectLastPageNumber(html);
                }

                // 첫 페이지 HTML 파싱
                var firstPageItems = _htmlParser.ParseBoardItems(html, firstPageUrl, board);
                allItems.AddRange(firstPageItems);

                // 나머지 페이지 크롤링
                for (var pageNum = 2; pageNum <= endPage; pageNum++)
                {
                    Console.WriteLine($"페이지 {pageNum}/{endPage} 크롤링 중...");

                    // 페이지 URL 생성
                    var pageUrl = GetWordPressPageUrl(board.Url, pageNum);
                    Console.WriteLine($"페이지 URL: {pageUrl}");

                    // 페이지 HTML 다운로드
                    var pageHtml = await _webDownloader.GetHtmlAsync(pageUrl);

                    // HTML 파싱
                    var pageItems = _htmlParser.ParseBoardItems(pageHtml, pageUrl, board);
                    allItems.AddRange(pageItems);

                    // 서버에 부담을 주지 않기 위해 대기
                    if (pageNum < endPage)
                    {
                        await Task.Delay(_settings.DelayMilliseconds);
                    }
                }

                // 결과를 CSV 파일로 저장
                if (allItems.Count != 0)
                {
                    // 결과 파일명 생성
                    string outputFileName = GetOutputFileName(board.Name);
                    _dataExporter.ExportToCsv(allItems, outputFileName);

                    Console.WriteLine($"크롤링 완료! 총 {allItems.Count}개의 항목을 수집했습니다.");
                    Console.WriteLine($"결과 파일: {outputFileName}");
                }
                else
                {
                    Console.WriteLine($"'{board.Name}' 게시판에서 수집된 항목이 없습니다.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"'{board.Name}' 게시판 크롤링 오류: {ex.Message}");
                Console.WriteLine($"스택 트레이스: {ex.StackTrace}");
            }
        }

        // 결과 파일명 생성
        private string GetOutputFileName(string boardName)
        {
            // 출력 디렉토리와 파일명 조합
            if (_settings.AppendDateTime)
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                return Path.Combine(_settings.OutputDirectory, $"{boardName}_{timestamp}.csv");
            }
            else
            {
                return Path.Combine(_settings.OutputDirectory, $"{boardName}.csv");
            }
        }

        // WordPress 페이지 URL 생성
        private string GetWordPressPageUrl(string boardUrl, int pageNum)
        {
            // WordPress의 페이지 URL 형식에 맞게 변환
            if (boardUrl.EndsWith('/'))
            {
                return $"{boardUrl}page/{pageNum}/";
            }
            else
            {
                return $"{boardUrl}/page/{pageNum}/";
            }
        }
    }
}
