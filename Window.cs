using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System;
using SharpFont;


namespace dedsharp
{
	
    public class Window : GameWindow
    {
		public static uint FREE_GLYPH_FONT_SIZE = 64;
		public static int ZOOM_OUT_GLYPH_THRESHOLD = 30;

		public static void MessageCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, System.IntPtr message, System.IntPtr userParam )
		{
			Console.WriteLine($"GL CALLBACK: type = {type} severity = {severity} message = {Marshal.PtrToStringAuto(message)}");
		}
		
		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings,nativeWindowSettings) { }
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			
			base.OnUpdateFrame(e);
		}

		protected override void OnLoad()
		{

			base.OnLoad();
			//TODO: exception checking
			Library library = new Library();
			const string font_file_path = "./VictorMono-Regualr.ttf";
			Face face = new Face(library, font_file_path);
			uint pixel_size = FREE_GLYPH_FONT_SIZE;
			face.SetPixelSizes(0,pixel_size);

			
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.DebugOutput);
			DebugProc MCB = MessageCallback;
			GL.DebugMessageCallback(MCB,new System.IntPtr(0));
			FreeGlyphBuffer fgb = new FreeGlyphBuffer(
                           face,
                           "./shaders/free_glyph.vert",
                           "./shaders/free_glyph.frag");
			
		}
    }
}