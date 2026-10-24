
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class SwirlStone : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<PearlRarity>();
            Item.accessory = true;
        }


    
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SwirlStonePlayer>().Active = true;
        }
    }

    public class SwirlStonePlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
            {
                
            }
        }
    }

    public class SwirlStoneGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool active = Main.player[projectile.owner].GetModPlayer<SwirlStonePlayer>().Active == true;
            if (active && projectile.DamageType.CountsAsClass<ScepterClass>() && Main.rand.NextBool(20))
            {
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero, ModContent.ProjectileType<WindBlast>(), projectile.damage / 2, 10, projectile.owner);
            }
        }
    }

}