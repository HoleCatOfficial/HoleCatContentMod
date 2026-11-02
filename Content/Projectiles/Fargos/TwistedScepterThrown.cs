using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

            if (!DTCrossMod.FargosSoulsIsLoaded || !DTCrossMod.FargosSoulsMod.TryFind<ModProjectile>("GildedSceptreProj", out var SC))
            {
                return;
            }

            if (returning)
            {
                if (Main.GameUpdateCount % 15 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, SC.Type, Projectile.damage / 2, 3f, Projectile.owner);
                }
            }
        }
    }
}

