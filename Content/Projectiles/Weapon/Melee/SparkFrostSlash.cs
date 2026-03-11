using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class SparkFrostSlash : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetDefaults()
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public float MainSlashYScale = 0.5f;
        public override void PostDraw(Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, ColorLib.JavelinEnergy, Projectile.rotation, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(0.5f, MainSlashYScale), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(0.3f, MainSlashYScale * 0.8f), SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Flag3 && !Flag2;
        }

        public int BeginAttack = 49;
        public int AITimer = 0;
        public bool Flag1 = false;
        public int f = 0;
        public bool Flag2 = false;
        public bool Flag3 = false;
        public override void AI()
        {
            AITimer++;

            if (AITimer < 2)
            {
                SoundEngine.PlaySound(DTAssetLib.Charge.Quixotism with { MaxInstances = 0, PitchVariance = 1f }, Projectile.Center);
            }

            if (AITimer > BeginAttack)
            {
                if (!Flag1)
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { MaxInstances = 0, PitchVariance = 0.2f });
                    Flag1 = true;
                }

                if (!Flag3) // growth phase
                {
                    MainSlashYScale += 1f;

                    if (MainSlashYScale >= 12f)
                    {
                        MainSlashYScale = 12f;
                        Flag3 = true;
                    }
                }
                else // shrink phase
                {
                    MainSlashYScale -= 0.1f;
                    if (!Flag2)
                    {
                        Flag2 = true;
                    }

                    if (MainSlashYScale < 0f)
                    {
                        MainSlashYScale = 0f;
                        Projectile.Kill();
                    }
                }
            }
        }
    }
}
