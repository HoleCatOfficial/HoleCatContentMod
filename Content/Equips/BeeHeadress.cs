using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using System.IO;
using DestroyerTest.Content.Scepter;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Head)]
    public class BeeHeadress : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 28; // Width of the item
            Item.height = 12; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.White;
            Item.defense = 10; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.BeeBreastplate && legs.type == ItemID.BeeGreaves;
        }

        public override void UpdateArmorSet(Player player)
        {
            ScepterClassStats.Range += 2;
            var modPlayer = player.GetModPlayer<BeeScepterPlayer>();
            modPlayer.Active = true;
            player.setBonus = Language.GetText("Mods.DestroyerTest.Items.BeeHeadress.SetBonus").Value;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeeWax, 9)
                .AddTile(TileID.Anvils)
                .Register();

        }
    }

    public class BeeScepterPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                for (int y = 0; y < 4; y++)
                {
                    Vector2 ofst = new Vector2(100, 0);
                    Vector2 velocity = Player.Center + ofst.RotatedByRandom(MathHelper.Pi);
                    Projectile.NewProjectile(Entity.GetSource_OnHurt(hurtInfo.DamageSource), Player.Center, velocity, ProjectileID.Bee, 16, 2, Player.whoAmI);
                }
            }
            base.OnHitByNPC(npc, hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                for (int y = 0; y < 4; y++)
                {
                    Vector2 ofst = new Vector2(100, 0);
                    Vector2 velocity = Player.Center + ofst.RotatedByRandom(MathHelper.Pi);
                    Projectile.NewProjectile(Entity.GetSource_OnHurt(hurtInfo.DamageSource), Player.Center, velocity, ProjectileID.Bee, 16, 2, Player.whoAmI);
                }
            }
            base.OnHitByProjectile(proj, hurtInfo);
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Active)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && item.type != ModContent.ItemType<ScepterOfVespae>())
                {
                    for (int y = 0; y < 4; y++)
                    {
                        Projectile.NewProjectile(Entity.GetSource_ItemUse(item), position, velocity, ProjectileID.Bee, damage / 6, knockback / 2, Player.whoAmI);
                    }
                }
            }
            base.ModifyShootStats(item, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
    }
}