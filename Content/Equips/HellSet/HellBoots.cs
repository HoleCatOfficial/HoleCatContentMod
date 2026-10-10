using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.HellSet
{
    [AutoloadEquip(EquipType.Legs)]
    public class HellBoots : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
            float h = player.height - 2;
            Rectangle below = new Rectangle((int)player.position.X, (int)(player.position.Y + h), player.width, 2);
            if (Math.Abs(player.velocity.X) > 3.75f && (player.velocity.Y <= 0))
            {
        
                PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
                Glow.Initialize(Main.rand.NextVector2FromRectangle(below), Main.rand.NextVector2Circular(1, 1), Color.OrangeRed, 0.5f);
                ParticleEngine.ShaderParticles.Add(Glow);

                PointGlowPreMultiplied Glow2 = new PointGlowPreMultiplied();
                Glow2.Initialize(Main.rand.NextVector2FromRectangle(below), Main.rand.NextVector2Circular(1, 1), Color.Orange, 0.5f);
                ParticleEngine.ShaderParticles.Add(Glow2);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellArmorScrap>(5)
                .AddIngredient(ItemID.Ectoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
