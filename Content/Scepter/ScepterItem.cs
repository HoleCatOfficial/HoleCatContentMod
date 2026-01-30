using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using Terraria.Utilities;
using System.Collections.Generic;
using DestroyerTest.Content.Reforges;

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
            Item.useTime = 40;
            Item.useAnimation = 40;
            
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

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            Defaults_Crit(player);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            Defaults_Damage(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Defaults_ShootStats(player, ref type);
        }


        
        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 2.5f;
            }
            else
            {
                return 1f;
            }
            
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                ThrowDefaults();
            }
            else
            {
                ShootDefaults();
            }
            return player.ownedProjectileCounts[ThrowID] < 1;
        }

        public virtual void ShootDefaults()
        {
            Item.shoot = ShootID;
            Item.channel = ChannelingDuringShoot;
            Item.useAnimation = 40;
            Item.UseSound = ShootSound;
            Item.shootSpeed = 20f;
            Item.noUseGraphic = false;
        }

        public virtual void ThrowDefaults()
        {   
            ThrowVelocity = 15f * ScepterClassStats.ThrowSpeedModifier;
            Item.channel = ChannelingDuringThrow;
            Item.useAnimation = 40;
            Item.UseSound = ThrowSound;
            Item.shootSpeed = ThrowVelocity;
            Item.noUseGraphic = true;
        }

        public virtual void Defaults_ShootStats(Player player, ref int type)
        {
            if (player.altFunctionUse == 2)
            {
                type = ThrowID;
            }
            else
            {
                type = ShootID;
            }
        }
        public virtual void Defaults_Damage(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.damage = ThrowDMG;
            }
            else
            {
                Item.damage = ShootDMG;
            }
        }

        public virtual void Defaults_Crit(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.crit = ThrowCrit;
            }
            else
            {
                Item.crit = ShootCrit;
            }
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

        public override bool CanReforge()
        {
            return true;
        }

        public List<int> ScepterPrefixes = new List<int>
        {
            ModContent.PrefixType<Resolute>(),
            ModContent.PrefixType<Tainted>(),
            ModContent.PrefixType<Gilded>(),
            ModContent.PrefixType<Pure>(),
            ModContent.PrefixType<Lowly>(),
            ModContent.PrefixType<Grand>(),
        };

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return ScepterPrefixes[rand.Next(ScepterPrefixes.Count)];
        }

        public override bool WeaponPrefix()
        {
            return true;
        }
    }
} 