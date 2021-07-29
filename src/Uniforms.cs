using System.Collections.Generic;
using System;
using OpenTK.Graphics.OpenGL;
namespace dedsharp
{
	public class Uniforms 
	{
		public enum Uniform_Slot
		{
			UNIFORM_SLOT_TIME = 0,
			UNIFORM_SLOT_RESOLUTION,
			UNIFORM_SLOT_CAMERA_POS,
			UNIFORM_SLOT_CAMERA_SCALE,
			UNIFORM_SLOT_CURSOR_POS,
			UNIFORM_SLOT_CURSOR_HEIGHT,
			UNIFORM_SLOT_LAST_STROKE,
			COUNT_UNIFORM_SLOTS,
		}
		public struct Uniform_Def
		{
			public Uniform_Slot slot;
			public string name;
			public Uniform_Def (Uniform_Slot slot, string name)
			{
				this.slot = slot;
				this.name = name;
			}
		}
		static Uniforms()
		{
			if ((int)Uniform_Slot.COUNT_UNIFORM_SLOTS != 7)
				throw (new Exception("The amount of the shader uniforms have changed. Please update the definition table accordingly"));
		}
		static Dictionary<int,Uniform_Def> uniform_defs= new Dictionary<int,Uniform_Def>
		{
		   	{(int)Uniform_Slot.UNIFORM_SLOT_TIME,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_TIME,"time")},
		   	{(int)Uniform_Slot.UNIFORM_SLOT_RESOLUTION,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_RESOLUTION,"resolution")},
			{(int)Uniform_Slot.UNIFORM_SLOT_CAMERA_POS,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_CAMERA_POS,"camera_pos")},
			{(int)Uniform_Slot.UNIFORM_SLOT_CAMERA_SCALE,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_CAMERA_SCALE,"camera_scale")},
			{(int)Uniform_Slot.UNIFORM_SLOT_CURSOR_POS,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_CURSOR_POS,"cursor_pos")},
			{(int)Uniform_Slot.UNIFORM_SLOT_CURSOR_HEIGHT,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_CURSOR_HEIGHT,"cursor_height")},
			{(int)Uniform_Slot.UNIFORM_SLOT_LAST_STROKE,new Uniform_Def(Uniform_Slot.UNIFORM_SLOT_LAST_STROKE,"last_stroke")},
		};
		static public void get_uniform_location(int program, ref int[] locations)
		{
			for (Uniform_Slot slot = 0; slot < Uniform_Slot.COUNT_UNIFORM_SLOTS; ++slot)
			{
				locations[(int)slot] = GL.GetUniformLocation(program,uniform_defs[(int)slot].name);
			}
		}
	}
}