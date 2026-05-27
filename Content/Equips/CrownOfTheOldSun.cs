
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class CrownOfTheOldSun : ModItem
    {

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = 310;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity2>();
        }

      

        public override void UpdateInventory(Player player)
        {
            InvGlowRot += 0.01f;
        }

        float InvGlowRot= 0f;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            spriteBatch.Draw(DTAssetLib.Sparkle(1).Value, position, null, ColorLib.Rift with { A = 0 } * 0.8f, InvGlowRot, DTAssetLib.Sparkle(1).Value.Size() / 2, 0.12f, SpriteEffects.None, 0f);
            spriteBatch.Draw(DTAssetLib.Sparkle(1).Value, position, null, Color.White with { A = 0 } * 0.8f, InvGlowRot, DTAssetLib.Sparkle(1).Value.Size() / 2, 0.07f, SpriteEffects.None, 0f);

            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            WorldGlowRot += 0.01f;
        }

        float WorldGlowRot = 0f;
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Type, out var T, out var Frame);
            Vector2 orig = Frame.Size() / 2;
            Vector2 position = Item.Bottom - Main.screenPosition - new Vector2(0, orig.Y + 12);

            spriteBatch.Draw(DTAssetLib.Sparkle(1).Value, position, null, ColorLib.Rift with { A = 0 } * 0.5f, WorldGlowRot, DTAssetLib.Sparkle(1).Value.Size() / 2, 0.24f, SpriteEffects.None, 0f);
            spriteBatch.Draw(DTAssetLib.Sparkle(1).Value, position, null, Color.White with { A = 0 } * 0.5f, WorldGlowRot, DTAssetLib.Sparkle(1).Value.Size() / 2, 0.14f, SpriteEffects.None, 0f);
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.rand.NextBool(32) && !hideVisual)
            {
                HeliciteShineParticle Shine = new();
                Shine.Initialize(player.Center, Main.rand.NextVector2Circular(3f, 3f));
                ParticleEngine.ShaderParticles.Add(Shine);

                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<SolarTrail>(), (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(16), 12, player.whoAmI);
            }

            player.maxMinions += 6;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.16f;
            player.GetArmorPenetration(DamageClass.Summon) += 22;
        }
    }
}