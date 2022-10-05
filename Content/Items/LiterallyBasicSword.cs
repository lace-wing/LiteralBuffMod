using Terraria;
using Terraria.ID;

namespace LiteralBuffMod.Content.Items
{
	public class LiterallyBasicSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("LiterallyBasicSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("This is a literally basic modded sword.");
		}

		public override void SetDefaults()
		{
			Item.damage = 500;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, int.MaxValue);
			recipe.AddTile(TileID.CrystalBall);
			recipe.Register();
		}
	}
}