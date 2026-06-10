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
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.Accessory;

namespace DestroyerTest.Content.Equips
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class RevenantMask : ModItem
    {


        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            //ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
                                                                  //ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
                                                                  // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 20; // Height of the item
            Item.value = Item.sellPrice(gold: 8); // How many coins the item is worth
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>(); // The rarity of the item
            Item.defense = 7; // The amount of defense the item will give when equipped
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.GetModPlayer<RevenantPlayer>();
            modPlayer.Active = true;
        }

        //Maybe I'll put this in a chest. I dunno.
    }

    public class RevenantPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (item.DamageType == ModContent.GetInstance<ScepterClass>())
            {
                scale = 1.3f;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active && proj.DamageType == ModContent.GetInstance<ScepterClass>() && proj.owner == Player.whoAmI)
            {
                if (Main.rand.NextBool(1, 3))
                {
                    int Amount = Main.rand.Next(1, 6);
                    for (int i = 0; i < Amount; i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_OnHit(target), Player.Center, new Microsoft.Xna.Framework.Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), ModContent.ProjectileType<RevenantFireball>(), 4, 1, Player.whoAmI);
                    }
                }
            }
        }
    }
}