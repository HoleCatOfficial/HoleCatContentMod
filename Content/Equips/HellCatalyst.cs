using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class HellCatalyst : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 88;
            Item.maxStack = 1;
            Item.value = 40;
            Item.rare = ItemRarityID.White;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.06f;
            player.GetModPlayer<HellCatalystPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 6)
                .AddIngredient(ItemID.PixieDust, 5)
                .AddIngredient(ItemID.CrystalShard, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HellCatalystPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
           
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Active && item.DamageType == DamageClass.Ranged && (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.Flare || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.StyngerBolt))
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.ExplosiveImpactSmall with { Volume = 0.3f }, position);

                for (int i = 0; i < 4; i++)
                {
                    Fire fire = new();
                    fire.PrepareFire(position, velocity.RotatedByRandom(0.5f) * 0.3f, DTUtils.RandomDirection(2), 0.01f, ColorLib.HellFire, 0.4f, 20, FireDrawMode.Additive, PixelLayer.AbovePlayer);
                    ParticleEngine.Particles.Add(fire);
                }
                Projectile fired = Main.projectile[Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Player.whoAmI)];
                fired.extraUpdates += 1;
                return false;
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
    }
}
