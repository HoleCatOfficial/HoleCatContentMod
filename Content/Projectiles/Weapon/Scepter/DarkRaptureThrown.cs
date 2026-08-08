using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.GameContent.Drawing;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class DarkRaptureThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.TenebrisMagenta;
            WidthDim = 56;
            HeightDim = 56;
            DustType = ModContent.DustType<ColorableNeonDust>();
            DustColor = ThemeColor;
            base.SetDefaults();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, target.Center);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<DarkRaptureExplosion>(), Projectile.damage, 10f, Projectile.owner);
        }

        
    }
}

