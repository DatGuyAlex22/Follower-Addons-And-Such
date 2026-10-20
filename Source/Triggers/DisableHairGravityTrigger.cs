using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/DisableHairGravityTrigger")]
public class DisableHairGravityTrigger : Trigger
{
    private bool enable;

    public DisableHairGravityTrigger(EntityData data, Vector2 offset) :
        base(data, offset)
    {
        enable = data.Bool("enable");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (enable)
        {
            FollowerAddonsModule.Session.StopHairFall = true;
        }
        else
        {
            FollowerAddonsModule.Session.StopHairFall = false;
        }
    }
}
