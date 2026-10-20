using YamlDotNet.Serialization;

namespace Celeste.Mod.FollowerAddons;

public class FollowerAddonsModuleSession : EverestModuleSession
{
    public bool StrawberriesOverrideCollect;

    public bool TriggeredStopCollect;

    public bool TriggeredKeepOnDeath;

    public bool TriggerTriggerFollowerSizeOverride;

    public bool FollowersAlwaysSized;

    public float TriggeredFollowerSizeOverride;

    public bool TriggeredFollowerTrails;

    public bool TriggerTriggerHoldableSizeOverride;

    public bool HoldablesAlwaysSized;

    public float TriggeredHoldableSizeOverride;

    public bool TriggeredHoldableTrails;

    public bool OverMaxFollowerBuffer;

    public bool TriggeredStopSeedPopping;

    public bool StopHairFall;

    public bool TriggeredStopHeartCollect;
}
