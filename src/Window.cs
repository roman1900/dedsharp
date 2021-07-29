using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using SharpFont;


namespace dedsharp
{
	
    public class Window : GameWindow
    {
		private Stopwatch sw = Stopwatch.StartNew();
		public static uint FREE_GLYPH_FONT_SIZE = 64;
		public static int ZOOM_OUT_GLYPH_THRESHOLD = 30;
		public static Vector2 camera_pos = new Vector2();
        public static Vector2 camera_vel = new Vector2();
        public static float camera_scale_vel = 0.0f;
		public static float camera_scale = 3.0f;
		Editor editor;
		private FreeGlyphBuffer fgb;
		private CursorRenderer cr;
		public static void MessageCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, System.IntPtr message, System.IntPtr userParam )
		{
			Console.WriteLine($"GL CALLBACK: type = {type} severity = {severity} message = {Marshal.PtrToStringAuto(message)}");
		}
		
		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings,Editor editor) : base(gameWindowSettings,nativeWindowSettings) {
			this.editor = editor;
		}
		private void render_editor_into_fgb()
		{
			float max_line_len = 0.0f;
			fgb.free_glyph_buffer_use();
			GL.Uniform2(fgb.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_RESOLUTION], (float)this.Size.X, (float)this.Size.Y);
			GL.Uniform1(fgb.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_TIME],(float) sw.Elapsed.TotalMilliseconds / 1000.0f);
			GL.Uniform2(fgb.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_CAMERA_POS],camera_pos.X,camera_pos.Y);
			GL.Uniform1(fgb.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_CAMERA_SCALE],camera_scale);

			fgb.free_glyph_buffer_clear();

			for (int row= 0; row < editor.size; ++row) 
			{
				Editor.Line line = editor.lines[row];
				Vector2 begin = new Vector2(0, -(float)row * FREE_GLYPH_FONT_SIZE);
				Vector2 end = begin;
				fgb.free_glyph_buffer_render_line_sized(line.chars,ref end,new Vector4(1.0f),new Vector4(0.0f));
				float line_len = Math.Abs(end.X - begin.X);
				if (line_len > max_line_len){
					max_line_len = line_len;
				}
			}
			fgb.free_glyph_buffer_sync();
			fgb.free_glyph_buffer_draw();

			Vector2 cursor_pos = new Vector2(0.0f);
			cursor_pos.Y = - editor.cursor_row * FREE_GLYPH_FONT_SIZE;
			if (editor.cursor_row < editor.size){
				Editor.Line line = editor.lines[editor.cursor_row];
				cursor_pos.X = fgb.free_glyph_buffer_cursor_pos(line.chars,new Vector2(0.0f,cursor_pos.Y),editor.cursor_col);
			}
			cr.cursor_renderer_use();
			GL.Uniform2(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_RESOLUTION],(float)this.Size.X,(float)this.Size.Y);
			GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_TIME],(float) sw.Elapsed.TotalMilliseconds / 1000.0f);
			GL.Uniform2(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_CAMERA_POS],camera_pos.X,camera_pos.Y);
			GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_CAMERA_SCALE], camera_scale);
			GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_CURSOR_HEIGHT], (float)FREE_GLYPH_FONT_SIZE);

			cr.cursor_renderer_move_to(cursor_pos);
			cr.cursor_renderer_draw();

			float target_scale = 3.0f;
			if (max_line_len > 0.0f) {
				target_scale = (float) this.Size.X / max_line_len;
			}
			if (target_scale > 3.0f){
				target_scale = 3.0f;
			}
			camera_vel = Vector2.Multiply(Vector2.Subtract(cursor_pos, camera_pos), 2.0f);
			camera_scale_vel = (target_scale - camera_scale) * 2.0f;
			camera_pos = Vector2.Add(camera_pos, Vector2.Multiply(camera_vel,Program.DELTA_TIME));
			camera_scale = camera_scale + camera_scale_vel * Program.DELTA_TIME;
		}
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			
			base.OnUpdateFrame(e);
			
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);
			
			GL.Clear(ClearBufferMask.ColorBufferBit);
			render_editor_into_fgb();
			SwapBuffers();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(0,0,Size.X,Size.Y);
		}
		protected override void OnLoad()
		{

			base.OnLoad();
			this.KeyDown +=  keyPress;
			this.TextInput += textInput;
			//TODO: exception checking
			Library library = new Library();
			const string font_file_path = "./resources/VictorMono-Regular.ttf";
			Face face = new Face(library, font_file_path,0);
			uint pixel_size = FREE_GLYPH_FONT_SIZE;
			face.SetPixelSizes(0,pixel_size);
			GL.Viewport(0,0,Size.X,Size.Y);
			GL.ClearColor(0.0f,0.0f,0.0f,1.0f);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.DebugOutput);
			DebugProc MCB = MessageCallback;
			GL.DebugMessageCallback(MCB,(IntPtr)0);
			fgb = new FreeGlyphBuffer(face,
                        	"./shaders/free_glyph.vert",
                        	"./shaders/free_glyph.frag");
			cr = new CursorRenderer("./shaders/cursor.vert",
                    		"./shaders/cursor.frag");
		}
		private void keyPress(KeyboardKeyEventArgs k)
        {
            switch (k.Key)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Backspace:
                    editor.editor_backspace();
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
                case Keys.F2:
                    if (editor.file_path != null)
                    {
                        editor.editor_save_to_file();
                    }
                    break;
                case Keys.Enter:
                    editor.editor_insert_new_line();
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
                case Keys.Delete:
                    editor.editor_delete();
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
                case Keys.Up:
                    if (editor.cursor_row > 0)
                    {
                        editor.cursor_row -= 1;
                    }
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
                case Keys.Down:
                    editor.cursor_row += 1;
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
                case Keys.Left:
                    if (editor.cursor_col > 0) {
                        editor.cursor_col -= 1;
                    } else {
						if (editor.cursor_row > 0)	
						{
							editor.cursor_row -=1;
							editor.cursor_col = editor.lines[editor.cursor_row].size;
						}
					}
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
                case Keys.Right:
					if	(editor.cursor_col < editor.lines[editor.cursor_row].size)
						editor.cursor_col +=1;
					else 
					{
						if (editor.size > editor.cursor_row)
						{
							editor.cursor_col =0;
							editor.cursor_row +=1;
						}
					}
					cr.cursor_renderer_use();
					GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
                    break;
            }
        }
        private void textInput(TextInputEventArgs t)
        {
            editor.editor_insert_text_before_cursor(t.AsString);
			cr.cursor_renderer_use();
			GL.Uniform1(cr.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_LAST_STROKE],(float)sw.ElapsedMilliseconds / 1000.0f);
        }
    }
}