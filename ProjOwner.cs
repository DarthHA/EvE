using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvE
{
    public class ProjectileOwnerGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int OwnerWMI = -1;
        //private bool flag = false;

        public override bool PreAI(NPC npc)
        {
            if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyA)
            {
                if (EvE.EnemyB != -1)
                {
                    if (Main.npc[EvE.EnemyB].active && FakePlayer.Dummy2Available())
                    {
                        npc.target = EvE.FakePlayer2;
                    }
                    else
                    {
                        if (npc.target == EvE.FakePlayer2)
                        {
                            npc.target = Main.myPlayer;
                        }
                    }
                }
                else
                {
                    if (npc.target == EvE.FakePlayer2)
                    {
                        npc.target = Main.myPlayer;
                    }
                }
            }

            if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyB)
            {
                if (EvE.EnemyA != -1)
                {
                    if (Main.npc[EvE.EnemyA].active && FakePlayer.Dummy1Available())
                    {
                        npc.target = EvE.FakePlayer1;
                    }
                    else
                    {
                        if (npc.target == EvE.FakePlayer1)
                        {
                            npc.target = Main.myPlayer;
                        }
                    }
                }
                else
                {
                    if (npc.target == EvE.FakePlayer1)
                    {
                        npc.target = Main.myPlayer;
                    }
                }
            }
            return true;
        }
        public override void PostAI(NPC npc)
        {
            EvE.UpdateOwner(npc);
        }
        /*
        public override void AI(NPC npc)
        {
            if (!flag)
            {
                flag = true;
                if (OwnerWMI != -1)
                {
                    if (Main.npc[OwnerWMI].active)
                    {
                        Main.NewText(Lang.GetNPCNameValue(Main.npc[OwnerWMI].type) + " : " + Lang.GetNPCNameValue(npc.type));
                    }
                }
            }
        }

        */

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.whoAmI == EvE.EnemyA)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "A", npc.Center.X - Main.screenPosition.X, npc.Top.Y - 15 - Main.screenPosition.Y, Color.Red, Color.Black, Vector2.Zero, 1.5f);
            }
            else if(EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyA)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "A", npc.Center.X - Main.screenPosition.X, npc.Top.Y - 10 - Main.screenPosition.Y, Color.Red, Color.Black, Vector2.Zero, 0.9f);
            }

            if (npc.whoAmI == EvE.EnemyB)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "B", npc.Center.X - Main.screenPosition.X, npc.Top.Y - 15 - Main.screenPosition.Y, Color.Cyan, Color.Black, Vector2.Zero, 1.5f);
            }
            else if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyB)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "B", npc.Center.X - Main.screenPosition.X, npc.Top.Y - 10 - Main.screenPosition.Y, Color.Cyan, Color.Black, Vector2.Zero, 0.9f);
            }

        }
    }

    public class ProjectileOwnerGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int OwnerWMI = -1;
        public override void PostAI(Projectile projectile)
        {
            EvE.UpdateOwner(projectile);
        }
        //private bool flag = false;
        /*
        public override void AI(Projectile projectile)
        {
            if (!flag)
            {
                flag = true;
                if (OwnerWMI != -1)
                {
                    if (Main.npc[OwnerWMI].active)
                    {
                        Main.NewText(Lang.GetNPCNameValue(Main.npc[OwnerWMI].type) + " : " + Lang.GetProjectileName(projectile.type));
                    }
                }
            }
        }
        */
    }
}