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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ScepterOfVespaeThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Gold;
            WidthDim = 50;
            HeightDim = 50;
            DustType = DustID.Honey;
            base.SetDefaults();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Generate flying dust effect
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {

                // Create AmbientSpore projectile at the same position but with zero velocity
                Projectile newProjectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Main.rand.NextVector2FromRectangle(EnchantmentVisuals()), Vector2.Zero, ProjectileID.Bee, Projectile.damage / 4, 2, player.whoAmI);
                newProjectile.friendly = true; // If it shouldn't harm the player, for example
            }
            base.AI();
        }

    }
}

