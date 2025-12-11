using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.CheatItems
{
	public class ParticleTester : ModItem
	{


		public override void SetDefaults() 
        {
            Item.UseSound = SoundID.Item4;
			Item.width = 18; // The item texture's width.
			Item.height = 18; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Shoot; // The useStyle of the Item.
			Item.useTime = 60; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 60; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = false; // Whether the weapon can be used more than once automatically by holding the use button.
		}

        public override bool? UseItem(Player player)
        {
            PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], player.Center, (Main.MouseWorld - player.Center).ToRotation().ToRotationVector2() * 16, Color.Red, 4f, 60, ai2: 2);
            return true;
        }
	}
}