using DestroyerTest.Common;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class WhisperingTomeHoldout : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2000;
        }

        public float rO = 0f;
        public float SFac = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            rO += 0.2f;
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, ColorLib.DarkRift3, false, 0);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, ColorLib.DarkRift2, false, rO, 1f * SFac, 2.3f * SFac);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, ColorLib.Rift, false, 0, 1f * SFac, 2.3f * SFac);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White, false, 0, 0.7f * SFac, 2f * SFac);

            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/LaserLoop1")
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        public float PitchVal = -3;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<WhisperingTome>() && player.controlUseItem == true)
            {
                if (SFac < 1f)
                {
                    SFac += 0.05f;
                }
                if (PitchVal < -1)
                {
                    PitchVal += 0.1f;
                }

                if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
                {
                    var tracker = new ProjectileAudioTracker(Projectile);
                    LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                        soundInstance.Position = Projectile.Center;
                        soundInstance.Pitch = PitchVal;
                        return tracker.IsActiveAndInGame();
                    });
                }
                else
                {
                    activeSound.Position = Projectile.Center;
                    activeSound.Pitch = PitchVal;
                }

                float holdDistance = 40f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                Projectile.Center = desiredPos;

                player.SetCompositeArmFront(player.HeldItem.type == ModContent.ItemType<WhisperingTome>() && player.controlUseItem == true, Player.CompositeArmStretchAmount.Full, toCursor.ToRotation() - MathHelper.PiOver2);


                if (Main.GameUpdateCount % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toCursor * 6, ModContent.ProjectileType<RiftSpark>(), Projectile.damage, 6f, Projectile.owner);
                }
            }
            else
            {
                if (SFac > 0f)
                {
                    SFac -= 0.05f;
                }
                if (PitchVal > -3f)
                {
                    PitchVal -= 0.1f;
                }

                if (SFac <= 0f && PitchVal <= -3f)
                {
                    Projectile.Kill();
                }
            }
        }

    }
}