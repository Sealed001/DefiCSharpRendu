using System;
using System.Collections.Generic;

namespace Defi.Characters
{
	// Base Class for all characters
	// Define the base variables, actions and logic
	public enum CharacterActions
	{
		Attack,
		Defend,
		Special
	}

	abstract class Character
	{
		protected abstract string Name { get; }
		protected abstract int InitialLife { get; }
		public abstract KeyValuePair<string, string>[] Choices { get; }
		protected abstract int AttackDamage { get; }
		
		
		protected readonly bool Robot;
		
		
		public bool Alive => _life != 0;
		private int _life;
		protected int Life
		{
			get => _life;
			set => _life = Math.Max(0, value);
		}
		
		
		public int Damage;
		
		
		protected Character(bool robot)
		{
			_life = InitialLife;
			Robot = robot;
		}
		public override string ToString()
		{
			return $"{(Robot ? "🟥" : "🟦")}{Name}⬜ [ 🟥{new string('♥', Math.Max(Life, 0))}{new string('_', Math.Max(InitialLife - Life, 0))}⬜ ]";
		}
		public void Do(CharacterActions characterAction, Character enemy)
		{
			switch (characterAction)
			{
				case CharacterActions.Attack:
					Attack(enemy);
					break;
				case CharacterActions.Defend:
					Defend();
					break;
				case CharacterActions.Special:
					Special(enemy);
					break;
				default:
					Attack(enemy);
					break;
			}
		}


		protected virtual void Attack(Character enemy)
		{
			enemy.Damage += AttackDamage;
		}

		private void Defend()
		{
			--Damage;
		}

		protected abstract void Special(Character enemy);

		
		public static void Update(Character player1, Character player2)
		{
			player1.PreUpdate(player2);
			player2.PreUpdate(player1);

			player1.Update();
			player2.Update();
		}

		protected virtual void PreUpdate(Character enemy) { }

		private void Update()
		{
			Life -= Math.Max(Damage, 0);
			Damage = 0;
		}
	}
}