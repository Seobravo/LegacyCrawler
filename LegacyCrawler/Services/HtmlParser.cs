using HtmlAgilityPack;
using LegacyCrawler.Models;
using LegacyCrawler.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;

namespace LegacyCrawler.Services
{
    public class HtmlParser(IOptions<CrawlerSettings> options) : IHtmlParser
    {
        private readonly CrawlerSettings _settings = options.Value;

        public int DetectLastPageNumber(string html)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // 마지막 페이지 링크(dots 다음에 오는 페이지 번호) 찾기
                var lastPageLink = doc.DocumentNode.SelectSingleNode("//div[@class='nav-links']/a[@class='page-numbers' and preceding-sibling::span[@class='page-numbers dots']]");

                if (lastPageLink != null)
                {
                    // 링크 텍스트에서 숫자 부분만 추출
                    var pageText = lastPageLink.InnerText.Trim();
                    var match = Regex.Match(pageText, @"\d+");

                    if (match.Success && int.TryParse(match.Value, out int pageNum))
                    {
                        Console.WriteLine($"마지막 페이지 감지: {pageNum}");
                        return pageNum;
                    }
                }

                // 기본값 반환 (감지 실패)
                return _settings.MaxPages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"마지막 페이지 감지 오류: {ex.Message}");
                return _settings.MaxPages;
            }
        }

        public List<BoardItem> ParseBoardItems(string html, string pageUrl, BoardSettings boardSettings)
        {
            var items = new List<BoardItem>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // 게시판 아이템을 포함하는 HTML 요소 선택
            var boardItemNodes = doc.DocumentNode.SelectNodes(boardSettings.ItemSelector);

            if (boardItemNodes != null)
            {
                Console.WriteLine($"{boardItemNodes.Count}개의 게시물 발견");

                foreach (var itemNode in boardItemNodes)
                {
                    try
                    {
                        // 제목 추출
                        var titleNode = itemNode.SelectSingleNode(boardSettings.TitleSelector);
                        string title = titleNode?.InnerText ?? "";
                        title = WebUtility.HtmlDecode(title).Trim();

                        // 내용 추출 (entry-content 클래스 내부의 HTML)
                        var contentNode = itemNode.SelectSingleNode(boardSettings.ContentSelector);
                        string content = "";

                        if (contentNode != null)
                        {
                            // HTML 엔티티 디코딩 (&#8230; 같은 특수 문자 처리)
                            content = WebUtility.HtmlDecode(contentNode.InnerHtml);
                            content = content.Trim();
                        }

                        var item = new BoardItem
                        {
                            Title = title,
                            Content = content
                        };

                        // 제목이 있는 경우에만 추가
                        if (!string.IsNullOrWhiteSpace(item.Title))
                        {
                            items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"항목 파싱 오류: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("게시물이 발견되지 않았습니다. 선택자가 올바른지 확인하세요.");
            }

            return items;
        }
    }
}
