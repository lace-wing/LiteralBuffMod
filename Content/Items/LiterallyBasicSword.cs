using Terraria;
using Terraria.ID;
using LiteralBuffMod.Common;

namespace LiteralBuffMod.Content.Items
{
	public class LiterallyBasicSword : ModItem
	{
		public bool toBeConsumed = false;

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
			Item.maxStack = 6;
		}

		public override bool? UseItem(Player player)
		{
			LiteralSystem.battleCooldown = 0;
			LiteralSystem.battleTimer = 0;
			for (int i = 0; i < 15; i++)
			{
				LiteralSystem.activeBattle[i] = false;
            }
			return base.UseItem(player);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<LiteralAccPlayer>().literalSwordAcc = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, int.MaxValue);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}