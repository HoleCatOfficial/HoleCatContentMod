
using System;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OpusLib;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;

namespace DestroyerTest.Content.Equips
{
    public class GalantineIncense : ModItem
    {
        public static bool CanSpawnBlossom = false;

        public override void SetDefaults()
        {
            Item.width = 30; // Width of the item
            Item.height = 34; // Height of the item
            Item.value = Item.sellPrice(gold: 100); // How many coins the item is worth
            Item.rare = ModContent.RarityType<StellarRarity>(); // The rarity of the item
            Item.vanity = false;
            Item.accessory = true;
            Item.expertOnly = true;
            Item.expert = true;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer<GalantineIncensePlayer>(out GalantineIncensePlayer Incense))
            {
                Incense.Active = true;
            }
        }


    }
    
    public class GalantineIncensePlayer : ModPlayer
    {
        public bool Active = false;
        public float TexRot = 0f;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                TexRot += 0.05f * Player.direction;
                Player.buffImmune[ModContent.BuffType<GalantineBurn>()] = true;
                if (Player.TryGetModPlayer<WeaponImbuePlayer>(out WeaponImbuePlayer Weapon))
                {
                    Weapon.GalantineBurn = true;
                }
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<ConstitutionStarFriendly>(), 5, Player.Center, 14, 4, 6, AI2: 1, RandomOffset: true);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<ConstitutionStarFriendly>(), 3, Player.Center, 10, 4, 6, AI2: 1, RandomOffset: true);
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Active)
            {
                Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, Player.Center - Main.screenPosition, null, ColorLib.StellarColor * 0.5f, -TexRot, DTAssetLib.FireRing.Value.Size() / 2, 0.095f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, Player.Center - Main.screenPosition, null, ColorLib.StellarColor * 0.25f, -TexRot * 2, DTAssetLib.FireRing.Value.Size() / 2, 0.085f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, Player.Center - Main.screenPosition, null, ColorLib.StellarColor * 0.25f, TexRot * 1.5f, DTAssetLib.FireRing.Value.Size() / 2, 0.0805f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, Player.Center - Main.screenPosition, null, ColorLib.StellarColor * 0.7f, -TexRot * 0.5f, DTAssetLib.FireRing.Value.Size() / 2, 0.08f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(DTAssetLib.FireRing.Value, Player.Center - Main.screenPosition, null, ColorLib.StellarColor * 0.7f, TexRot, DTAssetLib.FireRing.Value.Size() / 2, 0.08f, SpriteEffects.None, 0);
                Opus.ReturnToDefaultDrawing(Main.spriteBatch);

                if (Math.Abs(Player.velocity.X) > 5f)
                {
                    int dustIndex = Dust.NewDust(Player.position, Player.width, Player.height, DustID.TintableDustLighted, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, ColorLib.StellarColor, 1.2f);
                    drawInfo.DustCache.Add(dustIndex);
                }
            }
        }
    }
}
