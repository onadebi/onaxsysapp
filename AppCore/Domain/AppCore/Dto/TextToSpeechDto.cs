using System;
namespace AppCore.Domain.AppCore.Dto;
public class TextToSpeechDto
{
    public required string clearText { get; set; }
    public string voice { get; set; } = string.Empty;
}
