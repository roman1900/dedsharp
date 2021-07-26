using System;
using System.IO;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
            };

            using (window = new Window(GameWindowSettings.Default,nativeWindowSettings,editor))
            {
                window.UpdateFrequency=FPS;
                window.KeyDown +=  keyPress;
                window.TextInput += textInput;
                
                window.Run();
                
                
            }
        }
        static void keyPress(KeyboardKeyEventArgs k)
        {
            switch (k.Key)
            {
                case Keys.Escape:
                    window.Close();
                    break;
                case Keys.Backspace:
                    editor.editor_backspace();
                    break;
                case Keys.F2:
                    if (editor.file_path != null)
                    {
                        editor.editor_save_to_file();
                    }
                    break;
                case Keys.Enter:
                    editor.editor_insert_new_line();
                    break;
                case Keys.Delete:
                    editor.editor_delete();
                    break;
                case Keys.Up:
                    if (editor.cursor_row > 0)
                    {
                        editor.cursor_row -= 1;
                    }
                    break;
                case Keys.Down:
                    editor.cursor_row -= 1;
                    break;
                case Keys.Left:
                    if (editor.cursor_col > 0) {
                        editor.cursor_col -= 1;
                    }
                    break;
                case Keys.Right:
                    editor.cursor_row +=1;
                    break;
            }
        }
        static void textInput(TextInputEventArgs t)
        {
            editor.editor_insert_text_before_cursor(t.AsString);
        }
    }
}
