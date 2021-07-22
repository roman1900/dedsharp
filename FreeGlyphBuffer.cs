
using OpenTK.Mathematics;
namespace dedsharp
{
	public class FreeGlyphBuffer
	{
		private struct  Attr_Def
		{
			int offset;
			int comps;
			int type; //TODO: use correct enum
		}
		public struct Free_Glyph {
			Vector2 pos;
			Vector2 size;
			Vector2 uv_pos;
			Vector2 uv_size;
			Vector4 fg_color;
			Vector4 bg_color;
		} 
		public struct Glyph_Metric {
			float ax; // advance.x
			float ay; // advance.y
			
			float bw; // bitmap.width;
			float bh; // bitmap.rows;
			
			float bl; // bitmap_left;
			float bt; // bitmap_top;
			
			float tx; // x offset of glyph in texture coordinates
		}
		public const int FREE_GLYPH_BUFFER_CAP = (640 * 1000);
		public int vao;
		public int vbo;
		public int program;
		public uint atlas_width;
		public uint atlas_height;
		public int glyphs_texture;
		public int[] uniforms = new int[(int)Uniforms.Uniform_Slot.COUNT_UNIFORM_SLOTS];
		public int glyphs_count;
		public Free_Glyph[] glyphs = new Free_Glyph[FREE_GLYPH_BUFFER_CAP];
		public Glyph_Metric[] metrics = new Glyph_Metric[128];
	}
}