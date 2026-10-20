using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/FollowerTrailTrigger")]
public class FollowerTrailTrigger : Trigger
{
    private bool triggerOn;

    private bool resetOnLeave;

    public FollowerTrailTrigger(EntityData data, Vector2 offset) :
        base(data, offset)
    {
        this.triggerOn = data.Bool("triggerOn");
        this.resetOnLeave = data.Bool("resetOnLeave");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerOn)
        {
            FollowerAddonsModule.Session.TriggeredFollowerTrails = true;
        }
        else
        {
            FollowerAddonsModule.Session.TriggeredFollowerTrails = false;
        }
    }

    public override void OnLeave(Player player)
    {
        if (resetOnLeave)
        {
            if (triggerOn)
            {
                FollowerAddonsModule.Session.TriggeredFollowerTrails = false;
            }
            else
            {
                FollowerAddonsModule.Session.TriggeredFollowerTrails = true;
            }
        }
        base.OnLeave(player);
    }
}