using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class ConstellationSpawner : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;

        }

        List<Vector2> ConstellationPoints = new();

        public override void OnSpawn(IEntitySource source)
        {
            if (ConstellationPoints != null)
            {
                ConstellationPoints.Add(Projectile.Center);

                for (int i = 0; i < 6; i++)
                {
                    if (i > 0)
                    {
                        ConstellationPoints.Add(ConstellationPoints[i - 1] + new Vector2(Main.rand.NextFloat(20, 160), 0).RotatedByRandom(1f));
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < ConstellationPoints.Count; i++)
            {
                if (i > 0)
                {
                    //Utilities.DrawLineBetter(Main.spriteBatch, ConstellationPoints[i - 1], ConstellationPoints[i], Color.Red, 3f);

                    Vector2 Offset = new Vector2(ConstellationPoints[i - 1].Distance(ConstellationPoints[i]) * -0.15f, 0).RotatedBy(ConstellationPoints[i - 1].DirectionTo(ConstellationPoints[i]).ToRotation());
                    Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, (ConstellationPoints[i - 1] + Offset) - Main.screenPosition, null, VisualColor with { A = 0 } * VisualOpacity, ConstellationPoints[i - 1].DirectionTo(ConstellationPoints[i]).ToRotation() - MathHelper.PiOver2, new Vector2(DTAssetLib.MiscSparkle144.Value.Width / 2, 0f), new Vector2(1f, ((ConstellationPoints[i - 1].Distance(ConstellationPoints[i])) / DTAssetLib.MiscSparkle144.Value.Height) * 1.3f), SpriteEffects.None, 0);

                }

                Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, ConstellationPoints[i] - Main.screenPosition, null, VisualColor with { A = 0 } * VisualOpacity, 0f, DTAssetLib.Star(3).Value.Size() / 2, 1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, ConstellationPoints[i] - Main.screenPosition, null, Color.White with { A = 0 } * VisualOpacity, 0f, DTAssetLib.Star(3).Value.Size() / 2, 0.5f, SpriteEffects.None, 0);


            }
            return false;
        }

        float VisualOpacity = 0f;
        Color VisualColor = Color.White;
        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].Center;
            
            for (int i = 0; i < ConstellationPoints.Count; i++)
            {
                ConstellationPoints[i] += Main.player[Projectile.owner].velocity;
            }
            VisualColor = OpusColorUtils.MultiLerp(Projectile.ai[1] / 120f, ColorLib.StellarFireColormap);

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 90)
            {
                VisualOpacity = MathHelper.Lerp(0f, 1f, Projectile.ai[0] / 90f);
            }
            else
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] < 120)
                {
                    if (Projectile.ai[1] == 1)
                    {
                        for (int i = 0; i < ConstellationPoints.Count; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), ConstellationPoints[i], Main.rand.NextVector2Circular(2f, 2f), ModContent.ProjectileType<ConstellationNeedle>(), (int)Main.player[Projectile.owner].GetTotalDamage(DamageClass.Generic).ApplyTo(70), 5, Projectile.owner);
                        }
                    }
                }
                else
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.IceMagicImpact with { PitchVariance = 0.5f }, Projectile.Center);
                    for (int i = 0; i < ConstellationPoints.Count; i++)
                    {
                        LerpingBloomRingSharp Ring = new();
                        Ring.Prepare(ConstellationPoints[i], Vector2.Zero, ColorLib.StellarFireColormap, 0.03f, 0.001f, 0.5f);
                        ParticleEngine.Particles.Add(Ring);
                    }

                    Projectile.Kill();
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

 
    }
}
