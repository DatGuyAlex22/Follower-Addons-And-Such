using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/HoldableSizeTrigger")]
public class HoldableSizeTrigger : Trigger
{
    private float sizeMult;

    private bool resetOnLeave;

    private bool alwaysScale = false;

    public HoldableSizeTrigger(EntityData data, Vector2 offset) :
        base(data, offset)
    {
        this.sizeMult = data.Float("sizeMult");
        this.resetOnLeave = data.Bool("resetOnLeave");
        if (data.Has("alwaysScale"))
        {
            this.alwaysScale = data.Bool("alwaysScale");
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        FollowerAddonsModule.Session.TriggerTriggerHoldableSizeOverride = true;
        FollowerAddonsModule.Session.TriggeredHoldableSizeOverride = sizeMult;
        FollowerAddonsModule.Session.HoldablesAlwaysSized = alwaysScale;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (resetOnLeave)
        {
            FollowerAddonsModule.Session.TriggerTriggerHoldableSizeOverride = false;
            FollowerAddonsModule.Session.HoldablesAlwaysSized = !alwaysScale;
        }
    }
}