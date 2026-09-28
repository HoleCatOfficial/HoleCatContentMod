using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.OrionCrossover;
using DestroyerTest.Content.Projectiles.Pets;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Fargos.EternityDrops
{
    [AutoloadEquip(EquipType.Neck)]
    public class ConstellationWeaverScarf : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.value = 1000;
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ConstellationScarfPlayer>().Active = true;
        }
    }

    public class ConstellationScarfPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public int Cooldown = 600;
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (Cooldown > 0)
                {
                    Cooldown--;

                    if (Cooldown == 1)
                    {
                        SoundEngine.PlaySound(SoundID.Item165);
                    }
                }
                else
                {
                    if (DestroyerTestMod.ConstellationKeybind.JustPressed)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce);
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationSpawner>(), 0, 0, Player.whoAmI);
                        Cooldown = 600;

                    }
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {

            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {

            }
        }
    }
}
