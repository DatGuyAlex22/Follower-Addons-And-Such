using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Celeste.Mod.FollowerAddons.Triggers;

[CustomEntity("FollowerAddons/CustomRegisterAreaCompleteTrigger")]
public class CustomRegisterAreaCompleteTrigger : Trigger
{
    private bool collectStrawberries;

    private bool registerInSave;

    private bool onlyOnce;

    private string _flag;

    public CustomRegisterAreaCompleteTrigger(EntityData data, Vector2 offset, EntityID ID) :
        base(data, offset)
    {
        collectStrawberries = data.Bool("collectStrawberries");
        registerInSave = data.Bool("registerInSave");
        onlyOnce = data.Bool("onlyOnce");
        _flag = $"fa_customregisterareacompletetrigger_{ID}";
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (onlyOnce && !SceneAs<Level>().Session.GetFlag(_flag))
        {
            Player entity = Scene.Tracker.GetEntity<Player>();
            if ((entity != null) && collectStrawberries)
            {
                FollowerAddonsModule.Session.StrawberriesOverrideCollect = true;
                List<IStrawberry> list = new List<IStrawberry>();
                ReadOnlyCollection<Type> berryTypes = StrawberryRegistry.GetBerryTypes();
                foreach (Follower follower in entity.Leader.Followers)
                {
                    if (berryTypes.Contains(follower.Entity.GetType()) && follower.Entity is IStrawberry)
                    {
                        list.Add(follower.Entity as IStrawberry);
                    }
                }
                foreach (IStrawberry item in list)
                {
                    item.OnCollect();
                }
                FollowerAddonsModule.Session.StrawberriesOverrideCollect = false;
            }
            if (registerInSave)
            {
                SaveData.Instance.RegisterCompletion(SceneAs<Level>().Session);
            }
            SceneAs<Level>().Session.SetFlag(_flag);
        }
    }
}
