using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/FollowerSizeTrigger")]
public class FollowerSizeTrigger : Trigger
{
    private float sizeMult;

    private bool resetOnLeave;

    private bool alwaysScale = false;

    public FollowerSizeTrigger(EntityData data, Vector2 offset) :
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
        FollowerAddonsModule.Session.TriggerTriggerFollowerSizeOverride = true;
        FollowerAddonsModule.Session.TriggeredFollowerSizeOverride = sizeMult;
        FollowerAddonsModule.Session.FollowersAlwaysSized = alwaysScale;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (resetOnLeave)
        {
            FollowerAddonsModule.Session.TriggerTriggerFollowerSizeOverride = false;
            FollowerAddonsModule.Session.FollowersAlwaysSized =! alwaysScale;
        }
    }
}