using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Projectiles.Vanilla;
using DestroyerTest.Content.Scepter;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ReworkItemGlobal : GlobalItem
    {
        public static string VanillaOverridePath = "DestroyerTest/Content/Extras/VanillaTextures";
        public override void Load()
        {

        }
        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            if (DTVanillaChangesConfig.instance.Resprites)
            {
                TextureAssets.Item[ItemID.BloodButcherer] = ModContent.Request<Texture2D>(VanillaOverridePath + "/BloodButcherer");
                TextureAssets.Item[ItemID.Zenith] = TextureAssets.Projectile[ProjectileID.FinalFractal] = ModContent.Request<Texture2D>(VanillaOverridePath + "/Zenith");
            }

            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[ItemID.BeeKeeper] = ModContent.ItemType<ScepterOfVespae>();
            }

            ItemID.Sets.ShimmerTransformToItem[ItemID.NaturesGift] = ItemID.JungleRose;
            ItemID.Sets.ShimmerTransformToItem[ItemID.JungleRose] = ItemID.NaturesGift;
        }
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.BloodButcherer)
            {
                BloodButchererDefaults(entity);
            }
            if (entity.type == ItemID.FlaskofFire)
            {
                FlaskOfFireDefaults(entity);
            }
            if (entity.type == ItemID.BeamSword)
            {
                BeamSwordDefaults(entity);
            }
        }

        private void FlaskOfFireDefaults(Item item)
        {
            item.UseSound = SoundID.Item3;
            item.useStyle = ItemUseStyleID.DrinkLiquid;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.maxStack = Item.CommonMaxStack;
            item.consumable = true;
            item.width = 14;
            item.height = 24;
            item.buffType = ModContent.BuffType<WeaponImbueHellfire>();
            item.buffTime = 72000;
            item.value = Item.sellPrice(0, 0, 5);
            item.rare = ItemRarityID.LightRed;
        }

        private void BloodButchererDefaults(Item item)
        {
            item.width = 60;
            item.height = 64;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useTime = 60;
            item.useAnimation = 60;
            item.autoReuse = true;

            item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            item.damage = 28;
            item.knockBack = 6.5f;
            item.crit = 4;

            item.value = Item.buyPrice(gold: 16);
            item.rare = ItemRarityID.Blue;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<BloodButchererSwing>();
            item.channel = true;
        }

        private void BeamSwordDefaults(Item item)
        {
            item.rare = ItemRarityID.LightRed;
            item.useStyle = ItemUseStyleID.Shoot;
            item.damage = 60;
            item.useAnimation = 20;
            item.useTime = 35;
            item.autoReuse = true;
            item.channel = true;
            item.width = 30;
            item.height = 30;
            item.shoot = ModContent.ProjectileType<BeamSwordSwing>();
            item.shootSpeed = 0;
            item.knockBack = 6.5f;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.DamageType = DamageClass.Melee;
            item.value = Item.sellPrice(0, 3);
            item.ChangePlayerDirectionOnShoot = true;
            item.useTurn = true;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.BloodButcherer || item.type == ItemID.BeamSword)
            {
                return player.ownedProjectileCounts[item.shoot] < 1;
            }
            return base.CanUseItem(item, player);
        }
    }
}
