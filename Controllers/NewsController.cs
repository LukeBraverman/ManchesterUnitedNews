using Microsoft.AspNetCore.Mvc;
using ManUtdDashboard.Models;
using System.Net.Http;
using System.Text.Json;

namespace ManUtdDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "ab555ceceb785f006e9ab39d5d072e96";
    private readonly string _gnewsUrl = "https://gnews.io/api/v4/search?q=manchester+united&lang=en&token=";

    public NewsController()
    {
        _httpClient = new HttpClient();
    }

    [HttpGet]
    public async Task<IActionResult> GetNews()
    {
        var url = _gnewsUrl + _apiKey;

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch news.");
        }

        var content = await response.Content.ReadAsStringAsync();

        using var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;

        var articles = new List<NewsArticle>();

        foreach (var item in root.GetProperty("articles").EnumerateArray())
        {
            articles.Add(new NewsArticle
            {
                Title = item.GetProperty("title").GetString(),
                Description = item.GetProperty("description").GetString(),
                Url = item.GetProperty("url").GetString(),
                Image = item.GetProperty("image").GetString(),
                PublishedAt = item.GetProperty("publishedAt").GetDateTime(),
                SourceName = item.GetProperty("source").GetProperty("name").GetString()
            });
        }

        return Ok(articles);
    }
}