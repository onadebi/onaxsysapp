using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;

namespace AppGlobal.Services.Media;


public static class ImageHelper
{
    /// <summary>
    /// Creates an optimized thumbnail from an uploaded image using ImageSharp
    /// with a transparent background
    /// </summary>
    /// <param name="uploadedImage">The uploaded image from form</param>
    /// <param name="basePath">Base web path, typically IWebHostEnvironment.WebRootPath</param>
    /// <returns>The path to the generated thumbnail relative to webroot</returns>
    public static async Task<string> CreateThumbnailAsync(IFormFile uploadedImage, string basePath)
    {
        // Validate input
        if (uploadedImage == null || uploadedImage.Length == 0)
        {
            throw new ArgumentException("No image was uploaded", nameof(uploadedImage));
        }

        // Ensure thumbnail directory exists
        string thumbnailDirectory = Path.Combine(basePath, "thumbnail");
        if (!Directory.Exists(thumbnailDirectory))
        {
            Directory.CreateDirectory(thumbnailDirectory);
        }

        // Create unique filename for the thumbnail
        // Force PNG extension for transparency support
        string uniqueFileName = $"{Guid.NewGuid()}.png";
        string thumbnailPath = Path.Combine(thumbnailDirectory, uniqueFileName);

        // Target dimensions
        int targetWidth = 250;
        int targetHeight = 350;

        // Load and process the image
        using (var stream = uploadedImage.OpenReadStream())
        {
            // Create a new image with transparent background
            using (var outputImage = new Image<Rgba32>(targetWidth, targetHeight))
            {
                // Clear with transparency (all 0s = fully transparent)
                //outputImage.Mutate(x => x.Clear(Color.Transparent));
                outputImage.Mutate(x => x.BackgroundColor(Color.Transparent));

                // Load the input image
                using (var inputImage = await Image.LoadAsync(stream))
                {
                    // Calculate scaling to maintain aspect ratio
                    var scalingFactor = Math.Min(
                        (float)targetWidth / inputImage.Width,
                        (float)targetHeight / inputImage.Height);

                    var scaledWidth = (int)(inputImage.Width * scalingFactor);
                    var scaledHeight = (int)(inputImage.Height * scalingFactor);

                    // Resize the input image
                    inputImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(scaledWidth, scaledHeight),
                        Mode = ResizeMode.Stretch,
                        Sampler = KnownResamplers.Bicubic
                    }));

                    // Calculate position to center the image
                    int x = (targetWidth - scaledWidth) / 2;
                    int y = (targetHeight - scaledHeight) / 2;

                    // Draw the image onto the transparent background
                    outputImage.Mutate(ctx => ctx.DrawImage(inputImage, new Point(x, y), 1.0f));
                }

                // Always use PNG encoder for transparency support
                var encoder = new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression,
                    ColorType = PngColorType.RgbWithAlpha,
                    TransparentColorMode = PngTransparentColorMode.Preserve
                };

                // Save the thumbnail
                await outputImage.SaveAsync(thumbnailPath, encoder);
            }
        }

        // Return the relative path from web root
        return $"/thumbnail/{uniqueFileName}";
    }
}