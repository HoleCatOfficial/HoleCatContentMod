using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class SparkFrostCleaverSwing : BaseBroadswordProjectileFullSwing
    {
        public override void SetStaticDefaults()
        {
            
        }

        Asset<Texture2D> ColdTex;
        Asset<Texture2D> HotTex;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 96;
            Projectile.height = 96;
            UsesDefaultSweepFX = true;
            UsesFireSweepFX = true;
            SweepColor = Color.Blue;
            SweepHighlightColor = Color.SkyBlue;
            WaitTimeMultiplier = 2f;

            ColdTex = ModContent.Request<Texture2D>($"{Texture}_Cold");
            HotTex = ModContent.Request<Texture2D>($"{Texture}_Hot");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.StandardSwing with { MaxInstances = 0, Pitch = -0.4f, PitchVariance = 0.3f };

      
        public override void DrawUnderBlade()
        {

        }

        float MaskOpacity = 0f;
        public override void DrawOverBlade()
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = Hot ? HotTex.Value : ColdTex.Value;

            //i swear to FUCKING GOD.
            //dont touch this shit.
            //FUCK ROTATIONS DUDE.

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(45f);
            }
            else
            {
                origin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(45f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale, effects, 0);
            
            Main.EntitySpriteDraw(DTAssetLib.SparkFrostCleaverMask.Value, Projectile.Center - Main.screenPosition, null, Color.White * MaskOpacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale, effects, 0);
        }

        public Vector2 swordTip;
        public Line SwordLine;

        bool Hot = false;

        int SwingCounter = 0;

        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
           
            Hot = SwingCounter % 2 == 0;

            if (MaskOpacity > 0)
            {
                MaskOpacity -= 0.05f;
            }

            if (CurrentState == State.Swing)
            {
                F1 = false;
            }

            if (Hot)
            {
                SweepColor = Color.OrangeRed;
                SweepHighlightColor = Color.Orange;
            }
            else
            {
                SweepColor = Color.Blue;
                SweepHighlightColor = Color.SkyBlue;
            }

            
            ScaleMult = 1.17f;
        }


        bool F1 = false;
        public override void BetweenSwing()
        {
            if (!F1)
            {
                SoundEngine.PlaySound(SoundID.Item28);
                MaskOpacity = 1f;
                SwingCounter++;
                F1 = true;
            }
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            Lighting.AddLight(npc.Center, SweepColor.ToVector3() * 0.8f);


            if (Hot)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { MaxInstances = 0 }, npc.Center);
                npc.AddBuff(BuffID.OnFire3, 300);
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.IceMagicImpact with { MaxInstances = 0 }, npc.Center);
                npc.AddBuff(BuffID.Frozen, 300);
            }

            for (int i = 0; i < 6; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(-2, 2), -Main.rand.NextFloat(10, 15)), 0f, SweepHighlightColor, 1f, true, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }
        }
    }
}