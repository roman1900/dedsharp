using System;
using System.IO;
using System.Linq;
namespace dedsharp {
	public class Editor
	{
		public struct Line {
			public int capacity;
			public int size;
			public string chars;
		}
		private const int EDITOR_INIT_CAPACITY = 128;
		public string file_path;
		public int capacity {get;set;}
		public int size {get;set;}
		public Line[] lines {get;set;}
		public int cursor_row;
		public int cursor_col;
		public void line_append_text(ref Line line, string text, int text_size)
		{
			int col = line.size;
			line_insert_text_before(ref line, text, text_size, col);
		}
		public void line_insert_text_before(ref Line line,string text,int text_size, int col)
		{
			throw(new NotImplementedException());
		}
		public void line_backspace(ref Line line, ref int col)
		{
			if (col > line.size) {
				col = line.size;
			}

			if (col > 0 && line.size > 0) {
				line.chars = line.chars.Substring(0,col - 1)+line.chars.Substring(col);
				line.size -= 1;
				col -= 1;
			}
		}

		public void line_delete(ref Line line, ref int col)
		{
			if (col > line.size) {
				col = line.size;
			}

			if (col < line.size && line.size > 0) {
				line.chars = line.chars.Substring(0,col)+line.chars.Substring(col+1);
				line.size -= 1;
			}
		}
		private void editor_grow(int n)
		{
			int new_capacity = this.capacity;
			if (new_capacity < this.size)
			{
				throw(new Exception($"The Capacity {new_capacity} is less than the current editor size {this.size}"));
			}
			while (new_capacity - this.size < n)
			{
				if (new_capacity == 0) {
					new_capacity = EDITOR_INIT_CAPACITY;
				} 
				else
				{
					new_capacity *= 2;
				}
			}
			if (new_capacity != this.capacity)
			{
				Line[] temp_lines = this.lines;
				Array.Resize(ref temp_lines,new_capacity);
				this.lines = temp_lines;
				this.capacity = new_capacity;
			}
			
		}

		public void editor_insert_new_line()
		{
			if (this.cursor_row > this.size)
			{
				this.cursor_row = this.size;
			}
			editor_grow(1);
			Array.Copy(this.lines,this.cursor_row,this.lines,this.cursor_row+1,this.cursor_row);
			this.lines[this.cursor_row+1]=new Line();
			this.cursor_row +=1;
			this.cursor_col = 0;
			this.size +=1;

		}

		private void editor_create_first_new_line()
		{
			if (this.cursor_row >= this.size) {
				if (this.size > 0) {
					this.cursor_row = this.size - 1;
				} else {
					editor_grow(1);
					this.lines[this.size] = new Line();
					this.size += 1;
				}
			}
		}
		public void editor_insert_text_before_cursor(string text)
		{
			editor_create_first_new_line();
			line_insert_text_before(ref this.lines[this.cursor_row], text, text.Length, this.cursor_col);
		}

		public void editor_backspace()
		{
			editor_create_first_new_line();
			line_backspace(ref this.lines[this.cursor_row], ref this.cursor_col);
		}

		public void editor_delete()
		{
			editor_create_first_new_line();
			line_delete(ref this.lines[this.cursor_row], ref this.cursor_col);
		}
		public char? editor_char_under_cursor()
		{
			if (this.cursor_row < this.size) {
				if (this.cursor_col < this.lines[this.cursor_row].size) {
					return this.lines[this.cursor_row].chars[this.cursor_col];
				}
			}
			return null;
		}
		public async void editor_save_to_file()
		{
			await File.WriteAllLinesAsync(file_path,this.lines.Select(x =>  x.chars).ToArray());
		}
		public void editor_load_from_file(string file_path)
		{
			if (this.lines != null)
			{
				throw(new Exception("You can only load files into an empty editor"));
			}
			editor_create_first_new_line();
			string[] file_lines = File.ReadAllLines(file_path);
			foreach(string file_line in file_lines)
			{
				Line line = new Line();
				line_append_text(ref line, file_line, file_line.Length);
				this.lines[this.size-1] = line;
				editor_insert_new_line();
			}
			this.cursor_row = 0;
			this.file_path = file_path;
		}
	}
}