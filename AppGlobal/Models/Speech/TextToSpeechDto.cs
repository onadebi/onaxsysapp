using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGlobal.Models.Speech;

public class TextToSpeechDto
{
    public required string clearText { get; set; }
    public string voice { get; set; } = string.Empty;
}
