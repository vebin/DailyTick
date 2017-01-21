using System;
using System.IO;
using NGraphics;

[assembly: Xamarin.Forms.Dependency(typeof(DailyTick.Droid.DurationImageSourceImplementation))]
namespace DailyTick.Droid
{
    class DurationImageSourceImplementation : IDurationImageSource
    {
        public DurationImageSourceImplementation()
        {

        }

        public void Generate(MemoryStream stream, string text)
        {
            var size = new Size(40);
            var font = new Font {
                Name = "Arial",
                Size = 14
            };
            var canvas = Platforms.Current.CreateImageCanvas(size);
            var textSize = canvas.MeasureText(text, font);

            canvas.DrawText(text, new Point((size.Width - textSize.Width*0.8) * 0.5, 24), font, Colors.Black);
            canvas.GetImage().SaveAsPng(stream);
        }
    }
}