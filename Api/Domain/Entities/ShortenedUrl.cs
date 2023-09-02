namespace url_shortener.Api.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public class ShortenedUrl
{
    public Guid Id { get; set; }

    [Required]
    public string LongUrl { get; set; } = string.Empty;

    [Required]
    public string ShortUrl { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}