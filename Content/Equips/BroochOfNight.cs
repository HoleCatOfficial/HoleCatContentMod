
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;

namespace DestroyerTest.Content.Equips
{
	public class BroochOfNight : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.buffImmune[ModContent.BuffType<NightInferno>()] = true;
            if (player.TryGetModPlayer<BroochKnockbackPlayer>(out var Knockback))
            {
                Knockback.Active = true;
                Knockback.HalfKnockback = true;
            }
		}


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddIngredient(ItemID.AdamantiteBar, 10)
            .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddIngredient(ItemID.TitaniumBar, 10)
            .Register();
        }
    }

    public class BroochKnockbackPlayer : ModPlayer
    {
        public bool Active = false;
        public bool HalfKnockback = false;
        public bool CobaltShieldKnockback = false;
        public override void ResetEffects()
		{
			Active = false;
            HalfKnockback = false;
            CobaltShieldKnockback = false;
		}

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Active)
            {
                if (HalfKnockback)
                {
                    modifiers.Knockback *= 0.5f;
                }
                if (CobaltShieldKnockback)
                {
                    modifiers.Knockback *= 0.25f;
                }
            }
        }
    };
}