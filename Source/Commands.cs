using FMOD.Studio;
using Monocle;

namespace Celeste.Mod.FollowerAddons;

public static class Commands
{
    [Command("fa_wipedonotload", "Clear every entry in Session.DoNotLoad. Log level must be at least debug to log entries.")]
    public static void WipeSessionDoNotLoad(bool logEntries = false)
    {
        Scene scene = Engine.Scene;
        if (scene.GetType() == typeof(Level))
        {
            if (logEntries)
            {
                foreach (EntityID item in (scene as Level).Session.DoNotLoad)
                {
                    Logger.Debug("Follower Addons & Such", "Cleared item with ID " + item.ToString());
                }
            }
            (scene as Level).Session.DoNotLoad.Clear();
        }
        else
        {
            Engine.Commands.Log("Scene must be Level!");
        }
    }

    [Command("fa_wipeleaderpastpoints", "Wipes the 'past points' property of every Leader in the current scene.")]
    public static void WipeLeaderPastPoints()
    {
        foreach (Entity entity in Engine.Scene.Entities.FindAll<Entity>())
        {
            if (entity.Get<Leader>() != null)
            {
                entity.Get<Leader>().PastPoints.Clear();
            }
        }
    }

    [Command("music_param", "Set a FMOD parameter in the currently playing music event.")]
    public static void SetMusicParam(string parameter = "progress", int value = 0)
    {
        EventInstance eventInstance = Audio.CurrentMusicEventInstance;
        Scene scene = Engine.Scene;
        if (eventInstance == null)
        {
            Engine.Commands.Log("Could not find event.");
        }
        else if (scene.GetType() != typeof(Level))
        {
            Engine.Commands.Log("Scene must be Level!");
        }
        else
        {
            eventInstance.setParameterValue(parameter, value);
        }
    }
}