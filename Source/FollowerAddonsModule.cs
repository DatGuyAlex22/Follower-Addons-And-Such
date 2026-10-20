using Celeste.Mod.FollowerAddons.Entities;
using Celeste.Mod.Meta;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.FollowerAddons;

public class FollowerAddonsModule : EverestModule
{
    public class EntityDepthFinder : Component
    {
        public int depth;

        public EntityDepthFinder(int depth) : base(visible: false, active: false)
        {
            this.depth = depth;
        }
    }
    public static FollowerAddonsModule Instance { get; private set; }

    public override Type SettingsType => typeof(FollowerAddonsModuleSettings);
    public static FollowerAddonsModuleSettings Settings => (FollowerAddonsModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(FollowerAddonsModuleSession);
    public static FollowerAddonsModuleSession Session => (FollowerAddonsModuleSession)Instance._Session;

    public FollowerAddonsModule()
    {
        Instance = this;
    }

    public override void Load()
    {
        On.Celeste.Strawberry.OnCollect += StopStrawberryCollect;
        On.Celeste.Strawberry.Update += InstantBerryCollect;

        On.Celeste.Level.RegisterAreaComplete += StrawberriesOverrideCollect;

        On.Celeste.Follower.Update += FollowerUpdateOverride;
        On.Celeste.Holdable.Update += HoldableUpdateOverride;
        On.Celeste.Strawberry.OnPlayer += AddBerryToSave;
        On.Celeste.Player.Die += RemoveDoNotLoad;

        On.Celeste.HeartGem.Collect += StopHeartCollect;

        Everest.Events.Player.OnSpawn += RestoreBerriesOnRespawn;

        On.Celeste.StrawberrySeed.Update += StopSeedReturn;

        On.Celeste.Player.UpdateHair += StopHairFalling;

        On.Celeste.Audio.CreateInstance += PauseSoundsOnBuffer;
    }

    public override void Unload()
    {
        On.Celeste.Strawberry.OnCollect -= StopStrawberryCollect;
        On.Celeste.Strawberry.Update -= InstantBerryCollect;

        On.Celeste.Level.RegisterAreaComplete -= StrawberriesOverrideCollect;

        On.Celeste.Follower.Update -= FollowerUpdateOverride;
        On.Celeste.Holdable.Update -= HoldableUpdateOverride;
        On.Celeste.Strawberry.OnPlayer -= AddBerryToSave;
        On.Celeste.Player.Die -= RemoveDoNotLoad;

        On.Celeste.HeartGem.Collect -= StopHeartCollect;

        Everest.Events.Player.OnSpawn -= RestoreBerriesOnRespawn;

        On.Celeste.StrawberrySeed.Update -= StopSeedReturn;

        On.Celeste.Player.UpdateHair -= StopHairFalling;

        On.Celeste.Audio.CreateInstance -= PauseSoundsOnBuffer;
    }

    public static void StopStrawberryCollect(On.Celeste.Strawberry.orig_OnCollect orig, Strawberry self)
    {
        if (Session.TriggeredStopCollect)
        {
            if (Session.StrawberriesOverrideCollect)
            {
                orig(self);
            }
        }
        else if (Settings.MasterSwitch)
        {
            if ((Settings.CollectSpeed == (int)FollowerAddonsModuleSettings.CollectSpeeds.Default) || Session.StrawberriesOverrideCollect && !Session.TriggeredStopCollect)
            {
                orig(self);
            }
            else if ((Settings.CollectSpeed == (int)FollowerAddonsModuleSettings.CollectSpeeds.Instant) && Session.OverMaxFollowerBuffer)
            {
                orig(self);
            }
        }
        else
        {
            orig(self);
        }
    }

    public static void StrawberriesOverrideCollect(On.Celeste.Level.orig_RegisterAreaComplete orig, Level self)
    {
        Session.StrawberriesOverrideCollect = true;
        if (Session.OverMaxFollowerBuffer)
        {
            foreach (Strawberry berry in self.Entities.FindAll<Strawberry>())
            {
                if ((berry.Follower.Leader != null) && (berry.Follower.FollowIndex > Settings.MaxFollowersBuffer) && !berry.collected)
                {
                    if (berry.Follower.Leader == self.Entities.FindFirst<Player>().Leader)
                    {
                        self.Session.Strawberries.Add(berry.ID);
                        berry.RemoveSelf();
                    }
                }
            }
        }
        orig(self);
        Session.StrawberriesOverrideCollect = false;
    }

    public static void FollowerUpdateOverride(On.Celeste.Follower.orig_Update orig, Follower self)
    {
        if (self.Entity.Get<EntityDepthFinder>() == null)
        {
            self.Entity.Add(new EntityDepthFinder(self.Entity.Depth));
        }
        Sprite sprite = self.Entity.Get<Sprite>();
        if (Session.TriggerTriggerFollowerSizeOverride)
        {
            if (((self.Leader != null) || Session.FollowersAlwaysSized) && (sprite != null) && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities"))
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One * Session.TriggeredFollowerSizeOverride, 0.5f);
                if ((sprite.Scale != Vector2.One) && Settings.IncreaseDepth)
                {
                    self.Entity.Depth = Depths.BGTerrain + 100;
                }
                else if (self.Entity.Get<EntityDepthFinder>() != null)
                {
                    if ((self.Entity.Depth != Depths.BGTerrain + 100) && (self.Entity.Depth != self.Entity.Get<EntityDepthFinder>().depth))
                    {
                        self.Entity.Get<EntityDepthFinder>().depth = self.Entity.Depth;
                    }
                    self.Entity.Depth = self.Entity.Get<EntityDepthFinder>().depth;
                }
            }
        }
        else if (Settings.MasterSwitch)
        {
            if ((self.Leader != null) && (sprite != null) && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities"))
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One * Settings.FollowerScale / 10, 0.5f);
                if ((sprite.Scale != Vector2.One) && Settings.IncreaseDepth)
                {
                    self.Entity.Depth = Depths.BGTerrain + 100;
                }
                else if (self.Entity.Get<EntityDepthFinder>() != null)
                {
                    if ((self.Entity.Depth != Depths.BGTerrain + 100) && (self.Entity.Depth != self.Entity.Get<EntityDepthFinder>().depth))
                    {
                        self.Entity.Get<EntityDepthFinder>().depth = self.Entity.Depth;
                    }
                    self.Entity.Depth = self.Entity.Get<EntityDepthFinder>().depth;
                }
            }
        }
        else
        {
            if (sprite != null)
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One * new Vector2(Math.Sign(sprite.Scale.X), Math.Sign(sprite.Scale.Y)), 0.5f);
            }
        }
        orig(self);

        Color trailColor = Color.White;
        bool fallback = false;
        if (Session.TriggeredFollowerTrails)
        {
            foreach (TrailColorController controller in self.Entity.Scene.Entities.FindAll<TrailColorController>())
            {
                if (controller.entitiesSplit.Contains<string>(self.Entity.GetType().Name))
                {
                    trailColor = controller.color;
                    fallback = true;
                    break;
                }
            }
            if (!fallback)
            {
                if (self.Entity is Strawberry)
                {
                    if (self.EntityAs<Strawberry>().Golden)
                    {
                        trailColor = Color.Goldenrod;
                    }
                    else if (self.EntityAs<Strawberry>().Moon)
                    {
                        trailColor = Color.Green;
                    }
                    else
                    {
                        trailColor = Color.IndianRed;
                    }
                }
                else if (self.Entity is Key)
                {
                    trailColor = Color.DarkGoldenrod;
                }
                else if (self.Entity is StrawberrySeed)
                {
                    trailColor = Color.LimeGreen;
                }
                else
                {
                    Random rng = new Random();
                    trailColor.R = (byte)Calc.Range(rng, 0, 255);
                    trailColor.G = (byte)Calc.Range(rng, 0, 255);
                    trailColor.B = (byte)Calc.Range(rng, 0, 255);
                }
            }

            if (self.Scene.OnInterval(0.1f) && (self.Leader != null) && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities") && !Session.OverMaxFollowerBuffer)
            {
                TrailManager.Add(self.Entity, trailColor, 1);
            }
        }
        else if (Settings.MasterSwitch && Settings.CreateFollowerTrails)
        {
            foreach (TrailColorController controller in self.Entity.Scene.Entities.FindAll<TrailColorController>())
            {
                if (controller.entitiesSplit.Contains<string>(self.Entity.GetType().Name))
                {
                    trailColor = controller.color;
                    fallback = true;
                    break;
                }
            }
            if (!fallback)
            {
                if (self.Entity is Strawberry)
                {
                    if (self.EntityAs<Strawberry>().Golden)
                    {
                        trailColor = Color.Goldenrod;
                    }
                    else if (self.EntityAs<Strawberry>().Moon)
                    {
                        trailColor = Color.Green;
                    }
                    else
                    {
                        trailColor = Color.IndianRed;
                    }
                }
                else if (self.Entity is Key)
                {
                    trailColor = Color.DarkGoldenrod;
                }
                else if (self.Entity is StrawberrySeed)
                {
                    trailColor = Color.YellowGreen;
                }
                else
                {
                    Random rng = new Random();
                    trailColor.R = (byte)Calc.Range(rng, 0, 255);
                    trailColor.G = (byte)Calc.Range(rng, 0, 255);
                    trailColor.B = (byte)Calc.Range(rng, 0, 255);
                }
            }
            if (self.Scene.OnInterval(0.1f) && (self.Leader != null) && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities") && !Session.OverMaxFollowerBuffer)
            {
                TrailManager.Add(self.Entity, trailColor, 1);
            }
        }
        orig(self);

        if (self.Leader != null)
        {
            if (self.Leader.Followers.Count >= Settings.MaxFollowersBuffer)
            {
                if (!Session.OverMaxFollowerBuffer)
                {
                    Logger.Warn("Follower Addons & Such", "The amount of entites following the player has exceded the limit set in Mod Options (MAX: " + Settings.MaxFollowersBuffer + "), some features have been temporarily paused for performance.");
                }
                Session.OverMaxFollowerBuffer = true;
            }
            else
            {
                if (Session.OverMaxFollowerBuffer)
                {
                    Logger.Info("Follower Addons & Such", "Features unpaused.");
                }
                Session.OverMaxFollowerBuffer = false;
            }
        }
        orig(self);
    }

    public static void HoldableUpdateOverride(On.Celeste.Holdable.orig_Update orig, Holdable self)
    {
        orig(self);

        Sprite sprite = self.Entity.Get<Sprite>();
        if (Session.TriggerTriggerHoldableSizeOverride && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities"))
        {
            if ((self.IsHeld || Session.HoldablesAlwaysSized) && (sprite != null))
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One * Session.TriggeredHoldableSizeOverride, 1f);
                if ((sprite.Scale != Vector2.One) && Settings.IncreaseDepth)
                {
                    self.Entity.Depth = Depths.BGTerrain + 100;
                }
            }
            else
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One, 0.5f);
            }
        }
        else if (Settings.MasterSwitch && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities"))
        {
            if (self.IsHeld && (sprite != null))
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One * Settings.HoldableScale / 10, 1f);
                if ((sprite.Scale != Vector2.One) && Settings.IncreaseDepth)
                {
                    self.Entity.Depth = Depths.BGTerrain + 100;
                }
            }
            else
            {
                sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One, 0.5f);
            }
        }
        else if (sprite != null)
        {
            sprite.Scale = Calc.Approach(sprite.Scale, Vector2.One * new Vector2(Math.Sign(sprite.Scale.X), Math.Sign(sprite.Scale.Y)), 0.5f);
        }
        orig(self);

        Color trailColor = Color.White;
        bool fallback = false;
        if (Session.TriggeredHoldableTrails)
        {
            foreach (TrailColorController controller in self.Entity.Scene.Entities.FindAll<TrailColorController>())
            {
                if (controller.entitiesSplit.Contains<string>(self.Entity.GetType().Name))
                {
                    trailColor = controller.color;
                    fallback = true;
                    break;
                }
            }
            if (!fallback)
            {
                if (self.Entity is TheoCrystal)
                {
                    trailColor = Color.ForestGreen;
                }
                else if (self.Entity is Glider)
                {
                    trailColor = Color.DeepSkyBlue;
                }
                else
                {
                    Random rng = new Random();
                    trailColor.R = (byte)Calc.Range(rng, 0, 255);
                    trailColor.G = (byte)Calc.Range(rng, 0, 255);
                    trailColor.B = (byte)Calc.Range(rng, 0, 255);
                }
            }

            if (self.Scene.OnInterval(0.1f) && self.IsHeld && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities"))
            {
                TrailManager.Add(self.Entity, trailColor, 1);
            }
        }
        else if (Settings.MasterSwitch && Settings.CreateHoldableTrails)
        {
            foreach (TrailColorController controller in self.Entity.Scene.Entities.FindAll<TrailColorController>())
            {
                if (self.Entity.SourceData != null)
                {
                    if (controller.entitiesSplit.Contains<string>(self.Entity.GetType().Name))
                    {
                        trailColor = controller.color;
                        fallback = true;
                        break;
                    }
                }
            }
            if (!fallback)
            {
                if (self.Entity is TheoCrystal)
                {
                    trailColor = Color.ForestGreen;
                }
                else if (self.Entity is Glider)
                {
                    trailColor = Color.DeepSkyBlue;
                }
                else
                {
                    Random rng = new Random();
                    trailColor.R = (byte)Calc.Range(rng, 0, 255);
                    trailColor.G = (byte)Calc.Range(rng, 0, 255);
                    trailColor.B = (byte)Calc.Range(rng, 0, 255);
                }
            }

            if (self.Scene.OnInterval(0.1f) && self.IsHeld && (self.Entity.GetType().Namespace != "Celeste.Mod.CelesteNet.Client.Entities"))
            {
                TrailManager.Add(self.Entity, trailColor, 1);
            }
            orig(self);
        }
    }

    public static void AddBerryToSave(On.Celeste.Strawberry.orig_OnPlayer orig, Strawberry self, Player player)
    {
        if ((Settings.MasterSwitch && Settings.KeepOnDeath) || Session.TriggeredKeepOnDeath)
        {
            Session session = (self.Scene as Level).Session;
            session.DoNotLoad.Add(self.ID);
        }
        orig(self, player);
    }

    public static PlayerDeadBody RemoveDoNotLoad(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        if (!SaveData.Instance.Assists.Invincible)
        {
            if ((Settings.KeepOnDeath && Settings.MasterSwitch) || Session.TriggeredKeepOnDeath)
            {
                bool hasGolden = false;
                foreach (Follower follower in self.Leader.Followers)
                {
                    if (follower.Entity is Strawberry)
                    {
                        if (follower.EntityAs<Strawberry>().Golden)
                        {
                            hasGolden = true;
                        }
                        Vector2 start = self.SceneAs<Level>().Session.RespawnPoint.Value - new Vector2(0f, 20f);
                        Alarm.Set(follower.Entity, 0.15f, () =>
                        {
                            Vector2 vector = (start - follower.Entity.Position).SafeNormalize();
                            float num = Vector2.Distance(follower.Entity.Position, start);
                            float num2 = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
                            Vector2 control = start + vector * 16f + vector.Perpendicular() * num2 * Calc.Random.Choose(1, -1);
                            SimpleCurve curve = new SimpleCurve(follower.Entity.Position, start, control);
                            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Min(num / 100f, 2f), start: true);
                            tween.OnUpdate = (Tween f) =>
                            {
                                follower.Entity.Position = curve.GetPoint(f.Eased);
                            };
                            follower.Entity.Add(tween);
                        });
                    }
                }
                if (!hasGolden || Settings.EvenIfGolden)
                {
                    Leader.StoreStrawberries(self.Leader);
                }
            }
            else if (self.Leader.Followers != null)
            {
                foreach (Follower follower in self.Leader.Followers)
                {
                    if ((follower.Entity is Strawberry) && (!follower.EntityAs<Strawberry>().collected))
                    {
                        Session session = (self.Scene as Level).Session;
                        session.DoNotLoad.Remove(follower.EntityAs<Strawberry>().ID);
                    }
                }
            }
        }
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }

    public static void RestoreBerriesOnRespawn(Player player)
    {
        if ((Settings.MasterSwitch && Settings.KeepOnDeath) || Session.TriggeredKeepOnDeath)
        {
            player.Leader.PastPoints.Clear();
            if (Leader.storedBerries != null)
            {
                if (Leader.storedBerries.Count >= Settings.MaxFollowersBuffer)
                {
                    Logger.Warn("FollowerAddons", "Restored " + Leader.storedBerries.Count + " of " + Settings.MaxFollowersBuffer + " max strawberries. Collecting large amounts of strawberries at once may cause performance issues!");
                }
                for (int i = 0; i < Leader.storedBerries.Count; i++)
                {
                    Leader.storedOffsets[i] = new Vector2(0, -20);
                }
                Leader.RestoreStrawberries(player.Leader);
                Leader.storedOffsets.Clear();
                Leader.storedBerries.Clear();
            }
        }
    }

    public static void StopHeartCollect(On.Celeste.HeartGem.orig_Collect orig, HeartGem self, Player player)
    {
        if (!Settings.StopHeartCollect && !Session.TriggeredStopHeartCollect)
        {
            MapMetaModeProperties mapMetaModeProperties = ((self.SceneAs<Level>() != null) ? self.SceneAs<Level>().Session.MapData.Meta : null);
            if (mapMetaModeProperties != null && mapMetaModeProperties.HeartIsEnd.HasValue)
            {
                Session.StrawberriesOverrideCollect = mapMetaModeProperties.HeartIsEnd.Value;
            }
            orig(self, player);
        }
        else
        {
            Audio.Play("event:/game/general/assist_dreamblockbounce", self.Position);
            player.PointBounce(self.Center);
            self.moveWiggler.Start();
            self.ScaleWiggler.Start();
            self.moveWiggleDir = (self.Center - player.Center).SafeNormalize(Vector2.UnitY);
        }
    }

    public static void StopSeedReturn(On.Celeste.StrawberrySeed.orig_Update orig, StrawberrySeed self)
    {
        if (Settings.StopSeedPopping || Session.TriggeredStopSeedPopping)
        {
            self.canLoseTimer += Engine.DeltaTime;
        }
        orig(self);
    }

    public static void StopHairFalling(On.Celeste.Player.orig_UpdateHair orig, Player self, bool applyGravity)
    {
        if ((Settings.MasterSwitch && Settings.StopHairFall) || Session.StopHairFall)
        {
            applyGravity = false;
        }
        orig(self, applyGravity);
    }

    public static void CountDash(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        orig(self);
        Logger.Debug("Follower Addons & Such", "CountDash hook exists but isn't used.");
    }

    public static void InstantBerryCollect(On.Celeste.Strawberry.orig_Update orig, Strawberry self)
    {
        orig(self);
        if (Settings.MasterSwitch && (Settings.CollectSpeed == (int)FollowerAddonsModuleSettings.CollectSpeeds.Instant) && (!self.Golden || Settings.EvenIfGolden) && !Session.TriggeredStopCollect && self.Follower.Leader != null)
        {
            Session.StrawberriesOverrideCollect = true;
            if (!Session.OverMaxFollowerBuffer)
            {
                self.OnCollect();
            }
            Session.StrawberriesOverrideCollect = false;
        }
    }

    public static EventInstance PauseSoundsOnBuffer(On.Celeste.Audio.orig_CreateInstance orig, string path, Vector2? position)
    {
        if ((path != "event:/game/general/strawberry_get") || !Session.OverMaxFollowerBuffer)
        {
            return orig(path, position);
        }
        return null;
    }
}