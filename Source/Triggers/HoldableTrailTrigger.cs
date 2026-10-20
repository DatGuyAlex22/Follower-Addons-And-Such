using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/HoldableTrailTrigger")]
public class HoldableTrailTrigger : Trigger
{
    private bool triggerOn;

    private bool resetOnLeave;

    public HoldableTrailTrigger(EntityData data, Vector2 offset) :
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
            FollowerAddonsModule.Session.TriggeredHoldableTrails = true;
        }
        else
        {
            FollowerAddonsModule.Session.TriggeredHoldableTrails = false;
        }
    }

    public override void OnLeave(Player player)
    {
        if (resetOnLeave)
        {
            if (triggerOn)
            {
                FollowerAddonsModule.Session.TriggeredHoldableTrails = false;
            }
            else
            {
                FollowerAddonsModule.Session.TriggeredHoldableTrails = true;
            }
        }
        base.OnLeave(player);
    }
}