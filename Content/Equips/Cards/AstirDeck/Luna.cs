
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
	public class Luna : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.GetDamage(DamageClass.Magic) += 1.15f;

            if (player.TryGetModPlayer<LunaPlayer>(out var luna))
            {
                luna.Active = true;
            }
        }
    }

    public class LunaPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            int Tp = ModContent.ProjectileType<TinyMoon>();
            if (Player.ownedProjectileCounts[Tp] < 1 && Active)
            {
                Projectile.NewProjectile(Player.GetSource_None(), Player.Center, Vector2.Zero, Tp, (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(20), 4, Player.whoAmI);
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Active)
            {
                Player.runAcceleration *= 1.4f;
                Player.maxRunSpeed *= 2f;
            }
        }
    }
}