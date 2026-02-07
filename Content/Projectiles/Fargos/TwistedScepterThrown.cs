using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;

namespace DestroyerTest.Content.Projectiles.Fargos
{
    [JITWhenModsEnabled(DTCrossMod.FargosSoulsName)]
    public class TwistedScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Lavender;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.Gold;
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();
            if (returning)
            {
                if (Main.GameUpdateCount % 15 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GildedSceptreProj>(), Projectile.damage / 2, 3f, Projectile.owner);
                }
            }
        }
    }
}

