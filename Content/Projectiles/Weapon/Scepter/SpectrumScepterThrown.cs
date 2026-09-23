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
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class SpectrumScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Main.DiscoColor;
            WidthDim = 46;
            HeightDim = 46;
            DustType = DustID.SpectreStaff;
            DustColor = Main.DiscoColor;


            base.SetDefaults();
        }

        public override void PostAI()
        {
            base.PostAI();


        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            

            if (hit.Crit)
            {

                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/HopeScabbardTele") { PitchVariance = 0.5f, MaxInstances = 0 }, target.Center);

                Opus.RingSpreadProjectile(ModContent.ProjectileType<BlessedNodeCrystalFriendly>(), 4, target.Center, 200, Projectile.damage, 4, -18, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
         
        }
    }
}

