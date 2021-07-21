using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using OpenTK.Graphics;

namespace dedsharp
{
    public class Window : GameWindow
    {
		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings,nativeWindowSettings) { }
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			
			base.OnUpdateFrame(e);
		}
    }
}