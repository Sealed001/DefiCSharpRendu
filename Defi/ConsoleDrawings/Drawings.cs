using System;
using System.Collections.Generic;

namespace ConsoleDrawings
{
	public class ConsoleWindowCharacter
	{
		private char _character = ' ';
		public char Character
		{
			get => _character;
			set
			{
				if (value == _character) return;
				
				_character = value;
				Render();
			}
		}

		private ConsoleColor _color = ConsoleColor.White;
		public ConsoleColor Color
		{
			get => _color;
			set
			{
				if (value == _color) return;
				
				_color = value;
				Render();
			}
		}

		private readonly int _x;
		private readonly int _y;
		
		public ConsoleWindow ConsoleWindow;

		public ConsoleWindowCharacter(int x, int y, char character, ConsoleColor color = ConsoleColor.White)
		{
			_x = x;
			_y = y;
			Character = character;
			Color = color;
			
			Render();
		}

		public void Render()
		{
			Console.CursorVisible = false;

			if (_y + _x / Console.BufferWidth >= Console.BufferHeight) return;
			
			if (ReferenceEquals(ConsoleWindow, null) || _y != ConsoleWindow.LastCoordinates[1] || _x - 1 != ConsoleWindow.LastCoordinates[0])
				Console.SetCursorPosition(_x % Console.BufferWidth, _y + _x / Console.BufferWidth);
				
			Console.ForegroundColor = _color;
			Console.Write(_character);
			
			if (ReferenceEquals(ConsoleWindow, null)) return;
			
			ConsoleWindow.LastCoordinates = new[] { _x, _y };
		}
	}

	public class ConsoleWindow
	{
		public readonly ConsoleWindowCharacter[,] ConsoleWindowCharacters;

		// Store last coordinates to draw faster
		// ( Prevent pointless SetCursorPosition execution )
		public int[] LastCoordinates = { 0, 0 };
		
		private readonly int _width;
		private readonly int _height;
		private readonly KeyValuePair<char, ConsoleColor>[,] _scheme;

		public ConsoleWindow(int width, int height, KeyValuePair<char, ConsoleColor>[,] scheme)
		{
			_width = width;
			_height = height;
			_scheme = scheme;

			Console.Clear();
			Console.ResetColor();

			ConsoleWindowCharacters = new ConsoleWindowCharacter[width, height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					ConsoleWindowCharacters[x, y] = new ConsoleWindowCharacter(x, y, ' ');
				}
			}
			
			ReRender();
		}

		public void ReRender()
		{
			for (int y = 0; y < _scheme.GetLength(1); y++)
			{
				for (int x = 0; x < _scheme.GetLength(0); x++)
				{
					if (x >= _width || y >= _height) continue;

					ConsoleWindowCharacters[x, y].Character = _scheme[x, y].Key;
					ConsoleWindowCharacters[x, y].Color = _scheme[x, y].Value;
					ConsoleWindowCharacters[x, y].Render();
				}
			}
		}

		public void SetCharactersWindowAccessor()
		{
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					ConsoleWindowCharacters[x, y].ConsoleWindow = this;
				}
			}
		}
	}

	public abstract class DrawZone
	{
		protected readonly ConsoleWindow ConsoleWindow;
		
		protected DrawZone(ConsoleWindow consoleWindow)
		{
			ConsoleWindow = consoleWindow;
		}
		
		public abstract void ReRender();
	}
	
	public class ConsoleDrawZone : DrawZone
	{
		// Square emojis color transposition table
		private static readonly Dictionary<string, ConsoleColor> Colors = new()
		{
			{"🟥", ConsoleColor.Red},
			{"🟨", ConsoleColor.Yellow},
			{"🟩", ConsoleColor.Green},
			{"🟦", ConsoleColor.Blue},
			{"🟪", ConsoleColor.Magenta}
		};
		
		// First character of square emojis
		private const char StartColor = (char)55357;
		
		// Character used to reset the color
		private const char ResetColor = '⬜';
		
		// Character used to disable colors
		private const char IgnoreColor = '❌';
		
		// Character used to enable the color
		private const char AcknowledgeColor = '⭕';
		
		private ConsoleColor _lastColor;
		private ConsoleColor _currentColor = ConsoleColor.White;
		private bool _ignoreColors;

		private readonly int _x;
		private readonly int _y;
		private readonly int _width;
		private readonly int _height;
		private readonly int _bottomOffset;
		
		private int _cursorX;
		private readonly KeyValuePair<char, ConsoleColor>[,] _characters;
		
		public ConsoleDrawZone(ConsoleWindow consoleWindow, int x, int y, int width, int height, int bottomOffset = 0) : base(consoleWindow)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			_bottomOffset = bottomOffset;

			_characters = new KeyValuePair<char, ConsoleColor>[width, height - bottomOffset];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (j < height - bottomOffset)
						_characters[i, j] = new KeyValuePair<char, ConsoleColor>(' ', ConsoleColor.White);
					
					consoleWindow.ConsoleWindowCharacters[x + i, y + j].Character = ' ';
				}
			}
		}

		public void Write<T>(T text)
		{
			string textString = text.ToString() ?? "";
			
			for (var index = 0; index < textString.Length; index++)
			{
				char character = textString[index];
				
				if (character == '\n')
				{
					NextLine();
					continue;
				}

				if (_cursorX >= _width)
					NextLine();
				
				switch (character)
				{
					case StartColor:
						string emojiColor = textString.Substring(index, 2);
						
						if (Colors.ContainsKey(emojiColor))
						{
							if (_ignoreColors)
								_lastColor = Colors[emojiColor];
							else
								_currentColor = Colors[emojiColor];
						}
						
						++index;
						break;
					case ResetColor:
						_currentColor = ConsoleColor.White;
						break;
					case IgnoreColor:
						_lastColor = _currentColor;
						_currentColor = ConsoleColor.White;
						_ignoreColors = true;
						break;
					case AcknowledgeColor:
						_currentColor = _lastColor;
						_ignoreColors = false;
						break;
					default:
						_characters[_cursorX, _height - _bottomOffset - 1]
							= new KeyValuePair<char, ConsoleColor>(character, _currentColor);
						
						ConsoleWindow.ConsoleWindowCharacters[_x + _cursorX, _y + _height - _bottomOffset - 1].Character
							= character;
						ConsoleWindow.ConsoleWindowCharacters[_x + _cursorX, _y + _height - _bottomOffset - 1].Color
							= _currentColor;
						
						++_cursorX;
						break;
				}
			}
		}

		public void WriteLine<T>(T text)
		{
			Write($"\n{text.ToString()}");
		}
		
		private void NextLine()
		{
			_cursorX = 0;
			
			for (int y = 1; y < _height - _bottomOffset; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					_characters[x, y - 1] = new KeyValuePair<char, ConsoleColor>(_characters[x, y].Key, _characters[x, y].Value);
					
					ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y - 1].Character
						= ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Character;
					ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y - 1].Color
						= ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Color;
				}
			}

			for (int x = 0; x < _width; x++)
			{
				_characters[x, _height - _bottomOffset - 1] = new KeyValuePair<char, ConsoleColor>(' ', ConsoleColor.White);
				
				ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + _height - _bottomOffset - 1].Character
					= ' ';
			}
		}
		
		public static int StringLength(string message)
		{
			int length = 0;

			for (int i = 0; i < message.Length; i++)
			{
				switch (message[i])
				{
					case StartColor:
						++i;
						break;
					case ResetColor:
						break;
					case IgnoreColor:
						break;
					case AcknowledgeColor:
						break;
					default:
						length += message[i].ToString().Length;
						break;
				}
			}
			
			return length;
		}
		
		public static int[] StringLength2D(string message)
		{
			int maxLineLength = 0;
			int linesCount = 0;

			foreach (string line in message.Split('\n'))
			{ 
				maxLineLength = Math.Max(StringLength(line), maxLineLength);
				++linesCount;
			}

			return new []
			{
				maxLineLength,
				linesCount
			};
		}


		public override void ReRender()
		{
			for (int y = 0; y < _height; y++)
			{
				if (y < _height - _bottomOffset)
				{
					for (int x = 0; x < _width; x++)
					{
						ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Character
							= _characters[x, y].Key;
						ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Color
							= _characters[x, y].Value;
						
						ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Render();
					}
				}
				else
				{
					for (int x = 0; x < _width; x++)
					{
						ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Character
							= ' ';
						ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Color
							= ConsoleColor.White;
						
						ConsoleWindow.ConsoleWindowCharacters[_x + x, _y + y].Render();
					}
				}
			}
		}
	}

	public class Scheme
	{
		private readonly KeyValuePair<char, ConsoleColor>[,] _scheme;

		public Scheme(int width, int height, char defaultCharacter = ' ', ConsoleColor defaultColor = ConsoleColor.White)
		{
			_scheme = new KeyValuePair<char, ConsoleColor>[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					_scheme[x, y] = new KeyValuePair<char, ConsoleColor>(defaultCharacter, defaultColor);
				}
			}
		}

		public void VerticalLine(int x, int startY, int endY, char character = '|', ConsoleColor color = ConsoleColor.White)
		{
			for (int y = startY; y <= endY; y++)
			{
				_scheme[x, y] = new KeyValuePair<char, ConsoleColor>(character, color);
			}
		}
		
		public void HorizontalLine(int y, int startX, int endX, char character = '-', ConsoleColor color = ConsoleColor.White)
		{
			for (int x = startX; x <= endX; x++)
			{
				_scheme[x, y] = new KeyValuePair<char, ConsoleColor>(character, color);
			}
		}

		public void Char(int x, int y, char character, ConsoleColor color = ConsoleColor.White)
		{
			_scheme[x, y] = new KeyValuePair<char, ConsoleColor>(character, color);
		}

		public void Text(int x, int y, string text, ConsoleColor color = ConsoleColor.White)
		{
			for (int i = 0; i < text.Length; i++)
			{
				_scheme[x + i, y] = new KeyValuePair<char, ConsoleColor>(text[i], color);
			}
		}

		public KeyValuePair<char, ConsoleColor>[,] GetScheme()
		{
			return _scheme;
		}
	}
}