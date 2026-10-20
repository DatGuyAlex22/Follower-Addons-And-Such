using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.FollowerAddons.Entities;

[CustomEntity("FollowerAddons/CustomDepthTouchSwitch")]
public class CustomDepthTouchSwitch : TouchSwitch
{
    public CustomDepthTouchSwitch(EntityData data, Vector2 offset) :
        base(data.Position + offset)
    {
        base.Depth = data.Int("depth");
    }
}