using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/CrystalHeartBlockfieldTrigger")]
public class CrystalHeartBlockfieldTrigger : Trigger
{
    private bool enable;

    private bool resetOnLeave;

    public CrystalHeartBlockfieldTrigger(EntityData data, Vector2 offset) :
        base(data, offset)
    {
        enable = data.Bool("enable");
        resetOnLeave = data.Bool("resetOnLeave");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (enable)
        {
            FollowerAddonsModule.Session.TriggeredStopHeartCollect = true;
        }
        else
        {
            FollowerAddonsModule.Session.TriggeredStopHeartCollect = false;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (resetOnLeave)
        {
            if (enable)
            {
                FollowerAddonsModule.Session.TriggeredStopHeartCollect = false;
            }
            else
            {
                FollowerAddonsModule.Session.TriggeredStopHeartCollect = true;
            }
        }
    }
}
