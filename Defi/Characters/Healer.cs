using System.Collections.Generic;

namespace Defi.Characters
{
	class Healer : Character
	{
		// Classe du Healer permet de définir ses variables propres (attaque, vie)
		// Ainsi que sa capacité propre le soin	
		protected override string Name => "Healer";
		protected override int InitialLife => 3;
		protected override int AttackDamage => 1;
		public override KeyValuePair<string, string>[] Choices => new[]
		{
			new KeyValuePair<string, string>("Attaquer", "d'attaquer"),
			new KeyValuePair<string, string>("Défense", "de me défendre"),
			new KeyValuePair<string, string>("Se Soigner", "de me soigner")
		};
		
		
		public Healer(bool robot = false) : base(robot) { }
		

		protected override void Special(Character enemy)
		{
			if (Life < InitialLife)
				Life += 2;
		}
	}
}