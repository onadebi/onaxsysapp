using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Domain.AppCore.Dto
{
    public class ImageGenerationResponseDto
    {
        public int created { get; set; }
        public required List<Data> data { get; set; }
    }

    public class Data
    {
        public string? url { get; set; }
    }
}
