using System;
using System.Collections.Generic;

namespace Defi.Characters
{
	class Damager : Character
	{
		// Damager's permet de définir ses variables propres (attaque, vie)
		// Ainsi que sa capacité propre la rage
		protected override string Name => "Damager";
		protected override int InitialLife => 3;
		protected override int AttackDamage => 2;
		public override KeyValuePair<string, string>[] Choices => new[]
		{
			new KeyValuePair<string, string>("Attaquer", "d'attaquer"),
			new KeyValuePair<string, string>("Défense", "de me défendre"),
			new KeyValuePair<string, string>("Dégâts miroir", "de lancer dégâts miroir")
		};
		
		
		private bool _mirrorDamage;
		
		
		public Damager(bool robot = false) : base(robot) { }


		protected override void Special(Character enemy)
		{
			_mirrorDamage = true;
		}


		protected override void PreUpdate(Character enemy)
		{
			if (!_mirrorDamage) return;

			enemy.Damage += Math.Max(Damage, 0);
			_mirrorDamage = false;
		}
	}
}