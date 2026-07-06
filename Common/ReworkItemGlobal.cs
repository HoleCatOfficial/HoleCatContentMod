using DestroyerTest.Content.Projectiles.Vanilla;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            TextureAssets.Item[ItemID.BloodButcherer] = ModContent.Request<Texture2D>(VanillaOverridePath + "/BloodButcherer");
			TextureAssets.Item[ItemID.Zenith] = TextureAssets.Projectile[ProjectileID.FinalFractal] = ModContent.Request<Texture2D>(VanillaOverridePath + "/Zenith");
		}
        public override bool InstancePerEntity => true;

        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.BloodButcherer)
            {
                BloodButchererDefaults(entity);
            }
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

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.BloodButcherer)
            {
                return player.ownedProjectileCounts[item.shoot] < 1;
            }
            return base.CanUseItem(item, player);
        }
    }
}
