using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GlowmaskHelper.Content;
using Terraria.Audio;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;

namespace DestroyerTest.Content.RogueItems
{
    [AutoloadGlowmask]
    public class GalantineKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<GalantineKnifeThrown>();
            Item.width = 34;
            Item.height = 72;
            Item.UseSound = new SoundStyle("Destroyertest/Assets/Audio/SwordSounds/QuickSwing", 4) with { PitchVariance = 0.4f, MaxInstances = 0 };
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.damage = 90;
            Item.autoReuse = true;
        }

		
	}
}