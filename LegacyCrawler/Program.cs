using LegacyCrawler.Models;
using LegacyCrawler.Services;
using LegacyCrawler.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var serviceProvider = new ServiceCollection()
    .Configure<CrawlerSettings>(configuration.GetSection("CrawlerSettings"))
    .AddSingleton<IWebDownloader, WebDownloader>()
    .AddSingleton<IHtmlParser, HtmlParser>()
    .AddSingleton<IDataExporter, CsvExporter>()
    .AddSingleton<ICrawlerService, MultiCrawlerService>()
    .BuildServiceProvider();

try
{
    var crawlerService = serviceProvider.GetRequiredService<ICrawlerService>();
    await crawlerService.CrawlAsync();
}
catch(Exception ex)
{
    Console.WriteLine($"오류 발생: {ex.Message}");
}

Console.WriteLine("프로그램을 종료하려면 아무 키나 누르세요..");
Console.ReadKey();

if(serviceProvider is IDisposable disposable)
{
    disposable.Dispose();
}