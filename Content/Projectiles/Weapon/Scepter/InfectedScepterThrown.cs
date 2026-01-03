using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.DataStructures;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using System.IO;
using InnoVault.PRT;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using DestroyerTest.Content.Projectiles.Weapon.Magic;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class InfectedScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.InfectedGradient;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.FireworksRGB;
            DustColor = ThemeColor;
            base.SetDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawProjectileShadowsStatic(Projectile, 4, ThemeColor);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            base.AI();
            if (Main.GameUpdateCount % 3 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CursedFlamesFriendly>(), Projectile.damage / 3, 0, Projectile.owner);
            }
        }
    
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for(int y = 0; y < 6; y++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-3, -2)), ProjectileID.GoldenShowerFriendly, Projectile.damage / 3, 0, Projectile.owner);
            }
        }
    }
}

