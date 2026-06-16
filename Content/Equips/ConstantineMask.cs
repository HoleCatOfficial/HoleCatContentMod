
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class ConstantineMask : ModItem
	{
        public override void SetStaticDefaults()
        {
            DTUtils.isDevItem.Add(Type);
        }

        public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1, silver: 70);
			Item.rare = ModContent.RarityType<DevRarity>();
			Item.defense = 40; 
            Item.vanity = false;
		}

        public override void UpdateEquip(Player player)
        {
			player.buffImmune[BuffID.Ichor] = true;
        }

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<CoatStantine>() && legs.type == ModContent.ItemType<ConstanJeans>();
		}

		public override void UpdateArmorSet(Player player) 
		{
			player.DefaultSetBonusText(Item);
			player.GetModPlayer<ConstantineSetBonusPlayer>().Active = true;
		}
	}

	public class ConstantineSetBonusPlayer : ModPlayer
	{
		public bool Active = true;

        public override void ResetEffects()
        {
			Active = false;
        }

        bool IsCopy = false;

        public override void DrawPlayer(Camera camera)
        {
            if (Active)
            {
                IsCopy = true;
                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + new Vector2(10, 0), Player.fullRotation, Player.Center, 0.75f, 1f);
                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + new Vector2(-10, 0), Player.fullRotation, Player.Center, 0.75f, 1f);

                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + new Vector2(0, 10), Player.fullRotation, Player.Center, 0.75f, 1f);
                Main.PlayerRenderer.DrawPlayer(camera, Player, Player.position + new Vector2(0, -10), Player.fullRotation, Player.Center, 0.75f, 1f);
                IsCopy = false;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Color CloneColor = Opus.Sine(Color.IndianRed, Color.Orange);
            if (Active)
            {
                if (IsCopy)
                {
                    drawInfo.colorArmorBody = drawInfo.colorArmorHead = drawInfo.colorArmorLegs = CloneColor;
                }
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
			if (item.CountsAsClass(DamageClass.Generic) && Active)
			{
				return 0.97f;
			}
			return 1f;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && item.CountsAsClass(DamageClass.Generic) && Active)
			{
				int[] Options = new int[5] { BuffID.Electrified, BuffID.OnFire3, BuffID.Ichor, BuffID.CursedInferno, ModContent.BuffType<SpiritDrift>() };
				int Selection = Options[Main.rand.Next(Options.Length)];
				target.AddBuff(Selection, 300);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && proj.CountsAsClass(DamageClass.Generic) && Active)
            {
                int[] Options = new int[5] { BuffID.Electrified, BuffID.OnFire3, BuffID.Ichor, BuffID.CursedInferno, ModContent.BuffType<SpiritDrift>() };
                int Selection = Options[Main.rand.Next(Options.Length)];
                target.AddBuff(Selection, 300);
            }
        }
    }
}
