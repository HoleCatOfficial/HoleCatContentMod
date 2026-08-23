using System;
using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Ammunitions;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RangedItems
{
    public class ReanimationBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 44;
            Item.rare = ItemRarityID.White;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item5;

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 22;
            Item.knockBack = 3f;
            Item.noMelee = true;

            Item.shoot = ProjectileID.Fertilizer;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FossilOre, 8)
                .AddIngredient(ItemID.WebRope, 4)
                .AddIngredient<LifeEcho>(6)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FossilOre, 8)
                .AddIngredient(ItemID.SilkRope, 4)
                .AddIngredient<LifeEcho>(6)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FossilOre, 8)
                .AddIngredient(ItemID.VineRope, 4)
                .AddIngredient<LifeEcho>(6)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FossilOre, 8)
                .AddIngredient(ItemID.Rope, 4)
                .AddIngredient<LifeEcho>(6)
                .Register();
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, player.FindAmmoDT(AmmoID.Arrow).type, "ReanimationBowShot"), position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class ReanimationArrowGlobal : GlobalProjectile
    {
        public bool CanReanimate = false;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source != null)
            {
                if (source.Context == "ReanimationBowShot" && Main.rand.NextBool(4) && projectile.type == ModContent.ProjectileType<SpiritArrowProjectile>())
                {
                    CanReanimate = true;
                }
            }
        }
    }

    public class  ReanimationNPC : GlobalNPC
    {
        public bool Reanimated = false;
        public int ReanimationTimer = 0;

        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC entity)
        {
            UndeadEnemies.AddRange(OpusNPCDropHelper.Zombies);
            UndeadEnemies.AddRange(OpusNPCDropHelper.Skeletons);
        }

        List<int> UndeadEnemies = new()
        {
            NPCID.Zombie,
            NPCID.Skeleton,
            NPCID.UndeadMiner,
            NPCID.UndeadViking,
            NPCID.BloodZombie,
            NPCID.FaceMonster
        };

        bool F1 = false;
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.GetGlobalProjectile<ReanimationArrowGlobal>().CanReanimate && UndeadEnemies.Contains(npc.type))
            {
                Reanimated = true;
                F1 = true;
                ReanimationTimer = 600;
            }
        }

        float GlowOpacity = 0f;

        bool Release = false;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Reanimated)
            {
                if (GlowOpacity < 1f)
                {
                    GlowOpacity += 0.02f;
                }
                Release = false;
            }
            else
            {
                if (!Release && F1)
                {
                    BloomRing Ring = new();
                    Ring.Prepare(npc.Center, Vector2.Zero, ColorLib.LifeEcho, 0.05f, 0.01f, 2f, BlendState.Additive);
                    ParticleEngine.Particles.Add(Ring);

                    Release = true;
                }

                if (GlowOpacity > 0f)
                {
                    GlowOpacity -= 0.02f;
                }
            }

            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, npc.Center - screenPos, null, ColorLib.LifeEcho * GlowOpacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1.5f, SpriteEffects.None);

            return true;
        }

        public override void AI(NPC npc)
        {
            if (Reanimated)
            {
                if (ReanimationTimer > 0)
                {
                    ReanimationTimer--;
                }
                else
                {
                    Reanimated = false;
                }

                
            }
        }

        public override void OnKill(NPC npc)
        {
            Reanimated = false;
        }


    }
}