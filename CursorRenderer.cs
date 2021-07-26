using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
namespace dedsharp
{
	public class CursorRenderer
	{
		public int program;
		public int[] uniforms = new int[(int)Uniforms.Uniform_Slot.COUNT_UNIFORM_SLOTS];

		public CursorRenderer(string vert_file_path, string frag_file_path)
		{
			int[] shaders = new int[3];
			if(!GLExtra.compile_shader_file(vert_file_path, ShaderType.VertexShader, ref shaders[0])){
				System.Environment.Exit(1);
			}
			if(!GLExtra.compile_shader_file(frag_file_path, ShaderType.FragmentShader, ref shaders[1])){
				System.Environment.Exit(1);
			}
			if(!GLExtra.compile_shader_file("./shaders/camera.vert", ShaderType.VertexShader,ref shaders[2])){
				System.Environment.Exit(1);
			}
			this.program = GL.CreateProgram();
			GLExtra.attach_shaders_to_program(shaders,3,this.program);
			if(!GLExtra.link_program(this.program)){
				System.Environment.Exit(1);
			}
			GL.UseProgram(this.program);
			Uniforms.get_uniform_location(this.program,ref this.uniforms);
		}
		public void cursor_renderer_use()
		{
			GL.UseProgram(this.program);
		}
		public void cursor_renderer_move_to(Vector2 pos)
		{
			GL.Uniform2(this.uniforms[(int)Uniforms.Uniform_Slot.UNIFORM_SLOT_CURSOR_POS],pos.X,pos.Y);
		}
		public void cursor_renderer_draw()
		{
			GL.DrawArrays(PrimitiveType.TriangleStrip,0,4);
		}
	}
}