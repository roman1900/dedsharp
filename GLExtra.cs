using System;
using System.Runtime.CompilerServices;
using System.IO;
using OpenTK.Graphics.OpenGL;
namespace dedsharp{
	public class GLExtra{
		static bool compile_shader_source(string source,ShaderType shader_type,ref int shader){
			shader = GL.CreateShader(shader_type);
			GL.ShaderSource(shader,source);
			GL.CompileShader(shader);
			int compiled = 0;
			GL.GetShader(shader,ShaderParameter.CompileStatus,out compiled);
			if (compiled==0){
				Console.WriteLine($"ERROR: could not compile {shader_type.ToString()}");
				Console.WriteLine($"{GL.GetShaderInfoLog(shader)}");
				return false;
			}
			return true;
		}
		public static bool compile_shader_file(string file_path, ShaderType shader_type, ref int shader){
			string source = File.ReadAllText(file_path);
			bool ok = compile_shader_source(source,shader_type,ref shader);
			if (!ok)
			{
				Console.WriteLine($"Error: failed to compile {file_path} shader file");
			}
			return ok;
		}
		public static void attach_shaders_to_program(int[] shaders,int shaders_count,int program)
		{
			for (int i=0; i < shaders_count; ++i){
				GL.AttachShader(program,shaders[i]);
			}
		}
		public static bool link_program(int program,[CallerFilePath]string file_path="",[CallerLineNumber]int line=0)
		{
			GL.LinkProgram(program);
			int linked = 0;
			GL.GetProgram(program,GetProgramParameterName.LinkStatus,out linked);
			if (linked==0){
				string message = GL.GetProgramInfoLog(program);
				Console.WriteLine($"{file_path}:{line}: Program Linking: {message}");
			} 
			return linked == 1;
		}
	}
}