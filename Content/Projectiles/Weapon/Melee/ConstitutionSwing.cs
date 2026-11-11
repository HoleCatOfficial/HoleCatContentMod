using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class ConstitutionSwing : BaseBroadswordProjectile
    {
        public override string Texture => "DestroyerTest/Content/MeleeWeapons/Constitution";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 52;
            Projectile.height = 50;
            SweepColor = ColorLib.StellarFireGradientLooping();
            SweepHighlightColor = ColorLib.StellarFireGradientLooping();
            UsesDefaultSweepFX = true;
            UsesFireSweepFX = true;
            WaitTimeMultiplier = 1.5f;
            SweepScale = 1.1f;
            SwingSpeed = 0.2f;

            //Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MagicSwing with { Pitch = 0.4f, PitchVariance = 0.3f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {

            npc.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);

            for (int i = 0; i < 12; i++)
            {
                Spark spark = new();
                spark.PrepareSpark(npc.Center, npc.Center.DirectionFrom(Owner.Center).RotatedByRandom(0.2f) * (Main.rand.NextFloat(10f, 24f) * Owner.GetTotalAttackSpeed(DamageClass.Melee)), npc.Center.DirectionFrom(Owner.Center).ToRotation() + MathHelper.PiOver2, ColorLib.StellarFireColormap[Main.rand.Next(ColorLib.StellarFireColormap.Length)], 0.5f, false, 15, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(spark);
            }

            SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 0.5f }, npc.Center);
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 1.6f }, npc.Center);
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact with { MaxInstances = 0, PitchVariance = 0.4f, Volume = 1.6f }, npc.Center);
        }

        private void DrawSweepFX2()
        {
            Player player = Main.player[Projectile.owner];
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3").Value;
            var TexH = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3Highlight").Value;
            float TexBasedMod = (Projectile.Size.Length() * 0.015f);
            float rOffset = 0f;

            SpriteEffects FX = SpriteEffects.None;

            if (LastSwing == 1)
            {
                FX = SpriteEffects.FlipHorizontally;
                rOffset = MathHelper.PiOver2;
            }
            else
            {
                FX = SpriteEffects.None;
                rOffset = 0f;
            }

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, ColorLib.StellarFireGradient(SlashProgress) * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Main.EntitySpriteDraw(TexH, player.MountedCenter - Main.screenPosition, null, Color.White * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }
        public override void DrawUnderBlade()
        {
            //DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            //Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, swordTip, null, ColorLib.Wretched2, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, 2f, SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }

        public override void OnStartSwing()
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            SoundEngine.PlaySound(DTAssetLib.SwordSounds.SpiritOfJusticeSwing with { Pitch = 0.4f, PitchVariance = 0.3f }, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (toMouse * (30 * Owner.GetTotalAttackSpeed(DamageClass.Melee) * (1f + Owner.GetAdjustedItemScale(Owner.HeldItem)))), ModContent.ProjectileType<StellarFireSlash>(), Projectile.damage, 5, Projectile.owner, LastSwing);
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                //Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, ColorLib.CursedFlames * 0.5f, 3f);

                ConstitutionParticle FX = new();
                FX.Initialize(ppt[Main.rand.Next(15)], (SwordLine.GetLineRotation + (MathHelper.PiOver2 * Owner.direction)).ToRotationVector2(), Main.rand.NextFloat(0.7f, 1f), 30);
                ParticleEngine.BehindProjectiles.Add(FX);

            }

            ScaleMult = 1.25f;

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}