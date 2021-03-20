using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EvE.Items
{
	public class Watcher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Watch the battle");
			DisplayName.AddTranslation(GameCulture.Chinese, "观战");
			Tooltip.SetDefault("Left click to fix your perspective on a certain creature, right click to escape.\nJust be a onlooker.");
			Tooltip.AddTranslation(GameCulture.Chinese, "左键将你的视角固定在指定生物身上，右键脱离。\n吃瓜就行了");
		}

		public override void SetDefaults() 
		{
			item.width = 32;
			item.height = 32;
			item.useTime = 60;
			item.useAnimation = 60;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.value = 0;
			item.rare = ItemRarityID.Expert;
			item.UseSound = SoundID.Item2;
			item.autoReuse = false;
			item.shoot = ProjectileID.WoodenArrowFriendly;
		}
		public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (player.altFunctionUse == 2)
			{
				player.GetModPlayer<LensPlayer>().WatcherID = -1;
			}
			else
			{
				foreach (NPC target in Main.npc)
				{
					if (target.active)
					{
						if (Contains(target,Main.MouseWorld))
						{
							player.GetModPlayer<LensPlayer>().WatcherID = target.whoAmI;
							break;
						}
					}
				}
			}
			return false;
        }

		private bool Contains(NPC npc, Vector2 Pos)
		{
			if (Pos.X > npc.Center.X - npc.width / 2 && Pos.X < npc.Center.X + npc.width / 2)
			{
				if (Pos.Y > npc.Center.Y - npc.height / 2 && Pos.Y < npc.Center.Y + npc.height / 2)
				{
					return true;
				}
			}
			return false;
		}

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}