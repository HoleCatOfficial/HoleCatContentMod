using DestroyerTest.Common;
using DestroyerTest.Content.RiftArsenal;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class RiftScepterSun : ModProjectile, IRechargeFunctionality
    {
        public bool Energized
        {
            get
            {
                return Main.player[Projectile.owner].GetModPlayer<Recharge>().Energized;
            }
        }
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Generic;
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.DrawTextureOnProj(DTAssetLib.FeatheredCircle, Projectile, Color.Black, true, Projectile.rotation, Opus.Sine(2f, 2.5f), Opus.Sine(2f, 2.5f));
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
           
            Opus.DrawTextureOnProj(DTAssetLib.FeatheredCircle, Projectile, ColorLib.LightRift3, true, Projectile.rotation, 0.4f, 0.4f);

            Opus.DrawTextureOnProj(DTAssetLib.PointGlow, Projectile, ColorLib.Rift, true, Projectile.rotation, Opus.Sine(1f, 0.7f), Opus.Sine(1f, 0.7f));

            Opus.DrawTextureOnProj(DTAssetLib.BloomRingSharp, Projectile, ColorLib.Rift, true, Projectile.rotation, Opus.Sine(0.05f, 0.08f), Opus.Sine(0.05f, 0.08f));
            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f * Projectile.direction;
            Projectile.ai[0]++;

            List<BasePRT> Arcs = new List<BasePRT>();

            if (Main.rand.NextBool(5))
            {
                Arcs.Add(PRTLoader.NewParticle(DTUtils.ElectricArcs[DTUtils.ElectricArcs.Length - 1], Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.5f));
            }
            for (int i = 0; i < Arcs.Count; i++)
            {
                Arcs[i].Position = Projectile.Center;
                Arcs[i].Rotation = 0f;
            }


            if (Projectile.ai[0] % 20 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.Center);
                if (!Energized)
                {
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftLaser>(), 3, Projectile.Center, Projectile.damage / 3, 4, 5, offset: Projectile.rotation);
                }
                else
                {
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftStarFriendly>(), 3, Projectile.Center, Projectile.damage / 3, 4, 3, offset: Projectile.rotation);
                }
            }

            if (Projectile.ai[0] % 80 == 0 && Energized)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftBolt>(), 3, Projectile.Center, Projectile.damage / 3, 4, 3, offset: Projectile.rotation);
            }
        }
    }
}