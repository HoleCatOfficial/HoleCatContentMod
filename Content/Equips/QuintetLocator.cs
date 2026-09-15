using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class QuintetLocator : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 320;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<VesperRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<QuintetLocatorPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Vesper>(4)
                .AddIngredient(ItemID.FallenStar, 4)
                .Register();
        }
    }

    public class QuintetLocatorPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            CerebralGeodeGenSystem Instance = ModContent.GetInstance<CerebralGeodeGenSystem>();

            if (Player.miscCounter % 90 == 0 && Active)
            {
                //Thermos Glove
                ProvidenceSpark spark1 = new(Player);
                spark1.PrepareSpark(Player.Center, Player.Center.DirectionTo(Instance.ThermosGeodePosition) * 7f, Player.Center.DirectionTo(Instance.ThermosGeodePosition).ToRotation(), Color.OrangeRed, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark1, PixelLayer.AboveTiles);

                //Zyplon Ring
                ProvidenceSpark spark2 = new(Player);
                spark2.PrepareSpark(Player.Center, Player.Center.DirectionTo(Instance.ZyplonGeodePosition) * 7f, Player.Center.DirectionTo(Instance.ZyplonGeodePosition).ToRotation(), Color.SkyBlue, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark2, PixelLayer.AboveTiles);

                //Providence
                ProvidenceSpark spark3 = new(Player);
                spark3.PrepareSpark(Player.Center, Player.Center.DirectionTo(Instance.ProvidenceGeodePosition) * 7f, Player.Center.DirectionTo(Instance.ProvidenceGeodePosition).ToRotation(), Color.Goldenrod, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark3, PixelLayer.AboveTiles);

                //The Circle
                ProvidenceSpark spark4 = new(Player);
                spark4.PrepareSpark(Player.Center, Player.Center.DirectionTo(Instance.CircleGeodePosition) * 7f, Player.Center.DirectionTo(Instance.CircleGeodePosition).ToRotation(), Color.Red, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark4, PixelLayer.AboveTiles);

                //Heavenbleed
                ProvidenceSpark spark5 = new(Player);
                spark5.PrepareSpark(Player.Center, Player.Center.DirectionTo(Instance.HeavenbleedGeodePosition) * 7f, Player.Center.DirectionTo(Instance.HeavenbleedGeodePosition).ToRotation(), Color.PaleGoldenrod, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark5, PixelLayer.AboveTiles);
            }
        }
    }
}
