using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using System.Collections.Generic;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots;
using DestroyerTest.Content.Resources;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria.GameContent;

namespace DestroyerTest.Content.Scepter
{
	public class ElementalScepter : ScepterItem
	{
		public override int Width => 48;
        public override int Height => 48;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 90;
            ShootCrit = 30;
            ThrowCrit = 14;
            KB = 4;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<LightShot>();
            ThrowID = ModContent.ProjectileType<ElementalScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item60;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 2f;
            Item.useTime = 20;
            Item.useAnimation = 60;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            List<int> Options = new List<int>
            {
                ModContent.ProjectileType<CursedShot>(),
                ModContent.ProjectileType<IchorShot>(),
                ModContent.ProjectileType<FireShot>(),
                ModContent.ProjectileType<GalantineShot>(),
                ModContent.ProjectileType<IceShot>(),
                ModContent.ProjectileType<ElectricShot>(),
                ModContent.ProjectileType<RiftShot>(),
                ModContent.ProjectileType<ShadowFireShot>(),
                ModContent.ProjectileType<VenomShot>()
            };

            if (player.altFunctionUse != 2)
            {

                type = Options[Main.rand.Next(Options.Count)];
                SoundEngine.PlaySound(Item.UseSound, position);
            }
        }

        public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            lines.Append(new TooltipLine(Mod, "ElementalScepterSpecialText", " "));


            // Compute total height vanilla will consume
            float height = 0f;

            for (int i = 0; i < lines.Count; i++)
            {
                TooltipLine line = lines[i];

                Vector2 size = FontAssets.MouseText.Value.MeasureString(line.Text);
                height += size.Y;
            }

            Vector2 drawPos = new Vector2(x, y + height);

            Color[] cl = new Color[18]
            {
                new Color(5, 62, 80),
                new Color(24, 67, 97),
                new Color(26, 73, 107),
                new Color(15, 84, 125),
                new Color(25, 102, 148),
                new Color(28, 138, 204),
                new Color(0, 162, 232),
                new Color(0, 168, 218),
                new Color(0, 190, 164),
                new Color(34, 177, 76),
                new Color(22, 158, 69),
                new Color(24, 153, 135),
                new Color(20, 120, 118),
                new Color(14, 93, 82),
                new Color(12, 83, 67),
                new Color(6, 79, 57),
                new Color(6, 79, 76),
                new Color(6, 69, 79)
            };
            DTUtils.SweepColorOverString("MASTER OF THE ELEMENTS", cl, drawPos, 2f);

            return true; // vanilla still draws everything
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EmberCane>()
                .AddIngredient<FrostScepter>()
                .AddIngredient<NatureScepter>()
                .AddIngredient<ThunderScepter>()
                .AddIngredient<ShadowScepter>()
                .AddIngredient<StellarFoxScepter>()
                .AddIngredient<InfectedScepter>()
                .AddIngredient<Vesper>(16)
                .AddIngredient(ItemID.GoldBar, 18)
                .AddIngredient<LifeEcho>(100)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
} 