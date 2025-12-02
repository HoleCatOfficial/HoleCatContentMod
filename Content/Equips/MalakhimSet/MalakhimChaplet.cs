using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles;
using OpusLib;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Projectiles.player.ArmorSet;

namespace DestroyerTest.Content.Equips.MalakhimSet
{
    [AutoloadEquip(EquipType.Head)]
    public class MalakhimChaplet : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<PearlRarity>();
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MalakhimPlates>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.MalakhimChaplet.SetBonus");
            if (player.TryGetModPlayer<MalakhimPlayer>(out MalakhimPlayer MK))
            {
                MK.Active = true;
            }
        }

        public override void UpdateEquip(Player player)
        {
            float DamageBonus = 0.15f * player.statLife;
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += DamageBonus;
            player.statLifeMax2 += 40;
        }

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<Vesper>(25)
				.AddTile(TileID.Anvils)
				.Register();
        }
    }

    public class MalakhimPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                float Alpha = 0.2f + 0.2f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                Color clr = Color.Wheat * Alpha;
                Lighting.AddLight(Player.Center, clr.ToVector3());
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            Player.immuneTime = 100;
        }
    }
    
    public class MalakhimOwnedProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void AI(Projectile projectile)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            if (player.TryGetModPlayer<MalakhimPlayer>(out MalakhimPlayer MK) && MK.Active)
            {
                if (projectile.DamageType == ModContent.GetInstance<ScepterClass>())
                {
                    if (Main.rand.NextBool(24))
                    {
                        Projectile.NewProjectile(
                            projectile.GetSource_FromAI(),
                            projectile.Center,
                            (projectile.velocity * 0.04f).RotatedByRandom(1),
                            ModContent.ProjectileType<VesperDart>(),
                            projectile.damage / 10,
                            1,
                            projectile.owner,
                            ai2: 1
                        );
                    }
                }
            }
            base.AI(projectile);
        }
    }
}