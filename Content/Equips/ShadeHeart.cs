
using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadGlowmask]
    public class ShadeHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 9));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            DTUtils.NoUpgradeStack.Add(Type);
        }
        
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;

            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.expertOnly = true;
            Item.expert = true;
        }

        public bool Pet = true;

        public override bool CanRightClick()
        {
            bool ShiftKey = (Main.keyState.IsKeyDown(Keys.LeftShift) && Main.oldKeyState.IsKeyDown(Keys.LeftShift)) || (Main.keyState.IsKeyDown(Keys.RightShift) && Main.oldKeyState.IsKeyDown(Keys.RightShift));
            return ShiftKey;
        }

        int SwitchTime = 0;
        public override void RightClick(Player player)
        {
            if (SwitchTime <= 0)
            {
                if (Pet)
                {
                    Pet = false;
                    SoundEngine.PlaySound(SoundID.Item20);
                    SwitchTime = 60;
                }
                else
                {
                    Pet = true;
                    SoundEngine.PlaySound(SoundID.Item20);
                    SwitchTime = 60;
                }
            }
            else
            {
                SwitchTime--;
            }
        }
        public override bool ConsumeItem(Player player)
        {
            return false;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<ShimmeringFlames>()] = true;
            player.GetDamage(DamageClass.Generic) += 0.22f;
            player.GetArmorPenetration(DamageClass.Melee) += 20;
            player.GetArmorPenetration(DamageClass.SummonMeleeSpeed) += 20;
            player.endurance += 0.185f;

            Lighting.AddLight(player.Center, ColorLib.TenebrisGradient.ToVector3() * 0.1f);

            if (player.TryGetModPlayer<ShadeHeartPlayer>(out var Heart))
            {
                Heart.Active = true;
                if (Pet)
                {
                    Heart.Pet = true;
                }
                else
                {
                    Heart.Pet = false;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowOrb)
                .AddIngredient<Tenebris>(12)
                .AddIngredient<GalantineIncense>()
                .AddIngredient<StarFangNecklace>()
                .AddIngredient<LuminantMedallion>()
                .AddIngredient(ItemID.WormScarf)
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddIngredient(ItemID.SoulofSight, 8)
                .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }

    public class ShadeHeartPlayer : ModPlayer
    {
        public bool Active = false;
        public bool Pet = false;
        public float TexRot = 0f;
        public override void ResetEffects()
        {
            Active = false;
            Pet = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                TexRot += 0.05f * Player.direction;

                Spark Spark = new Spark();

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -5f).RotatedByRandom(0.05f), 0f, ColorLib.TenebrisGradient * 0.25f, 0.6f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.ShaderParticles.Add(Spark);

                if (Pet)
                {
                    Player.AddBuff(ModContent.BuffType<ShadeHeartPetBuff>(), 120);
                }
            }
            else
            {
                if (Player.HasBuff<ShadeHeartPetBuff>())
                {
                    Player.ClearBuff(ModContent.BuffType<ShadeHeartPetBuff>());
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
                ExplosionFX.Prepare(Player.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.1f, 0.02f, 2f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(ExplosionFX);

                BloomRingSharp Ring = new BloomRingSharp();
                Ring.Prepare(Player.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.03f, 0.007f, 0.6f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(Ring);

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 5, Player.Center, 14, 4, 16, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
                ExplosionFX.Prepare(Player.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.1f, 0.02f, 2f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(ExplosionFX);

                BloomRingSharp Ring = new BloomRingSharp();
                Ring.Prepare(Player.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.03f, 0.007f, 0.6f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(Ring);

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 3, Player.Center, 10, 4, 16, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }


        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (item.DamageType == DamageClass.Summon && Main.rand.NextBool(10) && Active)
            {
                ShimmeringFlames.ShimmerBurn(target);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (proj.DamageType == DamageClass.Summon && Main.rand.NextBool(10) && Active)
            {
                ShimmeringFlames.ShimmerBurn(target);
            }

            if (proj.DamageType == DamageClass.Summon && Main.rand.NextBool((int)(20 * (1 + (0.1f * Player.numMinions)))) && proj.type != ProjectileID.StardustGuardian && Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 6, target.Center, (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(40), 12, 16);
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (Active)
            {
                regen *= 1.4f;
            }
        }
    }

    public class ShadeHeartDrawLayer : PlayerDrawLayer
    {

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.TryGetModPlayer<ShadeHeartPlayer>(out var Heart))
            {
                if (Heart.Active)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FrozenOrWebbedDebuff);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.TryGetModPlayer<ShadeHeartPlayer>(out var Heart))
            {
                if (Heart.Active && drawInfo.shadow == 0)
                {
                    DrawRingOfFire(ref drawInfo, 0.25f, 1f, -Heart.TexRot);
                    DrawRingOfFire(ref drawInfo, 0.125f, 1f, -Heart.TexRot * 2);
                    DrawRingOfFire(ref drawInfo, 0.125f, 1.5f, Heart.TexRot * 1.5f);
                    DrawRingOfFire(ref drawInfo, 0.35f, 1.28f, -Heart.TexRot * 0.5f);
                    DrawRingOfFire(ref drawInfo, 0.35f, 1.08f, Heart.TexRot);
                }
            }

        }

        private void DrawRingOfFire(ref PlayerDrawSet drawInfo, float Opacity = 1f, float Scale = 1f, float Rotation = 0f)
        {
            var Tex = DTAssetLib.AuraRing.Value;

            var position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);

            drawInfo.DrawDataCache.Add(new DrawData(
                Tex,
                position,
                null,
                ColorLib.TenebrisGradient with { A = 0 } * Opacity,
                Rotation,
                Tex.Size() * 0.5f,
                Scale,
                SpriteEffects.None,
                0
            ));
        }

    }
}