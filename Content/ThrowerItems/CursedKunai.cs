using DestroyerTest.Content.Projectiles.Weapon.Thrower;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ThrowerItems
{
    public class CursedKunai : ModItem
    {

        public override void SetStaticDefaults()
        {
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.Corruptor, ItemDropRule.Common(Type, 16));
        }


        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.damage = 32;
            Item.DamageType = DamageClass.Throwing;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<CursedKunaiThrown>();
            Item.shootSpeed = 30f;
            Item.noUseGraphic = true;
        }


    }
}