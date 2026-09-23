
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class FrigidPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.accessory = true;
        }

        
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration<ScepterClass>() += 13f;
            player.GetModPlayer<FrigidPendantPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FrostCore, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FrigidPendantPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }
    }

    public class FrigidPendantGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.DamageType.CountsAsClass<ScepterClass>();
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[projectile.owner].GetModPlayer<FrigidPendantPlayer>().Active)
            {
                target.AddBuff(BuffID.Frostburn2, 300);
            }
        }
    }
}