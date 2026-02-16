using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{
    /// <summary>
    /// A simple explosion projectile with a definable radius.
    /// <br/> Can have an Interval, which will stall the actual explosion effect.
    /// <para/> Explosions cannot deal critical strikes, but do benefit from class damage bonuses.
    /// <para/> Explosions only deal damage once, on the first frame of the explosion.
    /// <para/> Explosions do not mine tiles. That can be done manually in OnExplode().
    /// </summary>
    public abstract class Explosion : ModProjectile
    {
        #region Parameters
        /// <summary>
        /// The amount of time that passes before the actual explosion.
        /// <br/> Defaults to 0, meaning an instant explosion.
        /// </summary>
        public virtual int Interval {get;} = 0;

        /// <summary>
        /// The Sound that plays when the explosion occurs.
        /// </summary>
        public virtual SoundStyle Sound {get;} = SoundID.DD2_KoboldExplosion;

        /// <summary>
        /// The radius of the explosion. Used for dealing damage, applying debuffs, and anything else happening within the bounds of the explosion.
        /// <para/> This value is not related to the hitbox, which is always a 2x2 rectangle.
        /// </summary>
        public abstract float AreaOfEffect {get;}

        public bool HasExploded { get; private set; }

        #endregion

        #region AI

        /// <summary>
        /// Allows you to control what happens before the explosion.
        /// <br/> If Interval is 0
        /// </summary>
        /// <returns> Whether or not the internal counter is less than or equal to <paramref name="Interval"/>. </returns>
        public virtual bool PreExplode()
        {
            return IntervalCounter >= Interval;
        }

        /// <summary>
        /// Called on the first frame of the explosion.
        /// <br/> The Explosion's Sound plays on the same frame as this.
        /// <br/> Does not do anything if <see cref="PreExplode()"/> returns false.
        /// </summary>
        public virtual void OnExplode()
        {

        }

        /// <summary>
        /// Runs continuously every tick after OnExplode until the projectile dies off.
        /// <para/> Even though this only really is an issue of edge cases, this also will not do anything if <see cref="PreExplode()"/> returns false.
        /// </summary>
        public virtual void PostExplosion()
        {
        
        }

        #endregion

        #region Inner Workings

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        private int IntervalCounter = 0;

        public override void AI()
        {
            if (IntervalCounter < Interval)
            {
                IntervalCounter++;
            }

            if (!PreExplode())
            return;

            if (IntervalCounter >= Interval)
            {
                if (HasExploded == false)
                {
                    SoundEngine.PlaySound(Sound, Projectile.Center);
                    OnExplode();
                    DealDamage();
                    HasExploded = true;
                }
                else
                {
                    PostExplosion();
                }
            }
        }

        private NPC.HitInfo DamageStats(NPC npc)
        {
            int dir = npc.Center.X > Projectile.Center.X ? 1 : -1;
            return new NPC.HitInfo
            {
                SourceDamage = Projectile.damage,
                DamageType = Projectile.DamageType,
                Knockback = Projectile.knockBack,
                Crit = false,
                HitDirection = dir
            };
        }
        private void DealDamage()
        {
            foreach (NPC npc in Main.npc)
            {
                float Range = AreaOfEffect * AreaOfEffect;
                if (Projectile.Center.DistanceSQ(npc.Center) < Range)
                {
                    npc.StrikeNPC(DamageStats(npc), false, false);
                }
            }
        }

        #endregion

    }
}