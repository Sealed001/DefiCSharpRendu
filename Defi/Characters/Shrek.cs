using System.Collections.Generic;

namespace Defi.Characters
{
	class Shrek : Character
	{
		// Classe de Shrek permet de définir ses variables propres (attaque, vie)
		// Ainsi que sa capacité propre le greuh
		protected override string Name => "Shrek";
		protected override int InitialLife => 4;
		protected override int AttackDamage => 1;
		public override KeyValuePair<string, string>[] Choices => new[]
		{
			new KeyValuePair<string, string>("Attaquer", "d'attaquer"),
			new KeyValuePair<string, string>("Défense", "de me défendre"),
			new KeyValuePair<string, string>("Faire son greuh", "de faire mon greuh")
		};

        private int _greuh;

        public Shrek(bool robot = false) : base(robot) { }


        protected override void Special(Character enemy)
        {
            _greuh = 2;
        }


        protected override void PreUpdate(Character enemy)
        {
	        if (_greuh <= 0) return;

	        --Damage;
	        --_greuh;
        }
    }
}