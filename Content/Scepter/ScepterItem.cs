using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;

namespace DestroyerTest.Content.Scepter
{
    /// <summary>
    /// All scepter items derive from this class. Certain properties such as projectiles and values can be modified, but other things like the use time will usually be universal. If a value is not overriden, it will use the one for the Enchanted Scepter.
    /// <para/> By default, the following will be true:
    /// <para/> ✪ All scepters will sell for at least 1 gold.
    /// <para/> ✪ All scepters will automatically reuse.
    /// <para/> ✪ Thrown Scepters will have their base damage set to 1.5 times the damage of the regular shots.
    /// </summary>
    public abstract class ScepterItem : ModItem
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public int AdditiveValue = Item.sellPrice();
        public int Rarity = ItemRarityID.Pink;
        public SoundStyle ShootSound = SoundID.Item25;
        public SoundStyle ThrowSound = SoundID.Item169;
        public int KB = 0;
        public int ShootDMG = 2;
        public int ThrowDMG = 0;
        public int ShootCrit = 4;
        public int ThrowCrit = 12;
        public bool ChannelingDuringShoot = false;
        public bool ChannelingDuringThrow = false;
        public int ShootID = -1;
        public int ThrowID = -1;
        public float ThrowVelocity = 15f * ScepterClassStats.ThrowSpeedModifier;
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            if (ShootID < 0) ShootID = ModContent.ProjectileType<EnchantedShot>();
            if (ThrowID < 0) ThrowID = ModContent.ProjectileType<EnchantedScepterThrown>();
            ThrowDMG = (int)(ShootDMG * 1.5f);
            Item.width = Width;
            Item.height = Height;
            Item.value = Item.sellPrice(gold: 1) + AdditiveValue;
            Item.rare = Rarity;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.DamageType = ModContent.GetInstance<ScepterClass>();
            Item.knockBack = KB;
            Item.shoot = ModContent.ProjectileType<EnchantedShot>(); // The sword as a projectile
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse == 2)
            {
                Item.staff[Type] = false;
                ThrowDefaults();
            }
            else
            {
                Item.staff[Type] = true;
                ShootDefaults();
            }
        }

        public override void HoldItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.staff[Type] = false;
                ThrowDefaults();
            }
            else
            {
                Item.staff[Type] = true;
                ShootDefaults();
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.staff[Type] = false;
                ThrowDefaults();
            }
            else
            {
                Item.staff[Type] = true;
                ShootDefaults();
            }
            return player.ownedProjectileCounts[ThrowID] < 1;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.staff[Type] = false;
                ThrowDefaults();
            }
            else
            {
                Item.staff[Type] = true;
                ShootDefaults();
            }
        }

        public virtual void ShootDefaults()
        {
            Item.shoot = ShootID;
            Item.channel = ChannelingDuringShoot;
            Item.damage = ShootDMG;
            Item.crit = ShootCrit;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.UseSound = ShootSound;
            Item.shootSpeed = 20f;
            Item.noUseGraphic = false;
        }

        public virtual void ThrowDefaults()
        {   
            ThrowVelocity = 15f * ScepterClassStats.ThrowSpeedModifier;
            Item.shoot = ThrowID;
            Item.channel = ChannelingDuringThrow;
            Item.damage = ThrowDMG;
            Item.crit = ThrowCrit;
            Item.useTime = 100;
            Item.useAnimation = 40;
            Item.UseSound = ThrowSound;
            Item.shootSpeed = ThrowVelocity;
            Item.noUseGraphic = true;
        }
        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                float animationSpeed = 8.0f;
                float progress = ((player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax);
                progress = Math.Min(progress * animationSpeed, 1.0f);

                float startAngle = MathHelper.ToRadians(180f);
                float endAngle;

                if (player.direction == 1)
                {
                    endAngle = MathHelper.ToRadians(270f);
                }
                else if (player.direction == -1)
                {
                    endAngle = MathHelper.ToRadians(90f);
                }
                else
                {
                    endAngle = startAngle;
                }

                float armRotation = MathHelper.Lerp(startAngle, endAngle, progress);

                if (progress == 1.0f)
                {
                    armRotation = endAngle;
                }

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            }
        }
    }
}