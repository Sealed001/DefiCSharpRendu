using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConsoleDrawings;
using ConsoleInputs;
using Defi.Characters;

namespace Defi
{
	class Program
	{
		private static Random _random;
		
		private static Character _player;
		private static Character _enemy;
		
		private static ConsoleWindow _consoleWindow;
		
		private static ConsoleDrawZone _actionsDrawZone;
		private static ConsoleDrawZone _choiceTextDrawZone;
		private static ConsoleDrawZone _choicesDrawZone;

		private static ConsoleDrawZone _playerChangesDrawZone;
		private static ConsoleDrawZone _playerStatisticsDrawZone;
		private static ConsoleDrawZone _playerAsciiDrawZone;
		
		private static ConsoleDrawZone _enemyChangesDrawZone;
		private static ConsoleDrawZone _enemyStatisticsDrawZone;
		private static ConsoleDrawZone _enemyAsciiDrawZone;

		private static void Init()
		{
			// Initialize blank window scheme
			Scheme consoleWindowScheme = new Scheme(82, 21);

			// Draw plus signs for panel superposition
			consoleWindowScheme.Char(1, 1, '+');
			consoleWindowScheme.Char(20, 1, '+');
			consoleWindowScheme.Char(39, 1, '+');
			consoleWindowScheme.Char(80, 1, '+');
			consoleWindowScheme.Char(1, 13, '+');
			consoleWindowScheme.Char(20, 13, '+');
			consoleWindowScheme.Char(39, 13, '+');
			consoleWindowScheme.Char(80, 19, '+');
			consoleWindowScheme.Char(1, 19, '+');
			consoleWindowScheme.Char(39, 19, '+');

			// Draw lines for panels
			consoleWindowScheme.HorizontalLine(1, 2, 19);
			consoleWindowScheme.HorizontalLine(1, 21, 38);
			consoleWindowScheme.HorizontalLine(1, 40, 79);
			consoleWindowScheme.HorizontalLine(13, 2, 19);
			consoleWindowScheme.HorizontalLine(13, 21, 38);
			consoleWindowScheme.HorizontalLine(19, 40, 79);
			consoleWindowScheme.HorizontalLine(19, 2, 38);

			consoleWindowScheme.VerticalLine(1, 2, 12);
			consoleWindowScheme.VerticalLine(20, 2, 12);
			consoleWindowScheme.VerticalLine(39, 2, 12);
			consoleWindowScheme.VerticalLine(80, 2, 18);
			consoleWindowScheme.VerticalLine(1, 14, 18);
			consoleWindowScheme.VerticalLine(39, 14, 18);

			// Draw help message
			consoleWindowScheme.Text(46, 20, "Appuyer sur F6 pour refaire le rendu");

			// Draw panel labels
			consoleWindowScheme.Text(7, 1, " Joueur ");
			consoleWindowScheme.Text(26, 1, " Ennemi ");
			consoleWindowScheme.Text(54, 1, " Historique ");
			consoleWindowScheme.Text(4, 13, " Possibilités ");

			// Initialize console window with the window scheme
			_consoleWindow = new ConsoleWindow(82, 21, consoleWindowScheme.GetScheme());

			// Link every console characters with the console window for faster performance
			// ( No SetCursorPosition if the last drawn console character is the one on the left )
			_consoleWindow.SetCharactersWindowAccessor();
			
			// Initialize draw zones
			_actionsDrawZone = new ConsoleDrawZone(_consoleWindow, 41, 3, 38, 15);
			_choiceTextDrawZone = new ConsoleDrawZone(_consoleWindow, 3, 15, 35, 1);
			_choicesDrawZone = new ConsoleDrawZone(_consoleWindow, 3, 17, 35, 1);
			
			_playerChangesDrawZone = new ConsoleDrawZone(_consoleWindow, 3, 3, 16, 3);
			_playerStatisticsDrawZone = new ConsoleDrawZone(_consoleWindow, 3, 6, 16, 1);
			_playerAsciiDrawZone = new ConsoleDrawZone(_consoleWindow, 7, 8, 8, 4);

			_enemyChangesDrawZone = new ConsoleDrawZone(_consoleWindow, 22, 3, 16, 3);
			_enemyStatisticsDrawZone = new ConsoleDrawZone(_consoleWindow, 22, 6, 16, 1);
			_enemyAsciiDrawZone = new ConsoleDrawZone(_consoleWindow, 26, 8, 8, 4);

			// Initialize random object
			_random = new Random();
		}

		private static void ReRenderAll()
		{
			_consoleWindow.ReRender();
			
			_actionsDrawZone.ReRender();
			_choiceTextDrawZone.ReRender();
			_choicesDrawZone.ReRender();
			
			_playerChangesDrawZone.ReRender();
			_playerStatisticsDrawZone.ReRender();
			_playerAsciiDrawZone.ReRender();

			_enemyChangesDrawZone.ReRender();
			_enemyStatisticsDrawZone.ReRender();
			_enemyAsciiDrawZone.ReRender();
		}

		private static void CharacterSelection()
		{
			// Setup choice panel
			_choiceTextDrawZone.WriteLine("Quelle classe voulez-vous jouer :");
			
			// Setup data
			int selection = 0;
			string[] characterTypes = {"🟦Healer⬜", "🟨Damager⬜", "🟥Tank⬜", "🟩Ogre⬜"};
			
			// Setup choices panel
			_choicesDrawZone.Write("\n");
			for (int i = 0; i < characterTypes.Length; i++)
			{
				_choicesDrawZone.Write($"{(i != selection ? "❌": "")}{characterTypes[i]}⭕ ");
			}

			IEnumerable <ConsoleKey> keysHandler
				= Keys.Handle(Keys.HorizontalSelectorKeys.Concat(new[] { ConsoleKey.F6 }).ToArray(), true);
			
			foreach (ConsoleKey key in keysHandler)
			{
				// Check exit key
				if (key == ConsoleKey.Enter)
					break;

				// Check ReRenderAll key
				if (key == ConsoleKey.F6)
				{
					ReRenderAll();
					continue;
				}

				// Move selected
				selection += key == ConsoleKey.LeftArrow ? -1 : 1;
				
				// Clamp selection to characterTypes limits
				selection = Math.Max(0, Math.Min(selection, characterTypes.Length - 1));
				
				// Draw choices panel
				_choicesDrawZone.Write("\n");
				for (int i = 0; i < characterTypes.Length; i++)
				{
					_choicesDrawZone.Write($"{(i != selection ? "❌": "")}{characterTypes[i]}⭕ ");
				}
			}
			
			// Select a random character type for the enemy
			int enemySelection = -1;
			while (enemySelection == -1 || enemySelection == selection)
			{
				enemySelection = _random.Next(characterTypes.Length);
			}

			// Initialize characters
			_player = selection switch
			{
				0 => new Healer(),
				1 => new Damager(),
				2 => new Tank(),
				3 => new Shrek(),
				_ => _player
			};
			
			_enemy = enemySelection switch
			{
				0 => new Healer(true),
				1 => new Damager(true),
				2 => new Tank(true),
				3 => new Shrek(true),
				_ => _enemy
			};
			
			// Clear choices
			_choiceTextDrawZone.Write("\n");
			_choicesDrawZone.Write("\n");
			
			// Write selection for player
			_actionsDrawZone.WriteLine($"\n🟦Joueur⬜: J'ai choisi de jouer un {characterTypes[selection]}.");
			
			// Display Ascii Icon for the selected character type ( player )
			switch (selection)
			{
				case 0:
					_playerAsciiDrawZone.WriteLine(Icons.Healer);
					break;
				case 1:
					_playerAsciiDrawZone.WriteLine(Icons.Damager);
					break;
				case 2:
					_playerAsciiDrawZone.WriteLine(Icons.Tank);
					break;
				case 3:
					_playerAsciiDrawZone.WriteLine(Icons.Shrek);
					break;
			}
			
			// Display Stats for the selected character type ( player )
			_playerStatisticsDrawZone.WriteLine(_player);

			// Write selection for enemy
			_actionsDrawZone.WriteLine("\n🟥Ennemi⬜: Je choisis de jouer un ...");
			Thread.Sleep(2000);
			_actionsDrawZone.WriteLine($"🟥Ennemi⬜: {characterTypes[enemySelection]} !");
			
			// Display Ascii Icon for the selected character type ( enemy )
			switch (enemySelection)
			{
				case 0:
					_enemyAsciiDrawZone.WriteLine(Icons.Healer);
					break;
				case 1:
					_enemyAsciiDrawZone.WriteLine(Icons.Damager);
					break;
				case 2:
					_enemyAsciiDrawZone.WriteLine(Icons.Tank);
					break;
				case 3:
					_enemyAsciiDrawZone.WriteLine(Icons.Shrek);
					break;
			}
			
			// Display Stats for the selected character type ( enemy )
			_enemyStatisticsDrawZone.WriteLine(_enemy);
		}

		private static void ActionSelection()
		{
			// Setup choice panel
			_choiceTextDrawZone.WriteLine("Quelle actions voulez-vous faire :");
			
			// Setup data
			int selection = 0;
			List<string> actionsTypes = new List<string>();
			foreach (KeyValuePair<string, string> action in _player.Choices)
			{
				actionsTypes.Add(action.Key);
			}
			
			List<string> actionsTypesEnemy = new List<string>();
			foreach (KeyValuePair<string, string> action in _enemy.Choices)
			{
				actionsTypesEnemy.Add(action.Key);
			}

			// Setup choices panel
			_choicesDrawZone.Write("\n");
			for (int i = 0; i < actionsTypes.Count; i++)
			{
				_choicesDrawZone.Write($"{(i != selection ? "❌": "")}{actionsTypes[i]}⭕ ");
			}
			
			IEnumerable <ConsoleKey> keysHandler
				= Keys.Handle(Keys.HorizontalSelectorKeys.Concat(new[] { ConsoleKey.F6 }).ToArray(), true);
			
			foreach (ConsoleKey key in keysHandler)
			{
				// Check exit key
				if (key == ConsoleKey.Enter)
					break;

				// Check ReRenderAll key
				if (key == ConsoleKey.F6)
				{
					ReRenderAll();
					continue;
				}

				// Move selected
				selection += key == ConsoleKey.LeftArrow ? -1 : 1;
				
				// Clamp selection to characterTypes limits
				selection = Math.Max(0, Math.Min(selection, actionsTypes.Count - 1));
				
				// Draw choices panel
				_choicesDrawZone.Write("\n");
				for (int i = 0; i < actionsTypes.Count; i++)
				{
					_choicesDrawZone.Write($"{(i != selection ? "❌": "")}{actionsTypes[i]}⭕ ");
				}
			}
			
			// Select a random action type for the enemy
			int enemySelection = _random.Next(actionsTypesEnemy.Count);
			
			// Clear choices
			_choiceTextDrawZone.Write("\n");
			_choicesDrawZone.Write("\n");
			
			// Write selection for player
			_actionsDrawZone.WriteLine($"\n🟦Joueur⬜: J'ai choisi {_player.Choices[selection].Value}.");
			
			_player.Do((CharacterActions)selection, _enemy);
			
			// Write selection for enemy
			_actionsDrawZone.WriteLine("\n🟥Ennemi⬜: Je choisis ...");
			Thread.Sleep(1000);
			_actionsDrawZone.WriteLine($"🟥Ennemi⬜: {_enemy.Choices[enemySelection].Value} !");
			
			_enemy.Do((CharacterActions)enemySelection, _player);
		}
		
		private static void Main()
		{
			Console.Clear();
			
			Init();
			
			_actionsDrawZone.WriteLine("🟪Bienvenue dans un jeu au tour par tour réalisé par Tom Rouet, Léo Slomczynski et Corentin Boblet.⬜");

			Thread.Sleep(1000);
			
			_actionsDrawZone.WriteLine("\n🟪Pour commencer, veuillez choisir une classe sur la gauche à l'aide des touches du clavier.⬜");
			
			Thread.Sleep(1000);
			
			CharacterSelection();

			while (_player.Alive && _enemy.Alive)
			{
				ActionSelection();
				
				Character.Update(_player, _enemy);
				
				_playerStatisticsDrawZone.WriteLine(_player);
				_enemyStatisticsDrawZone.WriteLine(_enemy);
			}

			if (_player.Alive)
			{
				_actionsDrawZone.WriteLine("\n🟦Joueur⬜: J'ai gagné");
				_actionsDrawZone.WriteLine("🟥Ennemi⬜: Ma vie est un lamentable échec");
			}
			else if (_enemy.Alive)
			{
				_actionsDrawZone.WriteLine("\n🟦Joueur⬜: Ma vie est un lamentable échec");
				_actionsDrawZone.WriteLine("🟥Ennemi⬜: J'ai gagné");
			}
			else
			{
				_actionsDrawZone.WriteLine("\n🟦Joueur⬜: Quelle perte de temps ... ");
				_actionsDrawZone.WriteLine("🟥Ennemi⬜: Quelle perte de temps ...");
			}
			

			Console.ReadKey();

			Console.Clear();
			Console.ResetColor();
		}
	}
}