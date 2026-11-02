using DestroyerTest.Content.Projectiles.Weapon.Summon.RiftWhip;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftArsenal
{
    public class RiftWhip : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 34;
            Item.DefaultToWhip(ModContent.ProjectileType<RiftWhipProjectile>(), 70, 2, 10);
            Item.rare = ModContent.RarityType<HeliciteRarity>();
            Item.autoReuse = true;

        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}