using System.IO;
using SixLabors.Fonts;
using SixLabors.Memory;
using SixLabors.ImageSharp;
using SixLabors.Shapes;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace BierRapportage.Models
{
    class ImageHelper
    {
            public static Stream AddTextToImage(Stream imageStream, params (string text, (float x, float y) position)[] texts)
            {
                var memoryStream = new MemoryStream();
                var image = Image.Load(imageStream);

                image.Clone(img =>
                    {
                        foreach (var (text, (x, y)) in texts)
                        {
                            img.DrawText(text, SystemFonts.CreateFont("Calibri", 28), Rgba32.DarkRed, new PointF(x, y));
                        }
                    })
                    .SaveAsPng(memoryStream);

                memoryStream.Position = 0;
                return memoryStream;
            }      
    }
}
