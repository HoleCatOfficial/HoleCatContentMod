using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

public class ScreenshakePlayer : ModPlayer
{
    public int screenshakeTimer;
    public int screenshakeMagnitude;
    public override void ModifyScreenPosition()
    {
        screenshakeTimer--;
        if (screenshakeTimer > 0 )
        {
            Main.screenPosition += new Vector2(Main.rand.Next(screenshakeMagnitude * -1, screenshakeMagnitude + 1), Main.rand.Next(screenshakeMagnitude * -1, screenshakeMagnitude + 1));
        }
    }
    
    public Vector2 GetShakeOffset()
    {
        if (screenshakeTimer <= 0)
            return Vector2.Zero;

        screenshakeTimer--;

        return new Vector2(
            Main.rand.Next(-screenshakeMagnitude, screenshakeMagnitude + 1),
            Main.rand.Next(-screenshakeMagnitude, screenshakeMagnitude + 1)
        );
    }
}