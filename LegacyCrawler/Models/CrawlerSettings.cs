namespace LegacyCrawler.Models
{
    // 크롤링할 대상 웹사이트 설정 클래스
    public class CrawlerSettings
    {
        public string BaseUrl { get; set; } = "https://example.com/";
        public bool IsPathBasedPagination { get; set; } = true; // 경로 기반 페이지네이션 사용 여부
        public string PathPageFormat { get; set; } = "page/{0}"; // 경로 기반 페이지 형식 (예: page/3)
        public string QueryPageFormat { get; set; } = "page={0}"; // 쿼리 기반 페이지 형식 (예: page=3)
        public int StartPage { get; set; } = 1;
        public bool DetectLastPage { get; set; } = true; // 마지막 페이지 자동 감지 여부
        public int MaxPages { get; set; } = 10; // 최대 크롤링할 페이지 수 (DetectLastPage가 false일 때 사용)
        public int DelayMilliseconds { get; set; } = 1500;
        public string OutputDirectory { get; set; } = "data"; // 결과 저장 디렉토리
        public string LastPagePattern { get; set; } = "//div[@class='pagination']//a[last()]"; // 마지막 페이지를 찾기 위한 XPath
        public bool AppendDateTime { get; set; } = true; // 파일명에 날짜/시간 추가 여부

        // 게시판 설정 목록
        public List<BoardSettings> Boards { get; set; } = [];
    }
}
