
using DestroyerTest.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;

using DestroyerTest.Content.Particles;
using Terraria.Audio;
using System;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Entities;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class ElementalFenzimSpark : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 7f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.07f;

        float IHomingProjectile.HomingMaxAccel => 40f;

        float IHomingProjectile.DetectRadius => 4800;

        bool IHomingProjectile.CanHome => DelayTimer >= 30;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public int variant;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Throwing;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            variant = (int)Projectile.ai[0];
        }
        public override bool PreDraw(ref Color lightColor)
        {

            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            DTUtils Utility = new DTUtils();
            float opacity = Projectile.Opacity;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawTextureOnProj(DTAssetLib.PointGlow, Projectile, drawColor * opacity, true, Projectile.rotation, Scale1, Scale1);
            Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5), Projectile, Color.White * opacity, false, 0f, Scale2, Scale2);
            Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5), Projectile, Color.White * opacity, false, rot, Scale3, Scale3);


            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public float Scale1 = 0f;
        public float Scale2 = 0f;
        public float Scale3 = 0f;
        public Color drawColor;
        public int Buff;
        public int DustType = DustID.Torch;
        public float rot = 0;

        public override void AI()
        {
            rot += 0.05f;
            DelayTimer++;
            variant = (int)Projectile.ai[0];

            switch (variant)
            {
                case 0:
                    drawColor = Color.OrangeRed;
                    Buff = BuffID.OnFire3;
                    DustType = DustID.Lava;
                    break;
                case 1:
                    drawColor = Color.Purple;
                    Buff = BuffID.Venom;
                    DustType = DustID.CorruptSpray;
                    break;
                case 2:
                    drawColor = Color.DeepSkyBlue;
                    Buff = BuffID.Electrified;
                    DustType = DustID.Electric;
                    break;
                case 3:
                    drawColor = ColorLib.SoulOfLightColor;
                    Buff = ModContent.BuffType<LightInferno>();
                    DustType = ModContent.DustType<SoulOfLightDust>();
                    break;
                case 4:
                    drawColor = ColorLib.SoulOfNightColor;
                    Buff = ModContent.BuffType<NightInferno>();
                    DustType = ModContent.DustType<SoulOfNightDust>();
                    break;
            }

            //Dust FX = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 1.6f);
            //FX.noGravity = true;
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.rotation += (Projectile.velocity.Length() * 0.5f) * Projectile.direction;

            Projectile.ai[2]++;

            Scale1 = Opus.Sine(0.5f, 0.8f, 0.01f);
            Scale2 = Opus.Sine(0.1f, 0.5f, 0.2f);
            Scale3 = Opus.Sine(0.05f, 0.25f, 0.2f);

            Lighting.AddLight(Projectile.Center, drawColor.ToVector3() * Scale2);

            if (Projectile.ai[2] > 200)
            {
                Projectile.Opacity -= 0.01f;
            }

            if (DelayTimer < 30)
            {
                Projectile.velocity *= 0.9f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, target.Center);
            target.AddBuff(Buff, 300);
        }

    }

}