
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Summon.SoulBoundWhip;
using DestroyerTest.Content.Projectiles.Weapon.Summon.WretchedWhip;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
    public class WretchedWhip : ModItem
    {
        public int HitCount = 0;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<WretchedWhipProjectile>(), 76, 1, 0.5f);
            Item.UseSound = new SoundStyle(DTAssetLib.AudioPath + "/WretchedWhip/Swing", 2) { MaxInstances = 0, PitchVariance = 0.4f };
            Item.useAnimation = 34;
            Item.useTime = 34;
            Item.shootSpeed = 8f;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.channel = true;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override bool MeleePrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (HitCount >= 5)
            { 
                if (projectile.ModProjectile is WretchedWhipProjectile wwp)
                {
                    //SoundEngine.PlaySound(DTAssetLib.SwordSounds.QuickSwing with { Pitch = -1.4f }, position);
                    wwp.PowerStrike = true;
                }
                HitCount = 0;
            }
            return false;
        }
    }
}

