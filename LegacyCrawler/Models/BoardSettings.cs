namespace LegacyCrawler.Models
{
    public class BoardSettings
    {
        public string Name { get; set; } = string.Empty; // 게시판 이름 (예: "이목사의창칼럼", "게시판")
        public string Url { get; set; } = string.Empty; // 게시판 URL
        public string ItemSelector { get; set; } = string.Empty; // 게시물 항목 선택자
        public string TitleSelector { get; set; } = string.Empty; // 제목 선택자
        public string ContentSelector { get; set; } = string.Empty; // 내용 선택자
        public string PaginationSelector { get; set; } = string.Empty; // 페이지네이션 선택자
    }
}
