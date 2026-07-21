
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Shield)]
    public class HolyBreaker : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            player.statDefense += 16;
            player.GetModPlayer<HolyBreakerPlayer>().Active = true;
        }
    }

    public class HolyBreakerPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        int IndexOfAttacker = -1;
        int Timer = 0;
        bool T = false;
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                if (hurtInfo.Damage > 20 && Cooldown <= 0)
                {
                    IndexOfAttacker = npc.whoAmI;
                    T = true;
                }
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                
                if (hurtInfo.Damage > 20 && Cooldown <= 0)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.boss && n.active && !n.dontTakeDamage)
                        {
                            IndexOfAttacker = n.whoAmI;
                            T = true;
                        }
                    }
                }
                
            }
        }

        int Cooldown = 0;
        public override void PostUpdateEquips()
        {
            
            if (Active)
            {
                if (Cooldown > 0)
                {
                    Cooldown--;
                }
                if (Cooldown == 1)
                {
                    SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate with { Pitch = -0.8f }, Player.Center);

                }
                if (IndexOfAttacker > -1)
                {
                    NPC Attacker = Main.npc[IndexOfAttacker];

                    if (!Attacker.active || Attacker.life <= 0)
                    {
                        return;
                    }

                    Vector2 Above = Attacker.Center + new Vector2(Main.rand.NextFloat(-300, 300), -1000);
                    Vector2 vel = Above.DirectionTo(Attacker.Center);
                    vel.Normalize();


                    if (Timer < 60 && T)
                    {
                        Timer++;
                    }
                    if (Timer >= 60 && T)
                    {
                        Timer = 0;
                        T = false;
                        SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/Smite"));
                        Projectile.NewProjectile(Player.GetSource_None(), Above, vel * 0.01f, ModContent.ProjectileType<HolyRay>(), 900, 10, Player.whoAmI, ai1: vel.ToRotation());
                        Player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 5;
                        Player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 80;
                        Cooldown = 600;
                    }

                   
                }
            }
        }
    }

    public class HolyRay : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.ai[1];
            Projectile.penetrate = -1;
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        int oF2 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 15;
            oF2 -= 10;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(11), Color.MediumOrchid * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.8f, 4f);


            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.DarkGoldenrod, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.8f), SpriteEffects.None);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(11), Color.PeachPuff, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.65f, 3f);

            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);

            Main.spriteBatch.ResetToDefault();
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.ai[1];
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 0.85f;

            float t = ((Projectile.ai[0] / 30));
            if (Projectile.timeLeft > 210)
            {
                WidthScl = MathHelper.Lerp(0f, MaxScl, t);
            }
            if (Projectile.timeLeft < 270 && Projectile.timeLeft > 30)
            {
                WidthScl = MaxScl;
                Projectile.ai[0] = 0;
            }
            if (Projectile.timeLeft < 30)
            {
                WidthScl = MathHelper.Lerp(MaxScl, 0, t);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }

    public class HB_DROP_NPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {

            if (npc.type == NPCID.BigMimicHallow)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HolyBreaker>(), 25, 1, 1));
            }

        }
    }
}