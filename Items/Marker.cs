using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EvE.Items
{
	public class Marker : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Creature marker");
			DisplayName.AddTranslation(GameCulture.Chinese, "生物标记器");
			Tooltip.SetDefault("Left click to apply Mark [c/ff0000:A] to the creature, and right click to apply Mark [c/00ffff:B] to another creature\n" +
				"When Mark [c/ff0000:A] and Mark [c/00ffff:B] coexist, the two creatures will fight with each other\n" +
				"Cannot apply marks to boss minions or minor parts of boss.\n" +
				"NOTE: Cannot apply marks to Eater of World or its segments");
			Tooltip.AddTranslation(GameCulture.Chinese, "左键向生物施加标记[c/ff0000:A]，右键向另一个生物施加标记[c/00ffff:B]\n" +
				"当标记[c/ff0000:A]和标记[c/00ffff:B]共存时两生物会互相产生仇恨\n" +
				"不能施加在Boss仆从或者次要部分上。\n" +
				"注意：不能施加于世界吞噬者及其体节上");
		}

		public override void SetDefaults() 
		{
			item.width = 32;
			item.height = 32;
			item.useTime = 10;
			item.useAnimation = 10;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.value = 0;
			item.rare = ItemRarityID.Expert;
			item.UseSound = SoundID.Item44;
			item.autoReuse = true;
			item.shoot = ProjectileID.WoodenArrowFriendly;
		}
		public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int t = -1;
			foreach(NPC target in Main.npc)
            {
				if(target.active && target.GetGlobalNPC<ProjectileOwnerGNPC>().OwnerWMI == -1)
                {
					if ((target.type < NPCID.EaterofWorldsHead || target.type > NPCID.EaterofWorldsTail) && !target.friendly)
					{
						if (Contains(target,Main.MouseWorld) && target.whoAmI != EvE.EnemyA && target.whoAmI != EvE.EnemyB)
						{
							t = target.whoAmI;
							break;
						}
					}
                }
            }
			if (t != -1)
			{
				if (player.altFunctionUse == 2)
				{
					EvE.EnemyB = t;
					for (int i = 0; i < 40; i++)
					{
						float r = Main.rand.NextFloat() * MathHelper.TwoPi;
						Dust dust = Dust.NewDustDirect(Main.MouseWorld, 1, 1, MyDustId.CyanFx);
						dust.noLight = false;
						dust.noGravity = true;
						dust.velocity = r.ToRotationVector2() * 8;
						dust.scale = 2f;
					}
					string str = Language.ActiveCulture == GameCulture.Chinese ? "标记B已施加" : "Mark B Applied";
					CombatText.NewText(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 1, 1), Color.Cyan, str);
					if (EvE.EnemyA != -1)
                    {
						string str1 = Language.ActiveCulture == GameCulture.Chinese ? "大乱斗开始！" : "SUPER SMASH BEGIN!";
						CombatText.NewText(player.Hitbox, Color.Red, str1);
					}
				}
				else
				{
					EvE.EnemyA = t;
					for (int i = 0; i < 40; i++)
					{
						float r = Main.rand.NextFloat() * MathHelper.TwoPi;
						Dust dust = Dust.NewDustDirect(Main.MouseWorld, 1, 1, MyDustId.RedTorch);
						dust.noLight = false;
						dust.noGravity = true;
						dust.velocity = r.ToRotationVector2() * 8;
						dust.scale = 2f;
					}
					string str = Language.ActiveCulture == GameCulture.Chinese ? "标记A已施加" : "Mark A Applied";
					CombatText.NewText(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 1, 1), Color.Red, str);
					if (EvE.EnemyB != -1)
					{
						string str1 = Language.ActiveCulture == GameCulture.Chinese ? "大乱斗开始！" : "SUPER SMASH BEGIN!";
						CombatText.NewText(player.Hitbox, Color.Red, str1);
					}
				}
			}
			return false;
        }

		private bool Contains(NPC npc, Vector2 Pos)
		{
			int width = npc.width < 16 ? 16 : npc.width;
			int height = npc.height < 16 ? 16 : npc.height;

			if (Pos.X > npc.Center.X - width / 2 && Pos.X < npc.Center.X + width / 2)
			{
				if (Pos.Y > npc.Center.Y - height / 2 && Pos.Y < npc.Center.Y + height / 2)
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