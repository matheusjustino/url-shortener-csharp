namespace url_shortener.Api.Domain.DTOs.ShortenerUrl;

using System.ComponentModel.DataAnnotations;

public class CreateUrl
{
    [Required]
    public string Url { get; set; }
}