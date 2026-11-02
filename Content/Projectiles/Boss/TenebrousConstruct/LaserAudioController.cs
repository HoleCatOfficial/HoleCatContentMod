using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class LaserAudioController : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<Entities.TenebrousConstruct>();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.timeLeft = 1200;
        }

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.LoopedSounds.HateLaser with
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public override void AI()
        {
            float Prog = (Projectile.timeLeft / 600f).Inverse();

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance =>
                {
                    //soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });

            }
            else
            {
                activeSound.Position = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                activeSound.Pitch = MathHelper.Lerp(0f, -0.1f, Prog);

                if (Projectile.timeLeft < 60)
                {
                    activeSound.Volume -= 0.04f;
                }
            }
        }
    }
}
