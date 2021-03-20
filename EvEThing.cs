using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.World.Generation;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace EvE
{
	public class EvEDamageProj : GlobalProjectile
	{

		public override void PostAI(Projectile projectile)
		{
			Damage(projectile);
		}


		public void Damage(Projectile projectile)
		{
			if (projectile.aiStyle == 31 || projectile.aiStyle == 32 || (projectile.type == 434 && projectile.localAI[0] != 0f) || ((projectile.aiStyle == 137 && projectile.ai[0] != 0f)) || projectile.aiStyle == 138)
			{
				return;
			}
			if (projectile.aiStyle == 93 && projectile.ai[0] != 0f && projectile.ai[0] != 2f)
			{
				return;
			}
			if (projectile.aiStyle == 10 && projectile.localAI[1] == -1f)
			{
				return;
			}
			//仆从不行
			if (Main.projPet[projectile.type] && projectile.type != 266 && projectile.type != 407 && projectile.type != 317 && (projectile.type != 388 || projectile.ai[0] != 2f) && (projectile.type < 390 || projectile.type > 392) && (projectile.type < 393 || projectile.type > 395) && (projectile.type != 533 || projectile.ai[0] < 6f || projectile.ai[0] > 8f) && (projectile.type < 625 || projectile.type > 628) && !ProjectileLoader.MinionContactDamage(projectile))
			{
				return;
			}
			if (!ProjectileLoader.CanDamage(projectile))
			{
				return;
			}
			Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
			if (projectile.type == 85 || projectile.type == 101)
			{
				int num = 30;
				myRect.X -= num;
				myRect.Y -= num;
				myRect.Width += num * 2;
				myRect.Height += num * 2;
			}
			if (projectile.type == 188)
			{
				int num2 = 20;
				myRect.X -= num2;
				myRect.Y -= num2;
				myRect.Width += num2 * 2;
				myRect.Height += num2 * 2;
			}
			if (projectile.aiStyle == 29)
			{
				int num3 = 4;
				myRect.X -= num3;
				myRect.Y -= num3;
				myRect.Width += num3 * 2;
				myRect.Height += num3 * 2;
			}
			ProjectileLoader.ModifyDamageHitbox(projectile, ref myRect);
			if (projectile.damage > 0)
			{
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage)
					{
						bool? flag2 = ProjectileLoader.CanHitNPC(projectile, Main.npc[i]);
						if (flag2 == null || flag2.Value)
						{
							bool? flag3 = NPCLoader.CanBeHitByProjectile(Main.npc[i], projectile);
							if (flag3 == null || flag3.Value)
							{
								bool flag5 = (flag2 != null && flag2.Value) || (flag3 != null && flag3.Value);
								if ((projectile.hostile && !projectile.friendly && CheckForHit(projectile, Main.npc[i]) && !Main.npc[i].dontTakeDamage) && Main.npc[i].immune[255] == 0)                 //重点判定
								{
									bool flag6 = false;
									if (projectile.type == 11 && (Main.npc[i].type == NPCID.CorruptBunny || Main.npc[i].type == NPCID.CorruptGoldfish))
									{
										flag6 = true;
									}
									else if (projectile.type == 31 && Main.npc[i].type == NPCID.Antlion)
									{
										flag6 = true;
									}
									if (flag5)
									{
										flag6 = false;
									}
									else if (Main.npc[i].trapImmune && projectile.trap)
									{
										flag6 = true;
									}
									else if (Main.npc[i].immortal && projectile.npcProj)
									{
										flag6 = true;
									}
									if (!flag6 && (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i])))
									{
										bool flag7;
										if (Main.npc[i].type == NPCID.SolarCrawltipedeTail)
										{
											Rectangle rect = Main.npc[i].getRect();
											int num5 = 8;
											rect.X -= num5;
											rect.Y -= num5;
											rect.Width += num5 * 2;
											rect.Height += num5 * 2;
											flag7 = projectile.Colliding(myRect, rect);
										}
										else
										{
											flag7 = projectile.Colliding(myRect, Main.npc[i].getRect());
										}
										if (flag7)
										{
											if (Main.npc[i].reflectingProjectiles && projectile.CanReflect())
											{
												Main.npc[i].ReflectProjectile(projectile.whoAmI);
												return;
											}
											int num6 = projectile.damage;
											num6 *= EvE.config.ProjDamageMultiplier;
											/*
											if (Main.npc[i].HasBuff(ModContent.BuffType<FerventAdoration2>()))           //易损增加友伤
											{
												num6 = projectile.damage * 5;
											}
											*/
											if (projectile.type > 0 && ProjectileID.Sets.StardustDragon[projectile.type])
											{
												float num7 = (projectile.scale - 1f) * 100f;
												num7 = Utils.Clamp<float>(num7, 0f, 50f);
												num6 = (int)((float)num6 * (1f + num7 * 0.23f));
											}
											int num8 = Main.DamageVar((float)num6);
											bool flag8 = !projectile.npcProj && !projectile.trap;
											if (projectile.trap && NPCID.Sets.BelongsToInvasionOldOnesArmy[Main.npc[i].type])
											{
												num8 /= 2;
											}
											if ((projectile.type == 400 || projectile.type == 401 || projectile.type == 402) && Main.npc[i].type >= NPCID.EaterofWorldsHead && Main.npc[i].type <= NPCID.EaterofWorldsTail)
											{
												num8 = (int)(num8 * 0.65);
												/*
												if (projectile.penetrate > 1)
												{
													projectile.penetrate--;                //不穿透
												}
												*/
											}
											if (projectile.type == 710)
											{
												Point origin = projectile.Center.ToTileCoordinates();
												if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(12), new GenCondition[]
												{
																new Conditions.IsSolid()
												}), out Point point))
												{
													num8 = (int)(num8 * 1.5f);
												}
											}
											if (projectile.type == 504)
											{
												float num9 = (60f - projectile.ai[0]) / 2f;
												projectile.ai[0] += num9;
											}
											if (projectile.aiStyle == 3 && projectile.type != 301)
											{
												if (projectile.ai[0] == 0f)
												{
													projectile.velocity.X = -projectile.velocity.X;
													projectile.velocity.Y = -projectile.velocity.Y;
													projectile.netUpdate = true;
												}
												projectile.ai[0] = 1f;
											}
											else if (projectile.type == 582)
											{
												if (projectile.ai[0] != 0f)
												{
													projectile.direction *= -1;
												}
											}
											else if (projectile.type == 624)
											{
												float num10 = 1f;
												if (Main.npc[i].knockBackResist > 0f)
												{
													num10 = 1f / Main.npc[i].knockBackResist;
												}
												projectile.knockBack = 4f * num10;
												if (Main.npc[i].Center.X < projectile.Center.X)
												{
													projectile.direction = 1;
												}
												else
												{
													projectile.direction = -1;
												}
											}
											else if (projectile.aiStyle == 16)
											{
												if (projectile.timeLeft > 3)
												{
													projectile.timeLeft = 3;
												}
												if (Main.npc[i].position.X + (float)(Main.npc[i].width / 2) < projectile.position.X + (float)(projectile.width / 2))
												{
													projectile.direction = -1;
												}
												else
												{
													projectile.direction = 1;
												}
											}
											else if (projectile.aiStyle == 68)
											{
												if (projectile.timeLeft > 3)
												{
													projectile.timeLeft = 3;
												}
												if (Main.npc[i].position.X + (float)(Main.npc[i].width / 2) < projectile.position.X + (float)(projectile.width / 2))
												{
													projectile.direction = -1;
												}
												else
												{
													projectile.direction = 1;
												}
											}
											else if (projectile.aiStyle == 50)
											{
												if (Main.npc[i].position.X + (float)(Main.npc[i].width / 2) < projectile.position.X + (float)(projectile.width / 2))
												{
													projectile.direction = -1;
												}
												else
												{
													projectile.direction = 1;
												}
											}
											if (projectile.type == 598 || projectile.type == 636 || projectile.type == 614)
											{
												projectile.ai[0] = 1f;
												projectile.ai[1] = (float)i;
												projectile.velocity = (Main.npc[i].Center - projectile.Center) * 0.75f;
												projectile.netUpdate = true;
											}
											if (projectile.type >= 511 && projectile.type <= 513)
											{
												projectile.timeLeft = 0;
											}
											if (projectile.type == 659)
											{
												projectile.timeLeft = 0;
											}
											if (projectile.type == 524)
											{
												projectile.netUpdate = true;
												projectile.ai[0] += 50f;
											}
											if ((projectile.type == 688 || projectile.type == 689 || projectile.type == 690) && Main.npc[i].type != NPCID.DungeonGuardian && Main.npc[i].defense < 999)
											{
												num8 += Main.npc[i].defense / 2;
											}
											if (projectile.aiStyle == 39)
											{
												if (projectile.ai[1] == 0f)
												{
													projectile.ai[1] = (float)(i + 1);
													projectile.netUpdate = true;
												}
											}
											if (projectile.type == 41 && projectile.timeLeft > 1)
											{
												projectile.timeLeft = 1;
											}
											bool flag9 = false;

											if (projectile.aiStyle == 93)
											{
												if (projectile.ai[0] == 0f)
												{
													projectile.ai[1] = 0f;
													int num14 = -i - 1;
													projectile.ai[0] = (float)num14;
													projectile.velocity = Main.npc[i].Center - projectile.Center;
												}
												if (projectile.ai[0] == 2f)
												{
													num8 = (int)((double)num8 * 1.35);
												}
												else
												{
													num8 = (int)((double)num8 * 0.15);
												}
											}

											if (Main.expertMode)
											{
												if ((projectile.type == 30 || projectile.type == 28 || projectile.type == 29 || projectile.type == 470 || projectile.type == 517 || projectile.type == 588 || projectile.type == 637) && Main.npc[i].type >= NPCID.EaterofWorldsHead && Main.npc[i].type <= NPCID.EaterofWorldsTail)
												{
													num8 /= 5;
												}
												if (projectile.type == 280 && ((Main.npc[i].type >= NPCID.TheDestroyer && Main.npc[i].type <= NPCID.TheDestroyerTail) || Main.npc[i].type == NPCID.Probe))
												{
													num8 = (int)(num8 * 0.75);
												}
											}
											if (Main.netMode != NetmodeID.Server && Main.npc[i].type == NPCID.CultistBoss && projectile.type >= 0 && ProjectileID.Sets.Homing[projectile.type])
											{
												num8 = (int)((float)num8 * 0.75f);
											}
											if (projectile.type == 323 && (Main.npc[i].type == NPCID.VampireBat || Main.npc[i].type == NPCID.Vampire))
											{
												num8 *= 10;
											}
											if (projectile.type == 294)
											{
												projectile.damage = (int)(projectile.damage * 0.8);
											}

											if (projectile.type == 261)
											{
												float num21 = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
												if (num21 < 1f)
												{
													num21 = 1f;
												}
												num8 = (int)(num8 * num21 / 8f);
											}
											float num22 = projectile.knockBack;
											int hitDirection = projectile.direction;
											ProjectileLoader.ModifyHitNPC(projectile, Main.npc[i], ref num8, ref num22, ref flag9, ref hitDirection);
											NPCLoader.ModifyHitByProjectile(Main.npc[i], projectile, ref num8, ref num22, ref flag9, ref hitDirection);
											PlayerHooks.ModifyHitNPCWithProj(projectile, Main.npc[i], ref num8, ref num22, ref flag9, ref hitDirection);
											projectile.StatusNPC(i);
											if (projectile.type == 317)
											{
												projectile.ai[1] = -1f;
												projectile.netUpdate = true;
											}
											int num23;
											if (flag8)
											{
												num23 = (int)Main.npc[i].StrikeNPC(num8, num22, hitDirection, flag9, false, false);
											}
											else
											{
												num23 = (int)Main.npc[i].StrikeNPCNoInteraction(num8, num22, hitDirection, flag9, false, false);
											}
											if (Main.netMode != NetmodeID.SinglePlayer)
											{
												if (flag9)
												{
													NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, i, (float)num8, num22, (float)projectile.direction, 1, 0, 0);
												}
												else
												{
													NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, i, num8, num22, projectile.direction, 0, 0, 0);
												}
											}
											if (projectile.type >= 390 && projectile.type <= 392)
											{
												projectile.localAI[1] = 20f;
											}
											if (projectile.usesIDStaticNPCImmunity)
											{
												Main.npc[i].immune[255] = 0;
												Projectile.perIDStaticNPCImmunity[projectile.type][i] = Main.GameUpdateCount + (uint)projectile.idStaticNPCHitCooldown;
											}
											else if (projectile.type == 434)
											{
												projectile.numUpdates = 0;
											}
											else if (projectile.type == 632)
											{
												Main.npc[i].immune[255] = 5;
											}
											else if (projectile.type == 514)
											{
												Main.npc[i].immune[255] = 1;
											}
											else if (projectile.type == 595)
											{
												Main.npc[i].immune[255] = 5;
											}
											else if (projectile.type >= 625 && projectile.type <= 628)
											{
												Main.npc[i].immune[255] = 6;
											}
											else if (projectile.type == 286)
											{
												Main.npc[i].immune[255] = 5;
											}
											else if (projectile.type == 514)
											{
												Main.npc[i].immune[255] = 3;
											}
											else if (projectile.type == 443)
											{
												Main.npc[i].immune[255] = 8;
											}
											else if (projectile.type >= 424 && projectile.type <= 426)
											{
												Main.npc[i].immune[255] = 5;
											}
											else if (projectile.type == 634 || projectile.type == 635)
											{
												Main.npc[i].immune[255] = 5;
											}
											else if (projectile.type == 659)
											{
												Main.npc[i].immune[255] = 5;
											}
											else if (projectile.type == 246)
											{
												Main.npc[i].immune[255] = 7;
											}
											else if (projectile.type == 249)
											{
												Main.npc[i].immune[255] = 7;
											}
											else if (projectile.type == 190)
											{
												Main.npc[i].immune[255] = 8;
											}
											else if (projectile.type == 409)
											{
												Main.npc[i].immune[255] = 6;
											}
											else if (projectile.type == 407)
											{
												Main.npc[i].immune[255] = 20;
											}
											else if (projectile.type == 311)
											{
												Main.npc[i].immune[255] = 7;
											}
											else if (projectile.type == 582)
											{
												Main.npc[i].immune[255] = 7;
												if (projectile.ai[0] != 1f)
												{
													projectile.ai[0] = 1f;
													projectile.netUpdate = true;
												}
											}
											else
											{

												if (projectile.type == 661)
												{
													projectile.localNPCImmunity[i] = 8;
													Main.npc[i].immune[255] = 0;
												}
												else if (projectile.usesLocalNPCImmunity && projectile.localNPCHitCooldown != -2)
												{
													Main.npc[i].immune[255] = 0;
													projectile.localNPCImmunity[i] = projectile.localNPCHitCooldown;
												}
												else                  //if (projectile.penetrate != 1)        //反真伤
												{
													Main.npc[i].immune[255] = 10;
												}
											}
											if (projectile.type == 710)
											{
												BetsySharpnel(projectile, i);
											}
											ProjectileLoader.OnHitNPC(projectile, Main.npc[i], num23, num22, flag9);
											NPCLoader.OnHitByProjectile(Main.npc[i], projectile, num23, num22, flag9);
											PlayerHooks.OnHitNPCWithProj(projectile, Main.npc[i], num23, num22, flag9);
											if (projectile.penetrate > 0)
											{

												//projectile.penetrate--;         防止没了
												if (projectile.penetrate == 0)
												{
													break;
												}
											}
											if (projectile.aiStyle == 7)
											{
												projectile.ai[0] = 1f;
												projectile.damage = 0;
												projectile.netUpdate = true;
											}
											else if (projectile.aiStyle == 13)
											{
												projectile.ai[0] = 1f;
												projectile.netUpdate = true;
											}
											else if (projectile.aiStyle == 69)
											{
												projectile.ai[0] = 1f;
												projectile.netUpdate = true;
											}
											else if (projectile.type == 638 || projectile.type == 639 || projectile.type == 640)
											{
												projectile.localNPCImmunity[i] = -1;
												Main.npc[i].immune[255] = 0;
												projectile.damage = (int)(projectile.damage * 0.96);
											}
											else if (projectile.type == 617)
											{
												projectile.localNPCImmunity[i] = 8;
												Main.npc[i].immune[255] = 0;
											}
											else if (projectile.type == 656)
											{
												projectile.localNPCImmunity[i] = 8;
												Main.npc[i].immune[255] = 0;
												projectile.localAI[0] += 1f;
											}
											else if (projectile.type == 618)
											{
												projectile.localNPCImmunity[i] = 20;
												Main.npc[i].immune[255] = 0;
											}
											else if (projectile.type == 642)
											{
												projectile.localNPCImmunity[i] = 10;
												Main.npc[i].immune[255] = 0;
											}
											else if (projectile.type == 611 || projectile.type == 612)
											{
												projectile.localNPCImmunity[i] = 6;
												Main.npc[i].immune[255] = 4;
											}
											else if (projectile.type == 645)
											{
												projectile.localNPCImmunity[i] = -1;
												Main.npc[i].immune[255] = 0;
												if (projectile.ai[1] != -1f)
												{
													projectile.ai[0] = 0f;
													projectile.ai[1] = -1f;
													projectile.netUpdate = true;
												}
											}
											projectile.numHits++;
											if (projectile.type == 697)
											{
												if (projectile.ai[0] >= 42f)
												{
													projectile.localAI[1] = 1f;
												}
											}
											else if (projectile.type == 699)
											{
												SummonMonkGhast(projectile);
											}
											else if (projectile.type == 706)
											{
												projectile.damage = (int)(projectile.damage * 0.95f);
											}
										}
									}
								}
							}





						}

					}
				}
			}

		}

		private bool CheckForHit(Projectile proj, NPC npc)
		{
			if (!npc.active) return false;
			if (proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI != -1)
			{
				if (proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI == EvE.EnemyA)
				{
					if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyB)
					{
						return true;
					}
				}
				if (proj.GetGlobalProjectile<ProjectileOwnerGProj>().OwnerWMI == EvE.EnemyB)
				{
					if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyA)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void BetsySharpnel(Projectile projectile, int npcIndex)
		{
			if (projectile.ai[1] == -1f || projectile.owner != Main.myPlayer)
			{
				return;
			}
			Vector2 spinningpoint = new Vector2(0f, 6f);
			Vector2 center = projectile.Center;
			float num = 0.7853982f;
			int num2 = 5;
			float num3 = -(num * 2f) / (float)(num2 - 1);
			for (int i = 0; i < num2; i++)
			{
				int num4 = Projectile.NewProjectile(center, spinningpoint.RotatedBy(num + num3 * i, default), ProjectileID.DD2BetsyArrow, projectile.damage, projectile.knockBack, projectile.owner, 0f, -1f);
				Projectile proj = Main.projectile[num4];
				for (int j = 0; j < projectile.localNPCImmunity.Length; j++)
				{
					proj.localNPCImmunity[j] = projectile.localNPCImmunity[j];
				}
			}
		}


		private void SummonMonkGhast(Projectile projectile)
		{
			if (projectile.localAI[0] > 0f)
			{
				return;
			}
			projectile.localAI[0] = 1000f;
			List<NPC> list = new List<NPC>();
			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy(projectile, false) && projectile.Distance(npc.Center) < 800f)
				{
					list.Add(npc);
				}
			}
			Vector2 center = projectile.Center;
			Vector2 value = Vector2.Zero;
			if (list.Count > 0)
			{
				NPC npc2 = list[Main.rand.Next(list.Count)];
				center = npc2.Center;
				value = npc2.velocity;
			}
			int num = Main.rand.Next(2) * 2 - 1;
			Vector2 vector = new Vector2((float)num * (4f + (float)Main.rand.Next(3)), 0f);
			Vector2 vector2 = center + new Vector2(-(float)num * 120f, 0f);
			vector += (center + value * 15f - vector2).SafeNormalize(Vector2.Zero) * 2f;
			Projectile.NewProjectile(vector2, vector, ProjectileID.MonkStaffT2Ghast, projectile.damage, 0f, projectile.owner, 0f, 0f);
		}

	}

	public class EvEDamageNPC : GlobalNPC
	{
		public override void PostAI(NPC npc)
		{
			if (EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyA || EvE.IsOrBelongsToNPCID(npc) == EvE.EnemyB)
			{
				MeleeHit(npc);
			}
		}

		public void MeleeHit(NPC npc)
		{

			if (npc.dontTakeDamage)
			{
				return;
			}
			int specialHitSitter = 1;
			if (npc.immune[254] == 0)
			{
				int immuneTime = 30;
				Rectangle hitbox = npc.Hitbox;
				foreach (NPC attacker in Main.npc)
				{
					if (attacker.active && attacker.whoAmI != npc.whoAmI && CheckForHit(npc, attacker))//CheckRealLife(npc, attacker))
					{
						if (!attacker.friendly && attacker.damage > 0)
						{
							Rectangle hitbox2 = attacker.Hitbox;
							float damageMultiplier = 1f;
							NPC.GetMeleeCollisionData(hitbox, attacker.whoAmI, ref specialHitSitter, ref damageMultiplier, ref hitbox2);
							bool? flag = NPCLoader.CanHitNPC(attacker, npc);
							if ((flag == null || flag.Value) && hitbox.Intersects(hitbox2) && ((flag != null && flag.Value) || npc.type != NPCID.SkeletonMerchant || !NPCID.Sets.Skeletons.Contains(attacker.netID)))
							{
								int dmg = attacker.damage;
								dmg *= EvE.config.MeleeDamageMultiplier;
								/*
								if (npc.HasBuff(ModContent.BuffType<FerventAdoration2>()))
								{
									dmg *= 5;
								}
								*/
								float kb = 6f;
								int hitDirection = 1;
								if (attacker.Center.X > npc.Center.X)
								{
									hitDirection = -1;
								}
								bool crit = Main.rand.NextFloat() > 0.75f;
								NPCLoader.ModifyHitNPC(attacker, npc, ref dmg, ref kb, ref crit);
								double realDmg = npc.StrikeNPCNoInteraction(dmg, kb, hitDirection, crit, false, false);
								if (Main.netMode != NetmodeID.SinglePlayer)
								{
									NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, dmg, kb, hitDirection, 0, 0, 0);
								}
								npc.netUpdate = true;
								npc.immune[254] = immuneTime;
								NPCLoader.OnHitNPC(attacker, npc, (int)realDmg, kb, crit);

							}
						}
					}
				}
			}
		}

		public bool CheckRealLife(NPC npc1, NPC npc2)
		{
			if (npc1.realLife == -1 && npc2.realLife == -1) return true;
			if (npc1.realLife != -1 && npc2.realLife == -1)
			{
				if (npc1.realLife == npc2.whoAmI)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			if (npc1.realLife == -1 && npc2.realLife != -1)
			{
				if (npc2.realLife == npc1.whoAmI)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			if (npc1.realLife != -1 && npc2.realLife != -1)
			{
				if (npc1.realLife == npc2.realLife)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			if (npc1.type >= NPCID.EaterofWorldsHead && npc1.type <= NPCID.EaterofWorldsTail)
			{
				return false;
			}
			return true;
		}


		private bool CheckForHit(NPC npc1, NPC npc2)
		{
			if (!npc1.active || !npc2.active) return false;
			if (EvE.IsOrBelongsToNPCID(npc1) == EvE.EnemyA && EvE.IsOrBelongsToNPCID(npc2) == EvE.EnemyB)
            {
				return true;
            }
			if (EvE.IsOrBelongsToNPCID(npc2) == EvE.EnemyA && EvE.IsOrBelongsToNPCID(npc1) == EvE.EnemyB)
			{
				return true;
			}
			return false;
		}
	}
}