using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.player.Potion;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class ShadeInfernoRingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ShadeRingPlayer>().Active = true;
            player.GetModPlayer<ShadeRingPlayer>().BuffIndex = buffIndex;
        }
    }


    public class ShadeRingPlayer : ModPlayer
    {
        public bool Active;
        public int BuffIndex;

        public override void ResetEffects()
        {
            Active = false;
            BuffIndex = 0;
        }

        public override void PostUpdateBuffs()
        {
            if (Active)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<ShadeInfernoRing>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_Buff(BuffIndex), Player.Center, Vector2.Zero, ModContent.ProjectileType<ShadeInfernoRing>(), 0, 0, Player.whoAmI);
                }
            }
            else
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<ShadeInfernoRing>()] > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == ModContent.ProjectileType<ShadeInfernoRing>() && proj.owner == Player.whoAmI)
                        {
                            proj.Kill();
                        }
                    }
                }
            }
        }

    }
}