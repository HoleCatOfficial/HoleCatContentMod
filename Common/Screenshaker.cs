using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

public class ScreenshakePlayer : ModPlayer
{
    public int screenshakeTimer;
    public int screenshakeMagnitude;
    public bool isShaking = false;
    public override void ModifyScreenPosition()
    {
        if (!DTConfig.instance.ScreenshakeEffects)
        {
            return;
        }

        screenshakeTimer--;
        if (screenshakeTimer > 0 )
        {
            isShaking = true;
            Main.screenPosition += new Vector2(Main.rand.Next(screenshakeMagnitude * -1, screenshakeMagnitude + 1), Main.rand.Next(screenshakeMagnitude * -1, screenshakeMagnitude + 1));
        }
        else
        {
            isShaking = false;
        }
    }
    
    public Vector2 GetShakeOffset()
    {
        if (screenshakeTimer <= 0)
        {
            isShaking = false;
            return Vector2.Zero;
        }

        screenshakeTimer--;
        isShaking = true;

        return new Vector2(
            Main.rand.Next(-screenshakeMagnitude, screenshakeMagnitude + 1),
            Main.rand.Next(-screenshakeMagnitude, screenshakeMagnitude + 1)
        );
    }
}