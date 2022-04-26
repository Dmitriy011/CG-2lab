using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace RedHeart
{
    public static class Program
    {
        private static void Main()
        {
            //НАстройки окна (размер)
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1024, 768),
            };

            //Запуск окна
            using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
            window.VSync = VSyncMode.On;
            window.Run();
        }
    }
}