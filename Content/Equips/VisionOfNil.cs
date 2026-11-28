using BreadLibrary.Core.Graphics.Particles;
using CalamityEntropy;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Core.Weapons;
using DestroyerTest.Common;
using DestroyerTest.Common.NPC_Folder;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Content.Equips
{
    [JITWhenModsEnabled(DTCrossMod.CallOfVoidName)]
    [ExtendsFromMod(DTCrossMod.CallOfVoidName)]
    public class VisionOfNil : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 12));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            DTUtils.NoUpgradeStack[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;

            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                Item.rare = ModContent.RarityType<VoidPurple>();
            }
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
            player.buffImmune[ModContent.BuffType<VoidTouch>()] = true;
            player.GetDamage(DamageClass.Generic) += 0.21f;

            if (Main.eclipse)
            {
                player.GetDamage(DamageClass.Generic) += 0.15f;
            }
            player.GetArmorPenetration(DamageClass.Generic) += 30;
            player.endurance += 0.17f;

            Lighting.AddLight(player.Center, Color.Indigo.ToVector3() * 0.1f);

            if (player.TryGetModPlayer<VoidHeartPlayer>(out var Heart))
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

        public override void SaveData(TagCompound tag)
        {
            tag.Add("Pet", Pet);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Pet"))
            {
                Pet = tag.GetBool("Pet");
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadeHeart>()
                .AddIngredient(ItemID.StalkersQuiver)
                .AddIngredient<ThiefsPocketwatchOfEclipse>()
                .AddIngredient<HematoidVisage>()
                .AddIngredient<VoidBar>(13)
                .AddTile<VoidWellTile>()
            .Register();
        }
    }

    [JITWhenModsEnabled(DTCrossMod.CallOfVoidName, DTCrossMod.InnoVaultName)]
    [ExtendsFromMod(DTCrossMod.CallOfVoidName, DTCrossMod.InnoVaultName)]
    public class VoidHeartPlayer : ModPlayer
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
                Player.aggro -= 200;
                Player.magicQuiver = true;
                Player.GetModPlayer<CEChargePlayer>().ChargeRateMult += 0.2f;
                Player.Entropy().thiefWatch = true;

                TexRot += 0.05f * Player.direction;

                /*
                Vector2[] pos = Opus.GetEquidistantOrbitVectors(12, Player.MountedCenter, 0.01f, Opus.Sine(270, 120, 0.003f));

                for (int i = 0; i < pos.Length; i++)
                {
                    var p = PRTLoader.NewParticle<PRT_Void>(pos[i], Vector2.Zero, Color.White, 1f);
                    p.Opacity = Opus.Sine(0.55f, 0.35f, 0.003f);
                }

                Vector2[] pos2 = Opus.GetEquidistantOrbitVectors(14, Player.MountedCenter, -0.01f, Opus.Sine(310, 150, 0.003f));

                for (int i = 0; i < pos2.Length; i++)
                {
                    var p = PRTLoader.NewParticle<PRT_Void>(pos2[i], Vector2.Zero, Color.White, 1f);
                    p.Opacity = Opus.Sine(0.55f, 0.35f, 0.003f);
                }
                */

                if (Pet)
                {
                    Player.AddBuff(ModContent.BuffType<VoidHeartBuff>(), 120);
                }
            }
            else
            {
                if (Player.HasBuff<VoidHeartBuff>())
                {
                    Player.ClearBuff(ModContent.BuffType<VoidHeartBuff>());
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
                ExplosionFX.Prepare(Player.Center, Vector2.Zero, Color.Indigo, 0.1f, 0.02f, 2f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(ExplosionFX);

                BloomRingSharp Ring = new BloomRingSharp();
                Ring.Prepare(Player.Center, Vector2.Zero, Color.Indigo, 0.03f, 0.007f, 0.6f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(Ring);

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<VoidStarF>(), 5, Player.Center, 14, 4, 16, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
                ExplosionFX.Prepare(Player.Center, Vector2.Zero, Color.Indigo, 0.1f, 0.02f, 2f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(ExplosionFX);

                BloomRingSharp Ring = new BloomRingSharp();
                Ring.Prepare(Player.Center, Vector2.Zero, Color.Indigo, 0.03f, 0.007f, 0.6f, BlendState.Additive);
                ParticleEngine.ShaderParticles.Add(Ring);

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<VoidStarF>(), 3, Player.Center, 10, 4, 16, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }


        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if ((item.DamageType.CountsAsClass(DamageClass.Summon)) && Main.rand.NextBool(10) && Active)
            {
                DTCrossMod.CallofVoid_AddVoidTouch(target, 300, 1);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if ((proj.DamageType.CountsAsClass(DamageClass.Summon) || proj.arrow) && Main.rand.NextBool(10) && Active)
            {
                DTCrossMod.CallofVoid_AddVoidTouch(target, 300, 1);
            }

            if (proj.DamageType.CountsAsClass(DamageClass.Summon) && Main.rand.NextBool((int)(20 * (1 + (0.1f * Player.numMinions)))) && proj.type != ProjectileID.StardustGuardian && Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<VoidStarF>(), 6, target.Center, (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(55), 12, 16, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }

            if ((proj.DamageType.CountsAsClass(DamageClass.Throwing) || proj.DamageType.CountsAsClass(DamageClass.Ranged)) && Main.rand.NextBool(3) && Active)
            {
                SoundEngine.PlaySound(MoonlordWarnings.LaserFire with { Volume = 0.35f, PitchVariance = 0.5f }, target.Center);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<VoidStarF>(), 3, target.Center, (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(55), 12, 16, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (Active)
            {
                regen *= 1.32f;
            }
        }
    }

    public class ShadeHeartDrawLayer : PlayerDrawLayer
    {

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.dead)
            {
                return false;
            }

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