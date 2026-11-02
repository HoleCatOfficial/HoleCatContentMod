using DestroyerTest.Content.Projectiles.ShadeThrasherFriendly;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class ShadeThrasherBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            SpawnIfNeededAndSetTime(player, buffIndex, ref unused, ModContent.ProjectileType<ShadeThrasherFriendlyHead>());

            /*
            if (player.TryGetModPlayer<TenebrisScepterPlayer>(out TenebrisScepterPlayer Scepter))
            {
                if (!Scepter.Active)
                {
                    player.DelBuff(buffIndex);
                }
            }
            */
        }

        public void SpawnIfNeededAndSetTime(Player player, int buffIndex, ref bool petBool, int petProjID, int buffTimeToGive = 18000)
        {
            player.buffTime[buffIndex] = buffTimeToGive;
            SpawnIfNeeded(player, ref petBool, petProjID, buffIndex);
        }

        public void SpawnIfNeeded(Player player, ref bool petBool, int petProjID, int buffIndex)
        {
            petBool = true;
            bool flag = true;
            if (player.ownedProjectileCounts[petProjID] > 0)
                flag = false;

            Vector2 center = player.Center;

            if (flag && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), center.X, center.Y, 0f, 0f, petProjID, 80, 5f, player.whoAmI);
            }
        }
    }
}