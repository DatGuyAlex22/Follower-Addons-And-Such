using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.FollowerAddons.Entities;

[CustomEntity("FollowerAddons/EntityOutliner")]
public class EntityOutliner : Entity
{
    private Color color;

    private string entities;

    private string[] entitiesSplit;

    private bool global;

    private int drawOffset;

    public EntityOutliner(EntityData data, Vector2 offset) :
        base(data.Position + offset)
    {
        entities = data.String("entities");
        entitiesSplit = String.Format(entities).Split(";");
        color = data.HexColor("color");
        global = data.Bool("global");
        drawOffset = data.Int("offset");

        if (global)
        {
            Tag += Tags.Global;
        }
    }

    public override void Render()
    {
        for (int i = 0; i < entitiesSplit.Length; i++)
        {
            foreach (Entity entity in SceneAs<Level>().Entities)
            {
                if (entity.GetType().Name == entitiesSplit[i] && entity.Get<Sprite>() != null)
                {
                    entity.Get<Sprite>().DrawOutline(color, drawOffset);
                }
            }
        }
        base.Render();
    }
}
