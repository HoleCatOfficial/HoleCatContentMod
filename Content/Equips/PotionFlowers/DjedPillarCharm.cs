using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class DjedPillarCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 96;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.07f;
            player.maxMinions += 2;
            player.lavaImmune = true;
            player.lavaRose = true;
            player.lavaMax += 600;
            player.fireWalk = true;
            player.waterWalk = true;
            player.waterWalk2 = true;
            if(player.TryGetModPlayer<DjedPillarCharmPlayer>(out DjedPillarCharmPlayer modPlayer))
            {
                modPlayer.Active = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ObsidianSkullRose, 1)
                .AddIngredient(ItemID.AnkhCharm, 1)
                .AddIngredient<FetidCrown>(1)
                .AddIngredient<BroochOfBalance>(1)
                .AddIngredient<RiftenOverloader>(1)
                .AddIngredient<SpiritBauble>(1)
                .AddIngredient(ItemID.SpiritFlame, 1)
                .AddIngredient(ItemID.OmegaBanner, 1)
                .AddIngredient(ItemID.AnkhBanner, 1)
                .AddIngredient(ItemID.SnakeBanner, 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class DjedPillarCharmPlayer : ModPlayer
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
                Player.buffImmune[BuffID.Poisoned] = true;
                Player.buffImmune[BuffID.Darkness] = true;
                Player.buffImmune[BuffID.Cursed] = true;
                Player.buffImmune[BuffID.OnFire] = true;
                Player.buffImmune[BuffID.Bleeding] = true;
                Player.buffImmune[BuffID.Confused] = true;
                Player.buffImmune[BuffID.Slow] = true;
                Player.buffImmune[BuffID.Weak] = true;
                Player.buffImmune[BuffID.Silenced] = true;
                Player.buffImmune[BuffID.BrokenArmor] = true;
                Player.buffImmune[BuffID.CursedInferno] = true;
                Player.buffImmune[BuffID.Frostburn] = true;
                Player.buffImmune[BuffID.Chilled] = true;
                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Burning] = true;
                Player.buffImmune[BuffID.Ichor] = true;
                Player.buffImmune[BuffID.Venom] = true;
                Player.buffImmune[BuffID.Blackout] = true;
                Player.buffImmune[BuffID.Electrified] = true;
                Player.buffImmune[BuffID.Rabies] = true;
                Player.buffImmune[BuffID.ShadowFlame] = true;
                Player.buffImmune[BuffID.WindPushed] = true;
                Player.buffImmune[ModContent.BuffType<SoulErosion>()] = true;
                Player.buffImmune[ModContent.BuffType<Brine>()] = true;
                Player.buffImmune[ModContent.BuffType<GalantineBurn>()] = true;
                Player.buffImmune[ModContent.BuffType<HeliouricShock>()] = true;
                Player.buffImmune[ModContent.BuffType<Muddy>()] = true;
                Player.buffImmune[ModContent.BuffType<NightInferno>()] = true;
                Player.buffImmune[ModContent.BuffType<LightInferno>()] = true;
                Player.buffImmune[ModContent.BuffType<SpiritDrift>()] = true;
                Player.noKnockback = true;
            }
        }
    }
}