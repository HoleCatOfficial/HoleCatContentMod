using DestroyerTest.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using GlowmaskHelper.Content;
using OpusLib;
using OpusLib.Content.Helpers;
using DestroyerTest.Content.Entities;
using Terraria.GameContent.ItemDropRules;

namespace DestroyerTest.Content.MeleeWeapons
{
    [AutoloadGlowmask]
    public class VeiledSpear : ModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(ModContent.NPCType<RiftObserver>(), ItemDropRule.Common(Type, 10));
        }

        public override void SetDefaults()
        {
            Item.width = 108;
            Item.height = 108;

            Item.rare = ModContent.RarityType<RiftRarity1>();
            Item.value = Item.sellPrice(silver: 10);

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = DTAssetLib.SwordSounds.Woosh;
            Item.autoReuse = true;

            Item.damage = 240;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<VeiledSpearProjectile>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

}

