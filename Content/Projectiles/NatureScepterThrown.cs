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
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles
{
    public class NatureScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Green;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.JungleSpore;
            base.SetDefaults();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {

                Projectile newProjectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                Projectile.Center, Vector2.Zero, ProjectileID.SporeGas, (int)(Projectile.damage * 0.45f), 2, Main.LocalPlayer.whoAmI);
                newProjectile.friendly = true; // If it shouldn't harm the player, for example
            }
            base.AI();
        }
    }
}

