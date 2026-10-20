using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/StrawberryBlockfieldTrigger = LoadV1", "FollowerAddons/StrawberryBlockfieldTriggerV2 = LoadV2")]
public class StrawberryBlockfieldTrigger : Trigger
{
    private bool keepOnDeath;

    private bool triggerOn;

    private bool resetOnLeave;

    private int version;

    private CollectionTypes collectionType;

    private enum CollectionTypes
    {
        Default = 0,
        Prevent = 1,
        Instant = 2,
    }

    public static StrawberryBlockfieldTrigger LoadV1(Level level, LevelData levelData, Vector2 offset, EntityData entityData) => new StrawberryBlockfieldTrigger(entityData, offset, 1);
    public static StrawberryBlockfieldTrigger LoadV2(Level level, LevelData levelData, Vector2 offset, EntityData entityData) => new StrawberryBlockfieldTrigger(entityData, offset, 2);

    public StrawberryBlockfieldTrigger(EntityData data, Vector2 offset, int version) :
        base(data, offset)
    {
        this.keepOnDeath = data.Bool("keepOnDeath");
        this.triggerOn = data.Bool("triggerOn");
        this.resetOnLeave = data.Bool("resetOnLeave");
        this.collectionType = data.Enum<CollectionTypes>(data.Attr("collectionType"));
        this.version = version;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        switch (version)
        {
            case 1:
                if (triggerOn)
                {
                    FollowerAddonsModule.Session.TriggeredStopCollect = true;
                }
                else
                {
                    FollowerAddonsModule.Session.TriggeredStopCollect = false;
                }

                if (keepOnDeath)
                {
                    FollowerAddonsModule.Session.TriggeredKeepOnDeath = true;
                }
                else
                {
                    FollowerAddonsModule.Session.TriggeredKeepOnDeath = false;
                }
                break;
            case 2:
                FollowerAddonsModule.Session.TriggeredStrawberryCollectionType = (int)collectionType;
                if (keepOnDeath)
                {
                    FollowerAddonsModule.Session.TriggeredKeepOnDeath = true;
                }
                else
                {
                    FollowerAddonsModule.Session.TriggeredKeepOnDeath = false;
                }
                break;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        
        if (resetOnLeave)
        {
            if (triggerOn)
            {
                FollowerAddonsModule.Session.TriggeredStopCollect = false;
            }
            else
            {
                FollowerAddonsModule.Session.TriggeredStopCollect = true;
            }

            if (keepOnDeath)
            {
                FollowerAddonsModule.Session.TriggeredKeepOnDeath = false;
            }
        }
    }
}