using System.Collections.Generic;

namespace Defi.Characters
{
	class Tank : Character
	{
		// Classe du Tank permet de définir ses variables propres (attaque, vie)
		// Ainsi que sa capacité propre le boost d'attaque
		protected override string Name => "Tank";
		protected override int InitialLife => 5;
		protected override int AttackDamage => 1 + (_attackBoost ? 1 : 0);
		public override KeyValuePair<string, string>[] Choices => new[]
		{
			new KeyValuePair<string, string>("Attaquer", "d'attaquer"),
			new KeyValuePair<string, string>("Défense", "de me défendre"),
			new KeyValuePair<string, string>("Boost d'attaque", "de prendre des produits dopants")
		};


		private bool _attackBoost;
		

		public Tank(bool robot = false) : base(robot) { }
		public override string ToString()
		{
			return $"{(Robot ? "🟥" : "🟦")}{Name}{(_attackBoost ? " +" : "")}⬜ [ 🟥{new string('♥', Life)}{new string('_', InitialLife - Life)}⬜ ]";
		}

		protected override void Attack(Character enemy)
		{
			base.Attack(enemy);
			_attackBoost = false;
		}


		protected override void Special(Character enemy)
		{
			if (_attackBoost) return;
			
			--Life;
			_attackBoost = true;
		}
	}
}