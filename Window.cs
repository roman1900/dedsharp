using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System;
//using SharpFont;

namespace dedsharp
{
	
    public class Window : GameWindow
    {
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
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.DebugOutput);
			DebugProc MCB = MessageCallback;
			GL.DebugMessageCallback(MCB,new System.IntPtr(0));
			/*FreeGlyphBuffer fgb = new FreeGlyphBuffer(
                           face,
                           "./shaders/free_glyph.vert",
                           "./shaders/free_glyph.frag");*/
			
		}
    }
}