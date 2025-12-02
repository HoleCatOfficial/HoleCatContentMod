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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class HolyScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Red;
            WidthDim = 46;
            HeightDim = 46;
            DustType = DustID.RedTorch;
            base.SetDefaults();
        }
    
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            SoundEngine.PlaySound(SoundID.Item113, Projectile.position);
            HitCount += 1;
            PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), target.Center, Vector2.Zero, Color.Red, 0.025f);
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}

