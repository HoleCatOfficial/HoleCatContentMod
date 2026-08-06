using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Pristine : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 1;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            if (player.TryGetModPlayer<UrcerisMiniPlayer>(out var cool))
            {
                cool.Active = true;
            }
        }
    }
    public class PristineGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if ((item.type == ItemID.FrozenCrate || item.type == ItemID.FrozenCrateHard))
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Pristine>(), 10, 1, 1));
            }
        }
    }

    public class PristineDropNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.IceBat || npc.type == NPCID.UndeadViking)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Pristine>(), 10, 1, 1));
            }
        }
    }


    public class UrcerisMiniPlayer : ModPlayer
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

                int DMG = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(26);
                if (DMG < 1)
                {
                    DMG = 26;
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<UrcerisMini>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<UrcerisMini>(), DMG, 4, Player.whoAmI);
                }
            }


        }
    }
}
