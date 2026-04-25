
using DestroyerTest.Content.Projectiles.Weapon.Summon.SoulBoundWhip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
    public class SoulBoundWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<SoulBoundWhipProjectile>(), 76, 5, 5);
            Item.useAnimation = 34;
            Item.useTime = 34;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.rare = ItemRarityID.Blue;
            Item.channel = true;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override bool MeleePrefix() => true;
    }
}

