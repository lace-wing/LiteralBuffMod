global using Terraria;
global using Terraria.ModLoader;
using LiteralBuffMod.Common;

namespace LiteralBuffMod
{
	public class LiteralBuffMod : Mod
	{
		public override void Load()
		{
			base.Load();

			LiteralSets.SetUpSets();
		}
	}
}