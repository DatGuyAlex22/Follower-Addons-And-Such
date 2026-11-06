using System.Collections.Generic;

namespace Celeste.Mod.FollowerAddons;

public class FollowerAddonsModuleSession : EverestModuleSession
{
    public bool StrawberriesOverrideCollect;

    public bool StopCollect;

    public bool KeepOnDeath;

    public float FollowerSizeOverride = 1;

    public bool FollowersAlwaysSized;

    public bool FollowerTrails;

    public float HoldableSizeOverride = 1;

    public bool HoldablesAlwaysSized;

    public bool HoldableTrails;

    public bool OverMaxFollowerBuffer;

    public bool StopSeedPopping;

    public bool StopHairFall;

    public bool StopHeartCollect;

    public List<string> StoredStrawberries = new List<string>();
}
