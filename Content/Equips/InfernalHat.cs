using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Head)]
    [AutoloadGlowmask]
    public class InfernalHat : ModItem
    {
        public override void Load()
        {

        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 10;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>();
            Item.defense = 8;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<InfernalDress>();
        }

        public override void UpdateEquip(Player player)
        {
            Rectangle DustBox = Utils.CenteredRectangle(player.Top + new Vector2(0, 6), new Vector2(player.width.WrapToTwo(), 10));
            if (Main.rand.NextBool(4))
            {
                PixelParticlePlayer Pixel = new(player);
                Pixel.Initialize((new Vector2(((int)DustBox.TopLeft().X).WrapToTwo(), ((int)DustBox.TopLeft().Y).WrapToTwo()) + new Vector2(Main.rand.Next(DustBox.Width).WrapToTwo(), Main.rand.Next(DustBox.Height).WrapToTwo())) + new Vector2(1f, 1f), new Vector2(0f, 0.2f), new Color(255, 49, 32), 2f, 30);
                ParticleEngine.Particles.Add(Pixel);
            }

            player.GetDamage(DamageClass.Ranged) += 0.1f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<InfernalHatPlayer>().Active = true;
            player.lavaImmune = true;
            player.DefaultSetBonusText(player.armor[0]);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 2)
            .AddIngredient(ItemID.HellstoneBar, 3)
            .AddIngredient(ItemID.Obsidian, 2)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    public class InfernalHatPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                Player.AddBuff(ModContent.BuffType<InfernalBatBuff>(), 60);
            }
            else
            {
                if (Player.HasBuff<InfernalBatBuff>())
                {
                    Player.ClearBuff(ModContent.BuffType<InfernalBatBuff>());
                }
            }
        }
    }

}