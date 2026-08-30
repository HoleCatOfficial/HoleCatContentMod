using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.BossSummons;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class BladeChunkProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.extraUpdates = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.StarAura.Value, Projectile.Center - Main.screenPosition, null, ColorLib.StellarFire3, Projectile.velocity.ToRotation(), DTAssetLib.StarAura.Value.Size() / 2, 1.2f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.StarAura.Value, Projectile.Center - Main.screenPosition, null, ColorLib.StellarFire2, Projectile.velocity.ToRotation(), DTAssetLib.StarAura.Value.Size() / 2, 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.StarAura.Value, Projectile.Center - Main.screenPosition, null, ColorLib.StellarFire1, Projectile.velocity.ToRotation(), DTAssetLib.StarAura.Value.Size() / 2, 0.8f, SpriteEffects.None);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            return false;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.5f;

            Projectile.rotation += 0.01f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] % 15 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item68 with { Pitch = 0.5f }, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.SpiritOfJusticeParry with { Pitch = -0.6f, Volume = 0.5f }, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                StellarPointGlow Glow = new();
                Glow.Prepare(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), 1.8f);
                ParticleEngine.BehindProjectiles.Add(Glow);
            }


            LerpingBloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.StellarFireColormap, 0.1f, 0.01f, 2f);
            ParticleEngine.BehindProjectiles.Add(Ring);

            Rectangle Shock = Utils.CenteredRectangle(Projectile.Center, new Vector2(40, 40));
            Point p1 = Shock.TopLeft().ToTileCoordinates();
            Point p2 = Shock.BottomRight().ToTileCoordinates();

            Projectile.CreateImpactExplosion(8, Projectile.Center, ref p1, ref p2, 200, out bool shockwave);
            Item.NewItem(Projectile.GetSource_Death(), Projectile.Hitbox, ModContent.ItemType<BladeChunk>());


        }
    }
}
