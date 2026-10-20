using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.FollowerAddons.Entities;

[CustomEntity("FollowerAddons/TrailColorController")]
public class TrailColorController : Entity
{
    public Color color;

    private string entities;

    public string[] entitiesSplit;

    private bool global;

    public TrailColorController(EntityData data, Vector2 offset) :
        base(data.Position + offset)
    {
        entities = data.String("entities");
        entitiesSplit = String.Format(entities).Split(";");
        color = data.HexColor("color");
        global = data.Bool("global");

        if (global)
        {
            Tag += Tags.Global;
        }
    }
}