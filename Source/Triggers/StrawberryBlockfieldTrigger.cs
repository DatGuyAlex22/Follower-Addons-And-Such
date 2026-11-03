using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/StrawberryBlockfieldTrigger")]
public class StrawberryBlockfieldTrigger : Trigger
{
    private bool keepOnDeath;

    private bool triggerOn;

    private bool resetOnLeave;

    public StrawberryBlockfieldTrigger(EntityData data, Vector2 offset) :
        base(data, offset)
    {
        this.keepOnDeath = data.Bool("keepOnDeath");
        this.triggerOn = data.Bool("triggerOn");
        this.resetOnLeave = data.Bool("resetOnLeave");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

        if (triggerOn)
        {
            FollowerAddonsModule.Session.StopCollect = true;
        }
        else
        {
            FollowerAddonsModule.Session.StopCollect = false;
        }

        if (keepOnDeath)
        {
            FollowerAddonsModule.Session.KeepOnDeath = true;
        }
        else
        {
            FollowerAddonsModule.Session.KeepOnDeath = false;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);

        if (resetOnLeave)
        {
            if (triggerOn)
            {
                FollowerAddonsModule.Session.StopCollect = false;
            }
            else
            {
                FollowerAddonsModule.Session.StopCollect = true;
            }

            if (keepOnDeath)
            {
                FollowerAddonsModule.Session.KeepOnDeath = false;
            }
        }
    }
}