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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ElementalScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.White;
            WidthDim = 48;
            HeightDim = 48;
            DustType = DustID.WhiteTorch;
            base.SetDefaults();
        }

        public override void PostAI()
        {
            base.PostAI();

            if (Main.rand.NextBool(3))
            {
                float RotOffset = Projectile.rotation - MathHelper.PiOver4;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(10, 0).RotatedBy(RotOffset), new Vector2(3, 0).RotatedBy(RotOffset), ModContent.ProjectileType<ElementalTrail>(), (int)(Projectile.damage / 5), 0f, Projectile.owner);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(10, 0).RotatedBy(RotOffset), new Vector2(-3, 0).RotatedBy(RotOffset), ModContent.ProjectileType<ElementalTrail>(), (int)(Projectile.damage / 5), 0f, Projectile.owner);
            }
        }

        public override bool PreDrawExtras()
        {
            float colorfade = Opus.Sine(0.1f, 0.8f, 0.1f);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, Color.White * colorfade, Projectile.rotation + MathHelper.PiOver4, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(1f, 3f), SpriteEffects.None, 0);
            return true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(40, 40);
        }
    }
}

