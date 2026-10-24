using Microsoft.Xna.Framework;
using System;
using YamlDotNet.Serialization;

namespace Celeste.Mod.FollowerAddons;

public class FollowerAddonsModuleSettings : EverestModuleSettings
{
    private TextMenu.Slider CollectSpeedEntry;
    public int CollectSpeed { get; set; }
    public enum CollectSpeeds
    {
        Default,
        Prevent,
        Instant
    }

    private TextMenu.OnOff KeepOnDeathEntry;
    [SettingIgnore]
    public bool KeepOnDeath { get; set; } = false;

    private TextMenu.OnOff StopSeedPoppingEntry;
    [SettingIgnore]
    public bool StopSeedPopping { get; set; } = false;

    [SettingIgnore]
    public bool CreateFollowerTrails { get; set; } = true;
    private TextMenu.OnOff CreateFollowerTrailsEntry;

    [SettingIgnore]
    public int FollowerScale { get; set; } = 10;
    private TextMenu.Slider FollowerScaleEntry;

    [SettingIgnore]
    public bool CreateHoldableTrails { get; set; } = true;
    private TextMenu.OnOff CreateHoldableTrailsEntry;

    [SettingIgnore]
    public int HoldableScale { get; set; } = 10;
    private TextMenu.Slider HoldableScaleEntry;

    [SettingIgnore]
    public bool StopHairFall { get; set; } = false;
    private TextMenu.OnOff StopHairFallEntry;

    [SettingIgnore]
    public bool IncreaseDepth { get; set; } = true;
    private TextMenu.OnOff IncreaseDepthEntry;

    [SettingIgnore]
    public bool StopHeartCollect { get; set; } = false;
    private TextMenu.OnOff StopHeartCollectEntry;

    [SettingIgnore]
    public bool EvenIfGolden { get; set; } = false;
    private TextMenu.OnOff EvenIfGoldenEntry;

    [SettingIgnore]
    public int MaxFollowersBuffer { get; set; } = 10;
    private TextMenu.Slider MaxFollowersBufferEntry;

    private bool _MasterSwitch;
    public bool MasterSwitch
    {
        get => _MasterSwitch;
        set
        {
            _MasterSwitch = value;
            if (MasterSwitchEntry != null)
            {
                MasterSwitchEntry.UnselectedColor = value ? Color.LawnGreen : Color.Red * 3;
            }

            if (LeniencyOptionsEntry != null)
            {
                LeniencyOptionsEntry.Disabled = !value;
            }
            if (CosmeticOptionsEntry != null)
            {
                CosmeticOptionsEntry.Disabled = !value;
            }
            if (ExperimentalOptionsEntry != null)
            {
                ExperimentalOptionsEntry.Disabled = !value;
            }
        }
    }
    private TextMenu.OnOff MasterSwitchEntry;
    public void CreateMasterSwitchEntry(TextMenu menu, bool inGame)
    {
        menu.Add(MasterSwitchEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_MASTERSWITCH"),
            MasterSwitch == true
        ));

        MasterSwitchEntry.UnselectedColor = MasterSwitch ? Color.LawnGreen : Color.Red * 3; ;
        MasterSwitchEntry.Change(master => MasterSwitch = master ? true : false);
        MasterSwitchEntry.AddDescription(menu, Dialog.Clean("MODSETTING_MASTERSWITCH_CONTEXT"));
    }

    // Menus
    [YamlIgnore]
    public bool LeniencyOptionsDummy { get; set; }
    private TextMenuExt.SubMenu LeniencyOptionsEntry;

    public void CreateLeniencyOptionsDummyEntry(TextMenu menu, bool inGame)
    {
        LeniencyOptionsEntry = new TextMenuExt.SubMenu(Dialog.Clean("MODMENU_LENIENCYOPTIONS"), false);
        LeniencyOptionsEntry.Disabled = !MasterSwitch;
        menu.Add(LeniencyOptionsEntry);

        // Strawberry Collection Type
        Array emunValues = Enum.GetValues(typeof(CollectSpeeds));
        Array.Sort((int[])emunValues);
        LeniencyOptionsEntry.Add(CollectSpeedEntry = new TextMenu.Slider(
            Dialog.Clean("MODSETTING_STRAWBERRYCOLLECTSPEED"),
            (int i) => Dialog.Clean($"MODSETTING_STRAWBERRYCOLLECTSPEED_{emunValues.GetValue(i)}".ToUpper()),
            0,
            emunValues.Length - 1,
            CollectSpeed
        ));
        CollectSpeedEntry.Change(speed => CollectSpeed = speed);

        // Keep On Death
        LeniencyOptionsEntry.Add(KeepOnDeathEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_KEEPONDEATH"),
            KeepOnDeath == true
        ));
        KeepOnDeathEntry.Change(keepOnDeath => KeepOnDeath = keepOnDeath ? true : false);

        // Easy Berry Seeds
        LeniencyOptionsEntry.Add(StopSeedPoppingEntry = new TextMenu.OnOff(
                label: Dialog.Clean("MODSETTING_STOPSEEDPOPPING"),
                on: StopSeedPopping == true
            ));
        StopSeedPoppingEntry.Change(stopPop => StopSeedPopping = stopPop ? true : false);

        // Followers Before Pausing Features
        LeniencyOptionsEntry.Add(MaxFollowersBufferEntry = new TextMenu.Slider(
            Dialog.Clean("MODSETTING_FOLLOWERSBUFFER"),
            (int i) => $"{i} Follower{((i == 1) ? "" : "s")}",
            0,
            100,
            MaxFollowersBuffer
        ));
        MaxFollowersBufferEntry.Change(maxFollowers => MaxFollowersBuffer = maxFollowers);
        MaxFollowersBufferEntry.AddDescription(LeniencyOptionsEntry, menu, Dialog.Clean("MODSETTING_FOLLOWERSBUFFER_CONTEXT"));
    }

    [YamlIgnore]
    public bool CosmeticOptionsDummy { get; set; }
    private TextMenuExt.SubMenu CosmeticOptionsEntry;

    public void CreateCosmeticOptionsDummyEntry(TextMenu menu, bool inGame)
    {
        CosmeticOptionsEntry = new TextMenuExt.SubMenu(
            Dialog.Clean("MODMENU_COSMETICOPTIONS"),
            false
        );
        CosmeticOptionsEntry.Disabled = !MasterSwitch;
        menu.Add(CosmeticOptionsEntry);

        CosmeticOptionsEntry.Add(new TextMenu.SubHeader(Dialog.Clean("MODSETTING_HEADER_FOLLOWER"), false));

        CosmeticOptionsEntry.Add(CreateFollowerTrailsEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_CREATEFOLLOWERTRAILS"),
            CreateFollowerTrails
         ));
        CreateFollowerTrailsEntry.Change(createTrail => CreateFollowerTrails = createTrail ? true : false);

        CosmeticOptionsEntry.Add(FollowerScaleEntry = new TextMenu.Slider(
            Dialog.Clean("MODSETTING_FOLLOWERSCALE"),
            (int i) => $"{((float)i) / 10}x",
            0,
            100,
            FollowerScale
        ));
        FollowerScaleEntry.Change(scale => FollowerScale = scale);

        CosmeticOptionsEntry.Add(new TextMenu.SubHeader(Dialog.Clean("MODSETTING_HEADER_HOLDABLE"), false));

        CosmeticOptionsEntry.Add(CreateHoldableTrailsEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_CREATEHOLDABLETRAILS"),
            CreateHoldableTrails == true
        ));
        CreateHoldableTrailsEntry.Change(createTrail => CreateHoldableTrails = createTrail ? true : false);

        CosmeticOptionsEntry.Add(HoldableScaleEntry = new TextMenu.Slider(
            Dialog.Clean("MODSETTING_HOLDABLESCALE"),
            (int i) => $"{((float)i) / 10}x",
            0,
            100,
            HoldableScale
        ));
        HoldableScaleEntry.Change(scale => HoldableScale = scale);

        CosmeticOptionsEntry.Add(new TextMenu.SubHeader(Dialog.Clean("MODSETTING_HEADER_PLAYER"), false));

        CosmeticOptionsEntry.Add(StopHairFallEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_STOPHAIRFALL"),
            StopHairFall == true
        ));
        StopHairFallEntry.Change(fall => StopHairFall = fall ? true : false);

        CosmeticOptionsEntry.Add(new TextMenu.SubHeader(Dialog.Clean("MODSETTING_HEADER_OTHER"), false));

        CosmeticOptionsEntry.Add(IncreaseDepthEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_INCREASEDEPTH"),
            IncreaseDepth == true
        ));
        IncreaseDepthEntry.Change(increase => IncreaseDepth = increase ? true : false);
        IncreaseDepthEntry.AddDescription(CosmeticOptionsEntry, menu, Dialog.Clean("MODSETTING_INCREASEDEPTH_CONTEXT"));

        CosmeticOptionsEntry.Add(StopHeartCollectEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_STOPHEARTCOLLECT"),
            StopHeartCollect == true
        ));
        StopHeartCollectEntry.Change(stopCollect => StopHeartCollect = stopCollect ? true : false);
    }

    [YamlIgnore]
    public bool ExperimentalOptionsDummy { get; set; }
    private TextMenuExt.SubMenu ExperimentalOptionsEntry;

    public void CreateExperimentalOptionsDummyEntry(TextMenu menu, bool inGame)
    {
        ExperimentalOptionsEntry = new TextMenuExt.SubMenu(
            Dialog.Clean("MODMENU_EXPERIMENTALOPTIONS"),
            false
        );
        ExperimentalOptionsEntry.Disabled = !MasterSwitch;
        menu.Add(ExperimentalOptionsEntry);

        ExperimentalOptionsEntry.Add(EvenIfGoldenEntry = new TextMenu.OnOff(
            Dialog.Clean("MODSETTING_EVENIFGOLDEN"),
            EvenIfGolden
        ));
        EvenIfGoldenEntry.AddDescription(ExperimentalOptionsEntry, menu, Dialog.Clean("MODSETTING_EVENIFGOLDEN_CONTEXT"));
        EvenIfGoldenEntry.Change(doOverride => EvenIfGolden = doOverride ? true : false);
    }
}