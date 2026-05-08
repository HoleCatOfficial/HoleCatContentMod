using BreadLibrary.Core.Graphics;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee.Quixotism
{
    public class QuixotismSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 72;
            Projectile.height = 72;
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.Woosh;


        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            Player Owner = Main.player[Projectile.owner];
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            SwordLine = new Line(Owner.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);

            if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (Q.Powered)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        PointGlowPreMultiplied FX = new();
                        FX.Initialize(pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, new Color(255, 219, 6), 1.5f);
                        ParticleEngine.BehindProjectiles.Add(FX);

                        Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, new Color(255, 219, 6), 2f);
                    }
                    SweepColor = ColorLib.Soul2;
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, Color.White, 2f);
                    }
                    SweepColor = Color.White;
                }
            }
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            Player Owner = Main.player[Projectile.owner];
            if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (!Q.Powered)
                {
                    Q.hitCount[0]++;

                    if (Q.hitCount[0] >= 8)
                    {
                        SoundEngine.PlaySound(DTAssetLib.Charge.Quixotism, npc.Center);
                        Q.Powered = true;
                        Q.hitCount[0] = 0;
                        Q.hitCount[1] = 0;
                        Q.comboExpireTimer = 120;
                    }
                }
                else
                {
                    Q.hitCount[1]++;
                    Q.comboExpireTimer = 120;

                    SoundEngine.PlaySound(DTAssetLib.SwordSounds.Slam, npc.Center);
                    Opus.RadialSpreadDustRandom(ModContent.DustType<ColorableNeonDust>(), 7, npc.Center, 0, new Color(255, 219, 6), 1f, 3);
                    npc.AddBuff(ModContent.BuffType<SoulInferno>(), 120);

                  

                    QuixoticParticle FX = new();
                    FX.Initiate(Main.rand.NextVector2FromRectangle(npc.Hitbox));
                    ParticleEngine.BehindProjectiles.Add(FX);

                    BloomRingSharp Ring = new();
                    Ring.Prepare(npc.Center, Vector2.Zero, new Color(255, 219, 6) * 0.5f, 0.1f, 0.01f, 0.4f, BlendState.Additive);
                    ParticleEngine.BehindProjectiles.Add(Ring);

                    if (Q.hitCount[1] >= 2)
                    {
                        Q.Powered = false;
                        Q.hitCount[1] = 0;
                        Q.hitCount[0] = 0;
                    }
                }
            }
        }

        public override void DrawUnderBlade()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 origin;

            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D powertexture = DTAssetLib.QuixotismPowerAura.Value;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, powertexture.Height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(powertexture.Width, powertexture.Height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }



            if (player.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (Q.Powered)
                {
                    if (Q.PowerOpacity < 1f)
                    {
                        Q.PowerOpacity += 0.02f;
                    }
                }
                if (!Q.Powered)
                {
                    if (Q.PowerOpacity > 0f)
                    {
                        Q.PowerOpacity -= 0.02f;
                    }
                }

                Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(powertexture, Projectile.Center - Main.screenPosition, null, (Color.White * Q.PowerOpacity) * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, AdjustedScale, effects, 0);
                Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            }
        }
    }
}