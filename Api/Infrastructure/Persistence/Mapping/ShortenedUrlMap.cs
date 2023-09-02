namespace url_shortener.Api.Infrastructure.Persistence.Mapping;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using url_shortener.Api.Application.Services;
using url_shortener.Api.Domain.Entities;

public class ShortenedUrlMap : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> modelBuilder)
    {
        modelBuilder.Property(s => s.Code).HasMaxLength(ShortenerUrlService.NumberOfCharsInShortLink);
        modelBuilder.HasIndex(s => s.Code).IsUnique();
    }
}