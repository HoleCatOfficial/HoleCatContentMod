
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class Scroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public virtual void RegisterScroll(Player player)
        {
            if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
            {
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RegisterScroll(player);
        }
    }

    public class ScrollScepterUsePlayer : ModPlayer
    {
        public bool ChristmasScroll1 = false;
        public bool ChristmasScroll2 = false;
        public bool ChristmasScroll3 = false;
        public bool CurseScroll = false;
        public bool EtherScroll = false;
        public bool FrigidScroll = false;
        public bool HandScroll = false;
        public bool HellfireScroll1 = false;
        public bool PurityScroll = false;
        public bool StarScroll = false;
        public bool TreasonScroll = false;
        public bool TurbulenceScroll = false;
        public override void ResetEffects()
        {
            ChristmasScroll1 = false;
            ChristmasScroll2 = false;
            ChristmasScroll3 = false;
            CurseScroll = false;
            EtherScroll = false;
            FrigidScroll = false;
            HandScroll = false;
            HellfireScroll1 = false;
            PurityScroll = false;
            StarScroll = false;
            TreasonScroll = false;
            TurbulenceScroll = false;
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (CurseScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(10, 10);
                            Vector2 motion = outer - position;

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                motion,
                                ModContent.ProjectileType<CurseProjectile>(),
                                damage / 3,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (EtherScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(4),
                                ProjectileID.DD2PhoenixBowShot,
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (FrigidScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(4),
                                ProjectileID.NorthPoleSpear,
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
            if (StarScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                        float ceilingLimit = target.Y;
                        if (ceilingLimit > Player.Center.Y - 200f)
                        {
                            ceilingLimit = Player.Center.Y - 200f;
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 position2 = Player.Center - new Vector2(Main.rand.NextFloat(401) * Player.direction, 600f);
                            position2.Y -= 100 * i;
                            Vector2 heading = target - position2;

                            if (heading.Y < 0f)
                            {
                                heading.Y *= -1f;
                            }

                            if (heading.Y < 20f)
                            {
                                heading.Y = 20f;
                            }

                            heading.Normalize();
                            heading *= velocity.Length();
                            heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                            Projectile Star = Projectile.NewProjectileDirect(Player.GetSource_ItemUse(item), position2, heading, ProjectileID.Starfury, damage / 2, knockback, Player.whoAmI, 0f, ceilingLimit);
                            Star.timeLeft = 600;
                        }
                    }
                }
            }
            if (TreasonScroll)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(5, 5);
                            Vector2 motion = outer - position;

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                motion,
                                ModContent.ProjectileType<TreasonScrollBomb>(),
                                damage,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
                if (TurbulenceScroll)
                {
                    if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            for (int t = 0; t < 5; t++)
                            {
                                Projectile.NewProjectile(
                                    Player.GetSource_ItemUse(item),
                                    Player.Center,
                                    velocity,
                                    ProjectileID.WeatherPainShot,
                                    damage / 2,
                                    knockback,
                                    Player.whoAmI
                                );
                            }
                        }
                    }
                }
            }
        }
    }

    public class ScrollScepterProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool IsAThrownScepter = false;
        public bool IsScepterClassButNotThrown = false;
        public bool ChristmasScroll1 = false;
        public bool ChristmasScroll2 = false;
        public bool ChristmasScroll3 = false;
        public override void SetDefaults(Projectile entity)
        {
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && entity.Name.Contains("Thrown"))
            {
                IsAThrownScepter = true;
            }
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && !entity.Name.Contains("Thrown"))
            {
                IsScepterClassButNotThrown = true;
            }
        }

        public int[] OrnamentVariants = new int[]
        {
            ProjectileID.OrnamentFriendly,
            ProjectileID.OrnamentStar
        };

        public override void AI(Projectile projectile)
        {
            if (ChristmasScroll1 && IsAThrownScepter)
            {
                if (Main.rand.NextBool(4))
                {
                    for (int t = 0; t < 9; t++)
                    {
                        Vector2 outer = projectile.Center + Main.rand.NextVector2CircularEdge(12, 12);
                        Vector2 motion = outer - projectile.Center;

                        Projectile.NewProjectile(
                            projectile.GetSource_FromAI(),
                            projectile.Center,
                            motion,
                            ProjectileID.PineNeedleFriendly,
                            projectile.damage / 4,
                            projectile.knockBack / 4,
                            projectile.owner
                        );
                    }
                }
            }
            if (ChristmasScroll2 && IsAThrownScepter)
            {
                if (Main.rand.NextBool(12))
                {
                    for (int t = 0; t < 7; t++)
                    {
                        Vector2 outer = projectile.Center + Main.rand.NextVector2CircularEdge(12, 12);
                        Vector2 motion = outer - projectile.Center;

                        Projectile.NewProjectile(
                            projectile.GetSource_FromAI(),
                            projectile.Center,
                            motion,
                            OrnamentVariants[Main.rand.Next(OrnamentVariants.Length)],
                            projectile.damage / 2,
                            projectile.knockBack / 4,
                            projectile.owner
                        );
                    }
                }
            }
            if (ChristmasScroll3 && IsAThrownScepter)
            {
                if (Main.rand.NextBool(13))
                {
                    new DTUtils().RadialSpreadProjectile(ProjectileID.Blizzard, 4, projectile.Center, (int)(projectile.damage * 1.75f), (int)projectile.knockBack, 12);
                }
            }
        }
    }
}