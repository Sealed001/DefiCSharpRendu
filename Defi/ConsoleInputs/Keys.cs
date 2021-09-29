using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleInputs
{
	//Handles the character and action selector with the arrow keys
	public class Keys
	{
		public static readonly ConsoleKey[] HorizontalSelectorKeys = {
			ConsoleKey.LeftArrow,
			ConsoleKey.RightArrow,
			ConsoleKey.Enter
		};
		
		public static IEnumerable<ConsoleKey> HandleAny(bool intercept = false)
		{
			while (true)
			{
				yield return Console.ReadKey(intercept).Key;
			}
		}

		public static IEnumerable<ConsoleKey> Handle(ConsoleKey[] allowedKeys, bool intercept = false)
		{
			return HandleAny(intercept).Where(allowedKeys.Contains);
		}
	}
}