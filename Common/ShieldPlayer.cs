using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using OpusLib;

namespace DestroyerTest.Common
{
    public class ShieldPlayer : ModPlayer
    {
        /// <summary>
        /// The maximum durability of the shield.
        /// </summary>
        public virtual int MaxDurability { get; set; } = 125;
        /// <summary>
        /// The current durability of the shield.
        /// <para/> Cannot exceed MaxDurability and if it reaches 0, the shield breaks and needs to recharge.
        /// </summary>
        public virtual int Durability { get; set; } = 125;
        /// <summary>
        /// The radius of the shield's coverage.
        /// </summary>
        public virtual int Radius { get; set; } = 100;
        /// <summary>
        /// Whether or not the shield can activate.
        /// </summary>
        public bool Active = false;
        /// <summary>
        /// Whether or not the shield is able to absorb an incoming projectile.
        /// </summary>
        public bool Absorb = false;
        /// <summary>
        /// Whether or not the shield is broken and is recharging.
        /// </summary>
        public bool Recharge = false;
        /// <summary>
        /// The color used for the dusts, particles, and ring.
        /// </summary>
        public virtual Color themeColor { get; set; } = Color.Red;
        /// <summary>
        /// The sound that plays when the shield recharges fully and operates again.
        /// </summary>
        public virtual SoundStyle Regen { get; set; } = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldActivate", 3) with { PitchVariance = 0.3f };
        /// <summary>
        /// The sound that plays when the shield absorbs a projectile.
        /// </summary>
        public virtual SoundStyle Hit { get; set; } = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldHit", 3) with { PitchVariance = 0.3f };
        /// <summary>
        /// The sound that plays when the shield is broken.
        /// </summary>
        public virtual SoundStyle Break { get; set; } = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldBreak") with { PitchVariance = 0.3f };
        /// <summary>
        /// The death message that is used if the player expends too much health recharging their shield.
        /// <para/> You can only have 4 different messages.
        /// </summary>
        public virtual NetworkText[] DeathMSGs { get; set; } = new NetworkText[4];
        /// <summary>
        /// The amount of health taken on each charge tick to recharge the shield. Unless overriden to behave otherwise, the shield will recharge the same amount that is taken from your health.
        /// </summary>
        public virtual int RechargeHealthTax { get; set; } = 5;

        
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            DTUtils Utility = new DTUtils();
            Vector2 drawPos = Player.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            drawPos.Y -= 200;

            string text = $"{Durability.ToString()} / {MaxDurability.ToString()}";

            if (Active)
            {
                Utils.DrawBorderString(spriteBatch, text, drawPos, themeColor, 2f, 0.5f, 0.5f);
                if (Absorb && !Recharge)
                {
                    Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                    Main.EntitySpriteDraw(
                        DTAssetLib.BloomRingSharp.Value,
                        Player.Center - Main.screenPosition,
                        null,
                        themeColor * 0.75f,
                        0f,
                        DTAssetLib.BloomRingSharp.Value.Size() / 2,
                        Radius / (DTAssetLib.BloomRingSharp.Value.Width / 2f),
                        SpriteEffects.None,
                        0f
                    );
                    Opus.ReturnToDefaultDrawing(spriteBatch);
                }
            }
        }

        public override void PostUpdateEquips()
        {
            var shieldManager = ShieldManager.Instance;
            if (shieldManager == null)
                return;

            if (shieldManager.ActiveShieldID != -1 && !shieldManager.IsOwner(Player))
                return;


            if (Active)
            {
                if (Durability == MaxDurability && !Recharge)
                {
                    Absorb = true;
                }
            }

            if (Active && Absorb && !Recharge)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (!p.active || !p.hostile || p.friendly || p.damage <= 0)
                        continue;

                    if (p.Distance(Player.Center) > Radius)
                        continue;

                    if (!p.TryGetGlobalProjectile(out ShieldGlobal hostile))
                        continue;

                    if (hostile.Blocked)
                        continue;

                    hostile.Blocked = true;

                    Durability -= p.damage;

                    SoundEngine.PlaySound(Hit, Player.Center);

                    for (int y = 0; y < 9; y++)
                    {
                        PRTLoader.NewParticle(
                            PRTLoader.GetParticleID<SparkParticle>(),
                            p.Center,
                            new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)),
                            themeColor,
                            0.4f
                        );
                    }

                    p.Kill();

                    if (Durability <= 0)
                    {
                        SoundEngine.PlaySound(Break, Player.Center);
                        Absorb = false;
                        Recharge = true;
                        break;
                    }
                }

                if (Main.rand.NextBool(2400))
                {
                    SoundEngine.PlaySound(SoundID.Pixie with { Pitch = -2 }, Player.Center);
                }

                for (int r = 0; r < 3; r++)
                {
                    BasePRT WallPRT = PRTLoader.NewParticle(
                        PRTLoader.GetParticleID<SimpleParticle>(),
                        Player.Center + Main.rand.NextVector2CircularEdge(Radius, Radius),
                        Vector2.Zero, themeColor, 0.4f
                    );
                    WallPRT.Velocity += Player.velocity;
                    Dust WallDust = Dust.NewDustPerfect(
                        Player.Center + Main.rand.NextVector2CircularEdge(Radius, Radius),
                        DustID.TintableDustLighted, Vector2.Zero, 0, themeColor, 1.0f
                    );
                    WallDust.velocity += Player.velocity;
                }

                if (Durability <= 0)
                {

                    Absorb = false;   // shield can’t block anymore
                    Recharge = true;  // enter recharge mode
                }
            }

            // --- Recharge phase ---
            if (Recharge)
            {
                if (Main.GameUpdateCount % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Unlock with { Pitch = -2 }, Player.Center);

                    PlayerDeathReason deathReason = PlayerDeathReason.ByCustomReason(DeathMSGs[Main.rand.Next(DeathMSGs.Length)]);
                    Player.Hurt(deathReason, RechargeHealthTax, 0, false, true, -1, false);
                    Durability += RechargeHealthTax;
                }


                if (Durability >= MaxDurability)
                {
                    SoundEngine.PlaySound(Regen, Player.Center);
                    Recharge = false;
                    Absorb = true;
                }
            }
        }

        public override void PostUpdate()
        {
            var shieldManager = ShieldManager.Instance;
            if (shieldManager == null)
                return;

            if (Active && Absorb && !shieldManager.IsOwner(Player))
            {
                shieldManager.TryActivateShield(Player, ShieldIDs.Infernal);
            }
        }

    }

    public class ShieldGlobal : GlobalProjectile
    {
        public bool Blocked = false;
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Blocked = false;
        }
    }
}