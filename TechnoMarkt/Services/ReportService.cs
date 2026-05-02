using PuppeteerSharp;
using PuppeteerSharp.Media;

public class ReportService
{
    private readonly IViewRenderService _viewRenderService;

    public ReportService(IViewRenderService viewRenderService)
    {
        _viewRenderService = viewRenderService;
    }

    public async Task<byte[]> GeneratePdfReportAsync<TModel>(string viewPath, TModel model)
    {
        string htmlContent = await _viewRenderService.RenderToStringAsync(viewPath, model);

        BrowserFetcher browserFetcher = new BrowserFetcher();

        await browserFetcher.DownloadAsync();

        var launchOptions = new LaunchOptions()
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        };

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();

        await page.SetContentAsync(htmlContent, new NavigationOptions
        {
            WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
        });

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px", Left = "20px", Right = "20px" }
        });
    }
}