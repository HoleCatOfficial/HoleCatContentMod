using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Sola : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 1;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<SolaPlayer>(out var sola))
            {
                sola.Active = true;
            }
        }
    }

    public class SolaPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            int Tp = ModContent.ProjectileType<TinySun>();
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
                Player.maxRunSpeed *= 1.1f;
            }
        }
    }
}
