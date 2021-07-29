
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using SharpFont;
namespace dedsharp
{
	public class FreeGlyphBuffer
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Free_Glyph {
			public Vector2 pos;
			public Vector2 size;
			public Vector2 uv_pos;
			public Vector2 uv_size;
			public Vector4 fg_color;
			public Vector4 bg_color;
		} 
		public enum Free_Gylph_Attr {
			FREE_GLYPH_ATTR_POS = 0,
			FREE_GLYPH_ATTR_SIZE,
			FREE_GLYPH_ATTR_UV_POS,
			FREE_GLYPH_ATTR_UV_SIZE,
			FREE_GLYPH_ATTR_FG_COLOR,
			FREE_GLYPH_ATTR_BG_COLOR,
			COUNT_FREE_GLYPH_ATTRS,
		}
		private struct  Attr_Def
		{
			public System.IntPtr offset;
			public int comps;
			public VertexAttribPointerType type; 
			public Attr_Def(System.IntPtr offset,int comps,VertexAttribPointerType type)
			{
				this.offset = offset;
				this.comps = comps;
				this.type = type;
			}
		}
		private  static Dictionary<int,Attr_Def> glyph_attr_defs = new Dictionary<int, Attr_Def>
		{
			{(int)Free_Gylph_Attr.FREE_GLYPH_ATTR_POS,new Attr_Def(Marshal.OffsetOf<Free_Glyph>("pos"),2,VertexAttribPointerType.Float)},
			{(int)Free_Gylph_Attr.FREE_GLYPH_ATTR_SIZE,new Attr_Def(Marshal.OffsetOf<Free_Glyph>("size"),2,VertexAttribPointerType.Float)},
			{(int)Free_Gylph_Attr.FREE_GLYPH_ATTR_UV_POS,new Attr_Def(Marshal.OffsetOf<Free_Glyph>("uv_pos"),2,VertexAttribPointerType.Float)},
			{(int)Free_Gylph_Attr.FREE_GLYPH_ATTR_UV_SIZE,new Attr_Def(Marshal.OffsetOf<Free_Glyph>("uv_size"),2,VertexAttribPointerType.Float)},
			{(int)Free_Gylph_Attr.FREE_GLYPH_ATTR_FG_COLOR,new Attr_Def(Marshal.OffsetOf<Free_Glyph>("fg_color"),4,VertexAttribPointerType.Float)},
			{(int)Free_Gylph_Attr.FREE_GLYPH_ATTR_BG_COLOR,new Attr_Def(Marshal.OffsetOf<Free_Glyph>("bg_color"),4,VertexAttribPointerType.Float)},
		};
		public struct Glyph_Metric {
			public float ax; // advance.x
			public float ay; // advance.y
			
			public float bw; // bitmap.width;
			public float bh; // bitmap.rows;
			
			public float bl; // bitmap_left;
			public float bt; // bitmap_top;
			public float tx; // x offset of glyph in texture coordinates
		}
		public const int FREE_GLYPH_BUFFER_CAP = (640 * 1000);
		public int vao;
		public int vbo;
		public int program;
		public int atlas_width;
		public int atlas_height;
		public int glyphs_texture;
		public int[] uniforms = new int[(int)Uniforms.Uniform_Slot.COUNT_UNIFORM_SLOTS];
		public int glyphs_count;
		public Free_Glyph[] glyphs = new Free_Glyph[FREE_GLYPH_BUFFER_CAP];
		public Glyph_Metric[] metrics = new Glyph_Metric[128];
		
		public FreeGlyphBuffer(Face face,string vert_file_path,string frag_file_path){
			if ((int)Free_Gylph_Attr.COUNT_FREE_GLYPH_ATTRS != 6)
				throw(new Exception("The amount of glyph vertex attributes have changed"));

			//Init Vertex Attributes	
			vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);
			vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer,vbo);
			GL.BufferData<Free_Glyph>(BufferTarget.ArrayBuffer,Marshal.SizeOf<Free_Glyph>() * FREE_GLYPH_BUFFER_CAP,glyphs,BufferUsageHint.DynamicDraw);
			for (Free_Gylph_Attr attr = 0; attr < Free_Gylph_Attr.COUNT_FREE_GLYPH_ATTRS; ++attr) {
				GL.VertexAttribPointer((int)attr,
				 								glyph_attr_defs[(int)attr].comps,
				 								glyph_attr_defs[(int)attr].type,
				 								false,
				 								Marshal.SizeOf<Free_Glyph>(),
				 								glyph_attr_defs[(int)attr].offset);
				GL.EnableVertexAttribArray((int)attr);
				GL.VertexAttribDivisor((int)attr,1);
			}

			//Init Shaders
			int[] shaders = new int[3];
			if(!GLExtra.compile_shader_file(vert_file_path,ShaderType.VertexShader,ref shaders[0])){
				System.Environment.Exit(1);
			}
			if(!GLExtra.compile_shader_file(frag_file_path,ShaderType.FragmentShader,ref shaders[1])){
				System.Environment.Exit(1);
			}
			if(!GLExtra.compile_shader_file("./shaders/camera.vert",ShaderType.VertexShader,ref shaders[2])){
				System.Environment.Exit(1);
			}
			program = GL.CreateProgram();
			GLExtra.attach_shaders_to_program(shaders, shaders.Length,program);
			if(!GLExtra.link_program(program)){
				System.Environment.Exit(1);
			}
			GL.UseProgram(program);
			Uniforms.get_uniform_location(program,ref uniforms);

			//Glyph Texture Atlas
			for (uint i =32; i < 128; ++i){
				face.LoadChar(i,LoadFlags.Render,LoadTarget.Normal);
				atlas_width += face.Glyph.Bitmap.Width;
				if (atlas_height < face.Glyph.Bitmap.Rows){
					atlas_height = face.Glyph.Bitmap.Rows;
				}
			}
			
			glyphs_texture = GL.GenTexture();
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,glyphs_texture);
			GL.TexParameterI(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,new int[]{(int)TextureMagFilter.Linear});
			GL.TexParameterI(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,new int[]{(int)TextureMinFilter.Linear});
			GL.TexParameterI(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,new int[]{(int)TextureWrapMode.ClampToEdge});
			GL.TexParameterI(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,new int[]{(int)TextureWrapMode.ClampToEdge});
			GL.PixelStore(PixelStoreParameter.UnpackAlignment,1);
			GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.R16,atlas_width,atlas_height,0,PixelFormat.Red,PixelType.UnsignedByte,(IntPtr)null);
			int x = 0;
			for (uint i = 32; i < 128; ++i){
				face.LoadChar(i,LoadFlags.Render,LoadTarget.Normal);
				face.Glyph.RenderGlyph(RenderMode.Normal);
				metrics[i].ax = (int)face.Glyph.Advance.X ;//>> 6;
				metrics[i].ay = (int)face.Glyph.Advance.Y ;//>> 6;
				metrics[i].bw = face.Glyph.Bitmap.Width;
				metrics[i].bh = face.Glyph.Bitmap.Rows;
				metrics[i].bl = face.Glyph.BitmapLeft;
				metrics[i].bt = face.Glyph.BitmapTop;
				metrics[i].tx = (float) x / (float) atlas_width;
				GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				GL.TexSubImage2D(TextureTarget.Texture2D,
								0,
								x,
								0,
								face.Glyph.Bitmap.Width,
								face.Glyph.Bitmap.Rows,
								PixelFormat.Red,
								PixelType.UnsignedByte,
								face.Glyph.Bitmap.Buffer);
				
				x += face.Glyph.Bitmap.Width;
			}
		}
		public void free_glyph_buffer_use()
		{
			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer,vbo);
			GL.UseProgram(program);
		}
		public void free_glyph_buffer_clear()
		{
			glyphs_count = 0;
		}
		public void free_glyph_buffer_push(Free_Glyph glyph)
		{
			if (glyphs_count >= FREE_GLYPH_BUFFER_CAP) 
				throw(new Exception());
			glyphs[glyphs_count++] = glyph;
		}
		public void free_glyph_buffer_sync()
		{
			GL.BufferSubData<Free_Glyph>(BufferTarget.ArrayBuffer,
								(IntPtr)0,
								(IntPtr)(glyphs_count * Marshal.SizeOf<Free_Glyph>()),
								glyphs);
		}
		public void free_glyph_buffer_draw()
		{
			GL.DrawArraysInstanced(PrimitiveType.TriangleStrip,0,4,glyphs_count);
		}
		public float free_glyph_buffer_cursor_pos(string text,Vector2 pos,int col)
		{
			for (int i = 0;i < text.Length; ++i){
				if (i == col){
					return pos.X;
				}
				Glyph_Metric metric = metrics[(int)text[i]];
				pos.X += metric.ax;
				pos.Y += metric.ay;
			}
			return pos.X;
		}
		public void free_glyph_buffer_render_line_sized(string text,Vector2 pos,Vector4 fg_color, Vector4 bg_color)
		{
			for (int i = 0; i < text.Length; ++i){
				Glyph_Metric metric = metrics[(int)text[i]];
				float x2 = pos.X + metric.bl;
				float y2 = -pos.Y - metric.bt;
				float w = metric.bw;
				float h = metric.bh;

				pos.X += metric.ax;
				pos.Y += metric.ay;

				Free_Glyph glyph = new Free_Glyph();
				glyph.pos = new Vector2(x2, -y2);
				glyph.size = new Vector2(w, -h);
				glyph.uv_pos = new Vector2(metric.tx, 0.0f);
				glyph.uv_size = new Vector2(metric.bw / (float) atlas_width, metric.bh / (float) atlas_height);
				glyph.fg_color = fg_color;
				glyph.bg_color = bg_color;
				free_glyph_buffer_push(glyph);
			}
		}
	}
}