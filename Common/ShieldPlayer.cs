using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using FargowiltasSouls.Common.Graphics.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static InnoVault.VaultUtils.ItemDropScanner;

namespace DestroyerTest.Common
{
    public class Shield : ILoadable
    {
        public string InternalName = "";
        public int MaxDurability = -1;
        public float Radius = -1;
        public Color themeColor = Color.Red;
        public SoundStyle Regen = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldActivate", 3) with { PitchVariance = 0.3f, MaxInstances = 0 };
        public SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldHit", 3) with { PitchVariance = 0.3f, MaxInstances = 0 };
        public SoundStyle Break = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldBreak") with { PitchVariance = 0.3f, MaxInstances = 0 };
        public List<NetworkText> DeathMSGs = new List<NetworkText>();
        public int RechargeHealthTax = 5;
        public float Priority = 0;

        public Shield(string internalName, int maxDurability, float radius, Color themeColor, SoundStyle regen, SoundStyle hit, SoundStyle breakSound, List<NetworkText> deathMSGs, int rechargeHealthTax, float priority)
        {
            InternalName = internalName;
            MaxDurability = maxDurability;
            Radius = radius;
            this.themeColor = themeColor;
            Regen = regen;
            Hit = hit;
            Break = breakSound;
            DeathMSGs = deathMSGs;
            RechargeHealthTax = rechargeHealthTax;
            Priority = priority;
        }

        public Action<Player> OnActivate = player => { };
        public Action<Player> OnBreak = player => { };
        public Action<Player> AmbientEffects = player => { };

        public Shield(string internalName, int maxDurability, float radius, Color themeColor, SoundStyle regen, SoundStyle hit, SoundStyle breakSound, List<NetworkText> deathMSGs, int rechargeHealthTax, float priority, Action<Player> onActivate, Action<Player> onBreak, Action<Player> ambientEffects)
        {
            InternalName = internalName;
            MaxDurability = maxDurability;
            Radius = radius;
            this.themeColor = themeColor;
            Regen = regen;
            Hit = hit;
            Break = breakSound;
            DeathMSGs = deathMSGs;
            RechargeHealthTax = rechargeHealthTax;
            Priority = priority;

            OnActivate = onActivate;
            OnBreak = onBreak;
            AmbientEffects = ambientEffects;
        }

        bool ValidShield(Mod mod)
        {
            if (MaxDurability <= 0)
            {
                mod.Logger.Warn($"{mod.Name}: Shield {InternalName} has a durability of {MaxDurability}, which is less than 0 and is invalid.");
                return false;
            }
            if (Radius <= 0)
            {
                mod.Logger.Warn($"{mod.Name}: Shield {InternalName} has a radius of {Radius}, which is less than 0 and is invalid.");
                return false;
            }
            if (DeathMSGs.Count == 0)
            {
                mod.Logger.Warn($"{mod.Name}: Shield {InternalName} has no death messages.");
                return false;
            }

            foreach(Shield S in ShieldManager.LoadedShields)
            {
                if (S.InternalName == InternalName)
                {
                    mod.Logger.Warn($"{mod.Name}: Shield {InternalName} has the same internal name as another shield.");
                    return false;
                }

                if (S.Priority == Priority)
                {
                    mod.Logger.Warn($"{mod.Name}: Shield {InternalName} has the same priority as another shield.");
                    return false;
                }
            }


            return true;
        }

        void ILoadable.Load(Mod mod)
        {
            if (ValidShield(mod))
            {
                mod.Logger.InfoFormat("Shield: {0} was Loaded Successfully.", this);
                ShieldManager.LoadedShields.Add(this);
            }
        }

        void ILoadable.Unload()
        {
            ShieldManager.LoadedShields.Remove(this);
        }
    }

    public class ShieldPlayer : ModPlayer, IDrawPixelated
    {
        Shield DecideShield()
        {
            if (ShieldManager.ActiveShields[Player.whoAmI] != null)
            {
                return ShieldManager.ActiveShields[Player.whoAmI]
                    .OrderByDescending(x => x.Priority)
                    .FirstOrDefault();
            }

            return null;
        }

        public Shield CurrentShield => DecideShield();

        private int MaxDurability => CurrentShield?.MaxDurability ?? 0;

        private int Durability = 0;

        public int GetDurability()
        {
            return Durability;
        }

        private float Radius => CurrentShield?.Radius ?? 0;

        public bool Active = false;

        public bool Absorb = false;

        public bool Recharge = false;
        private Color themeColor => CurrentShield?.themeColor ?? Color.Red;
        private SoundStyle Regen => CurrentShield?.Regen ?? SoundID.Item1;
        private SoundStyle Hit => CurrentShield?.Hit ?? SoundID.Item1;
        private SoundStyle Break => CurrentShield?.Break ?? SoundID.Item1;
        private List<NetworkText> DeathMSGs => CurrentShield?.DeathMSGs ?? new List<NetworkText>();
        private int RechargeHealthTax => CurrentShield?.RechargeHealthTax ?? 0;

        public override void ResetEffects()
        {
            if (ShieldManager.ActiveShields != null)
            {
                int idx = Player.whoAmI;
                if (idx >= 0 && idx < ShieldManager.ActiveShields.Length && ShieldManager.ActiveShields[idx] != null)
                {
                    ShieldManager.ActiveShields[idx].Clear();
                }
            }
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
            }
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AbovePlayer;
        bool IDrawPixelated.ShouldDrawPixelated => true;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            //Main.NewText("G");
            
        }

        int DamageCooldown = 300;
        bool HealFlag = false;
        public override void PostUpdateEquips()
        {
            Active = (CurrentShield != null);

            if (Active)
            {
                if (Durability >= MaxDurability && !Recharge)
                {
                    Absorb = true;
                }

                if (Durability <= 0)
                {
                    Absorb = false;
                    Recharge = true;
                }

                if (Durability > MaxDurability)
                {
                    SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot);
                    Durability = MaxDurability;

                }

                if (Durability < MaxDurability && DamageCooldown <= 0)
                {
                    if (Player.miscCounter % 60 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DeerclopsStep with { MaxInstances = 0 }, Player.Center);
                        Durability++;
                    }
                    
                }
                if (Durability == MaxDurability && !HealFlag)
                {
                    SoundEngine.PlaySound(Regen, Player.Center);
                    HealFlag = true;
                }
            }

            if (DamageCooldown > 0)
            {
                HealFlag = false;
                DamageCooldown--;
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

                    DamageCooldown = 300;

                    SoundEngine.PlaySound(Hit, Player.Center);

                    for (int y = 0; y < 9; y++)
                    {
                        Spark Spark = new Spark();

                        Spark.PrepareSpark(p.Center, new Vector2(Main.rand.NextFloat(-2f, 2.1f), Main.rand.NextFloat(-4f, -6.1f)), 0f, themeColor, 0.4f, true, 90, SparkDrawMode.Additive);
                        ParticleEngine.BehindProjectiles.Add(Spark);
                    }

                    p.Kill();

                    if (Durability <= 0)
                    {
                        CurrentShield.OnBreak.Invoke(Player);
                        SoundEngine.PlaySound(Break, Player.Center);
                        Absorb = false;
                        Recharge = true;
                        break;
                    }
                }

                /*
                if (Main.rand.NextBool(2400))
                {
                    SoundEngine.PlaySound(SoundID.Pixie with { Pitch = -2 }, Player.Center);
                }
                */

                CurrentShield?.AmbientEffects.Invoke(Player);

                List<Dust> WallDusts = new List<Dust>();

                var vel = Player.velocity;
                vel.Normalize();
                var len = vel.Length();
               

                var WallDustPositions = Opus.GetEquidistantOrbitVectors(5, Player.MountedCenter, (0.05f * Player.direction) /*+ ((0.0005f * len) * Player.direction)*/, Radius);
                foreach(Vector2 p in WallDustPositions)
                {
                    Dust WallDust = Dust.NewDustPerfect(p, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, themeColor, 1.35f);
                    WallDusts.Add(WallDust);

                    PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
                    Glow.Initialize(p, Vector2.Zero, themeColor * 0.75f, 0.5f, 30);
                    ParticleEngine.Particles.Add(Glow);
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
                    SoundEngine.PlaySound(SoundID.Unlock with { Pitch = -2, MaxInstances = 0 }, Player.Center);

                    PlayerDeathReason deathReason = PlayerDeathReason.ByCustomReason(DeathMSGs[Main.rand.Next(DeathMSGs.Count)]);
                    int Decrement = (int)(RechargeHealthTax - (0.5f * Player.statDefense));
                    if (RechargeHealthTax < 0.5f * Player.statDefense)
                    {
                        Decrement = (int)(0.2f * Player.statDefense);
                    }
                    Player.statLife -= Decrement;
                    if (Player.statLife <= 0)
                    {
                        Player.KillMe(deathReason, (double)RechargeHealthTax, 0, false);
                        Durability = MaxDurability;
                    }

                    Durability += RechargeHealthTax;
                }


                if (Durability >= MaxDurability)
                {
                    CurrentShield?.OnActivate.Invoke(Player);
                    SoundEngine.PlaySound(Regen, Player.Center);
                    Recharge = false;
                    Absorb = true;
                }
            }
        }

        public override void PostUpdate()
        {

            if (Active && Absorb)
            {
                
            }
        }

        public override void OnRespawn()
        {
            Durability = MaxDurability;
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

    public class ShieldDrawer : IPlayerPixelatedDrawer
    {
        PixelLayer IPlayerPixelatedDrawer.PixelLayer => PixelLayer.AboveNPCs;

        bool IPlayerPixelatedDrawer.IsActive(Player player)
        {
            if (!player.active || player.dead)
                return false;

            if (player.TryGetModPlayer<ShieldPlayer>(out ShieldPlayer Shield))
            {
                return Shield.Active && Shield.CurrentShield != null && Shield.Absorb;
            }
            return false;
        }

        void IPlayerPixelatedDrawer.DrawPixelated(Player player, SpriteBatch spriteBatch)
        {
            //Main.NewText("G");
            var Shield = player.GetModPlayer<ShieldPlayer>();

            var Cap = spriteBatch.Capture();
            //Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            //spriteBatch.UseBlendState(BlendState.Additive);


            Color color = Shield.CurrentShield.themeColor;
            var position = player.MountedCenter - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);

            Main.EntitySpriteDraw(DTAssetLib.ShieldRing.Value, position, null, color with { A = 0 }, 0f, DTAssetLib.ShieldRing.Size() / 2, Shield.CurrentShield.Radius / (DTAssetLib.ShieldRing.Value.Width / 2f), SpriteEffects.None, 0);
            

            //spriteBatch.ResetToDefault();
        }
    }

    
}