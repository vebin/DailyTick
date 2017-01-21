using System.IO;

namespace DailyTick
{
    public interface IDurationImageSource
    {
        void Generate(MemoryStream stream, string text);
    }
}
