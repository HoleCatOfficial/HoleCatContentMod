
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;
using OpusLib.Content.Helpers;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Soiled : ModItem
    {
        public override void SetStaticDefaults()
        {
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.Crimslime, ItemDropRule.Common(Type, 20));
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 10;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.16f;
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.14f;
            player.GetDamage<ScepterClass>() += 0.11f;

            if (player.TryGetModPlayer<SoiledPlayer>(out var soiled))
            {
                soiled.Active = true;
            }
        }
    }

    public class SoiledPlayer : ModPlayer
    {
        public bool Active = false;

        public int Cooldown = 90;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (Cooldown > 0)
                {
                    Cooldown--;
                }
            }
        }
    }

    internal class SoiledLifesteal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[projectile.owner];
            if (Owner.TryGetModPlayer<SoiledPlayer>(out var soiledPlayer))
            {
                if (soiledPlayer.Active && soiledPlayer.Cooldown <= 0 && Main.rand.NextBool(10) && (projectile.DamageType.CountsAsClass(DamageClass.Melee) || projectile.DamageType.CountsAsClass(DamageClass.SummonMeleeSpeed) || projectile.DamageType.CountsAsClass<ScepterClass>()))
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, target.Center);
                    Projectile.NewProjectile(Projectile.GetSource_None(), target.Center, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<SoiledHeal>(), projectile.damage / 2, 15, projectile.owner, ai1: damageDone * 0.1f);
                    soiledPlayer.Cooldown = 90;
                }
            }
        }
    }

}