using System;
using Defi.Characters;

namespace Defi
{
	class Program
	{
		private static Random _random = new Random();

		private static void Statistics()
		{
			Character ia1; 
			Character ia2;

			int ia1Wins = 0;
			int ia2Wins = 0;
			
			int loopIteration;
			int ia1CharacterSelection;
			int ia2CharacterSelection;

			Console.WriteLine("Combien voulez-vous effectuer de test ?");
			while (!int.TryParse(Console.ReadLine(), out loopIteration) || loopIteration < 1)
			{
				Console.WriteLine("Merci d'entrer un nombre correcte");
			}

			Console.Write("1 - Healer\n2 - Tank\n3 - Damager\n4 - Ogre\n");

			Console.WriteLine("Choisis une classe pour l'IA 1 :");
			while (!int.TryParse(Console.ReadLine(), out ia1CharacterSelection) || ia1CharacterSelection is < 1 or > 4)
			{
				Console.WriteLine("Merci d'entrer un nombre correcte");
			}

			Console.WriteLine("Choisis une classe pour l'IA 2 :");
			while (!int.TryParse(Console.ReadLine(), out ia2CharacterSelection) || ia2CharacterSelection is < 1 or > 4)
			{
				Console.WriteLine("Merci d'entrer un nombre correcte");
			}

			--ia1CharacterSelection;
			--ia2CharacterSelection;
			
			for(int i = 0; i < loopIteration; ++i)
			{
				ia1 = ia1CharacterSelection switch
				{
					0 => new Healer(),
					1 => new Tank(),
					2 => new Damager(),
					3 => new Shrek(),
					_ => throw new ArgumentOutOfRangeException()
				};
				
				ia2 = ia2CharacterSelection switch
				{
					0 => new Healer(),
					1 => new Tank(),
					2 => new Damager(),
					3 => new Shrek(),
					_ => throw new ArgumentOutOfRangeException()
				};

				while (ia1.Alive && ia2.Alive)
				{
					ia1.Do((CharacterActions)_random.Next(3), ia2);
					ia2.Do((CharacterActions)_random.Next(3), ia1);

					Character.Update(ia1, ia2);
				}

				if (ia1.Alive)
				{
					++ia1Wins;
					continue;
				}

				if (ia2.Alive)
					++ia2Wins;
			}
			
			Console.WriteLine($"IA1: {ia1Wins} {ia1Wins / (float)loopIteration}");
			Console.WriteLine($"IA2: {ia2Wins} {ia2Wins / (float)loopIteration}");
		}
		
		private static void Main()
		{
			Console.Clear();

			Statistics();

			Console.ReadKey();

			Console.Clear();
		}
	}
}