using System;

namespace AppCore.Domain.AppCore.Dto;
public class ImageGenerationRequestDto
{
    public required string ImageSize { get; set; }
    public required string ImageSearch { get; set; }
    public int ImagesCount { get; set; } = 1;
}
