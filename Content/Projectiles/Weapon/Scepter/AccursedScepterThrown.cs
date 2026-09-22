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
using ReLogic.Content;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class AccursedScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.DarkMagenta;
            WidthDim = 56;
            HeightDim = 56;
            DustType = DustID.Shadowflame;


            base.SetDefaults();
        }

        public override void PostAI()
        {
            base.PostAI();


        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/HopeScabbardTele") { PitchVariance = 0.5f, MaxInstances = 0 }, target.Center);
            SoundEngine.PlaySound(DTAssetLib.ChargeBreak with { PitchVariance = 0.5f, MaxInstances = 0 }, target.Center);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<AccursedScepterChunk>(), 4, target.Center, Projectile.damage / 3, 2f, 13f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
        }
    }
}

