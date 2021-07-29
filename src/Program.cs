using System;
using System.IO;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

//using freetype;

namespace dedsharp
{
    class Program
    {
        static Editor editor = new Editor();
        static Window window;

        public const int SCREEN_WIDTH = 800;
        const int SCREEN_HEIGHT = 600;
        const int FPS = 60;
        public const float DELTA_TIME = 1.0f / FPS;
        static void Main(string[] args)
        {
            
            if (args.Length > 0)
            {
                editor.file_path = args[0];
            }
            if (editor.file_path != null)
            {
                if (File.Exists(editor.file_path))
                {
                    editor.editor_load_from_file(editor.file_path);
                }
            }
            
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings(){
                Title="Text Editor",
                Size = new Vector2i(SCREEN_WIDTH,SCREEN_HEIGHT),
                API = ContextAPI.OpenGL,
            };

            using (window = new Window(GameWindowSettings.Default,nativeWindowSettings,editor))
            {
                window.RenderFrequency=FPS;
                 window.Run();
            }
        }
    }
}
