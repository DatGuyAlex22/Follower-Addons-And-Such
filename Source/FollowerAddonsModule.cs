using Celeste.Mod.FollowerAddons.Entities;
using Celeste.Mod.Meta;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public class EntityScaleManager : Component
    {
        public bool scaleReset;

        public EntityScaleManager(bool scaleReset = false) : base(false, false)
        {
            this.scaleReset = scaleReset;
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
        On.Celeste.Follower.Update += FollowerUpdateOverride;

        On.Celeste.Strawberry.OnCollect += StrawberryCollectOverride;
        On.Celeste.Strawberry.Update += StrawberryUpdateOverride;
        On.Celeste.Strawberry.OnPlayer += StrawberryOnPlayerOverride;
        On.Celeste.StrawberrySeed.Update += StrawberrySeedUpdateOverride;
        On.Celeste.Level.RegisterAreaComplete += StrawberriesOverrideCollect;

        On.Celeste.Holdable.Update += HoldableUpdateOverride;

        On.Celeste.Player.UpdateHair += PlayerUpdateHairOverride;
        On.Celeste.PlayerHair.AfterUpdate += PlayerHairAfterUpdateOverride;
        On.Celeste.Player.Die += PlayerDieOverride;
        Everest.Events.Player.OnSpawn += PlayerRespawnOverride;

        On.Celeste.HeartGem.Collect += CrystalHeartCollectOverride;

        On.Celeste.Audio.CreateInstance += AudioCreateInstanceOverride;
        On.Celeste.Level.LoadLevel += LoadLevelOverride;
    }

    public override void Unload()
    {
        On.Celeste.Follower.Update -= FollowerUpdateOverride;

        On.Celeste.Strawberry.OnCollect -= StrawberryCollectOverride;
        On.Celeste.Strawberry.Update -= StrawberryUpdateOverride;
        On.Celeste.Strawberry.OnPlayer -= StrawberryOnPlayerOverride;
        On.Celeste.StrawberrySeed.Update -= StrawberrySeedUpdateOverride;
        On.Celeste.Level.RegisterAreaComplete -= StrawberriesOverrideCollect;

        On.Celeste.Holdable.Update -= HoldableUpdateOverride;

        On.Celeste.Player.UpdateHair -= PlayerUpdateHairOverride;
        On.Celeste.PlayerHair.AfterUpdate -= PlayerHairAfterUpdateOverride;
        On.Celeste.Player.Die -= PlayerDieOverride;
        Everest.Events.Player.OnSpawn -= PlayerRespawnOverride;

        On.Celeste.HeartGem.Collect -= CrystalHeartCollectOverride;

        On.Celeste.Audio.CreateInstance -= AudioCreateInstanceOverride;
        On.Celeste.Level.LoadLevel -= LoadLevelOverride;
    }

    // Follower Hooks
    public static void FollowerUpdateOverride(On.Celeste.Follower.orig_Update orig, Follower self)
    {
        // TODO:
        // - Add support for MTextures

        Leader leader = self.Leader;
        bool hasLeader = (leader != null);

        if (!hasLeader)
        {
            orig(self);
            return;
        }

        bool leaderIsPlayer = (leader.Entity.GetType() == typeof(Player));

        Entity entity = self.Entity;
        bool hasEntity = (entity != null);
        Color trailColor = Color.Transparent;
        const int increasedDepth = Depths.BGTerrain + 100;

        if (!hasEntity)
        {
            orig(self);
            return;
        }

        List<Sprite> sprites = entity.Components.GetAll<Sprite>().ToList();
        bool hasSprite = (sprites.Count > 0);
        List<Image> mTextures = entity.Components.GetAll<Image>().ToList(); // Trying MTexture support
        bool hasMTexture = (mTextures.Count > 0);

        if (!hasSprite && !hasMTexture)
        {
            orig(self);
            return;
        }

        EverestModuleMetadata CelesteNet = new()
        {
            Name = "CelesteNet.Client",
            Version = new Version(2, 0)
        };
        bool isCelesteNetEntity = (Everest.Loader.DependencyLoaded(CelesteNet) && entity.GetType().Module.Name == CelesteNet.Name);

        if (isCelesteNetEntity)
        {
            orig(self);
            return;
        }

        if (entity.Get<EntityDepthFinder>() == null)
        {
            entity.Add(new EntityDepthFinder(entity.Depth));
            Logger.Verbose("FollowerAddons", $"Added new EntityDepthFinder to {entity}.");
        }
        EntityDepthFinder entityDepthFinder = entity.Get<EntityDepthFinder>();
        bool hasEntityDepthFinder = (entityDepthFinder != null);

        if (entity.Get<EntityScaleManager>() == null)
        {
            entity.Add(new EntityScaleManager());
            Logger.Verbose("FollowerAddons", $"Added new EntityScaleManager to {entity}.");
        }
        EntityScaleManager entityScaleManager = entity.Get<EntityScaleManager>();
        bool hasEntityScaleManager = (entityScaleManager != null);

        if (!hasEntityDepthFinder || !hasEntityScaleManager)
        {
            orig(self);
            return;
        }

        foreach (Sprite sprite in sprites)
        {
            if ((!Settings.MasterSwitch || Settings.FollowerScale == 10) && (Session.FollowerSizeOverride == 1))
            {
                if (!entityScaleManager.scaleReset)
                {
                    sprite.Scale = Vector2.One;
                }
                entityScaleManager.scaleReset = true;
            }
            else
            {
                entityScaleManager.scaleReset = false;
            }

            Vector2 scale = Vector2.One;
            if ((Session.FollowerSizeOverride != 1) && (leaderIsPlayer || Session.HoldablesAlwaysSized))
            {
                scale = Calc.Approach(sprite.Scale, Vector2.One * Session.FollowerSizeOverride / 10, 0.25f);
            }
            else if (Settings.MasterSwitch && leaderIsPlayer)
            {
                scale = Calc.Approach(sprite.Scale, Vector2.One * Settings.FollowerScale / 10, 0.25f);
            }

            if (!entityScaleManager.scaleReset)
            {
                sprite.Scale = scale;
            }

            if ((entity.Depth != entityDepthFinder.depth) && (entity.Depth != increasedDepth))
            {
                entityDepthFinder.depth = entity.Depth;
            }

            if (Settings.MasterSwitch && Settings.IncreaseDepth && (sprite.Scale != Vector2.One) && leaderIsPlayer)
            {
                entity.Depth = increasedDepth;
            }
            else
            {
                entity.Depth = entityDepthFinder.depth;
            }
        }
        foreach (Image image in mTextures)
        {
            if ((!Settings.MasterSwitch || Settings.FollowerScale == 10) && (Session.FollowerSizeOverride == 1))
            {
                if (!entityScaleManager.scaleReset)
                {
                    image.Scale = Vector2.One;
                }
                entityScaleManager.scaleReset = true;
            }
            else
            {
                entityScaleManager.scaleReset = false;
            }

            Vector2 scale = Vector2.One;
            if ((Session.FollowerSizeOverride != 1) && (leaderIsPlayer || Session.FollowersAlwaysSized))
            {
                scale = Calc.Approach(image.Scale, Vector2.One * Session.FollowerSizeOverride / 10, 0.25f);
            }
            else if (Settings.MasterSwitch && leaderIsPlayer)
            {
                scale = Calc.Approach(image.Scale, Vector2.One * Settings.FollowerScale / 10, 0.25f);
            }

            if (!entityScaleManager.scaleReset)
            {
                image.Scale = scale;
            }

            if ((entity.Depth != entityDepthFinder.depth) && (entity.Depth != increasedDepth))
            {
                entityDepthFinder.depth = entity.Depth;
            }

            if (Settings.MasterSwitch && Settings.IncreaseDepth && (image.Scale != Vector2.One) && leaderIsPlayer)
            {
                entity.Depth = increasedDepth;
            }
            else
            {
                entity.Depth = entityDepthFinder.depth;
            }
        }

        if (((Settings.MasterSwitch && Settings.CreateFollowerTrails) || Session.FollowerTrails) && leaderIsPlayer)
        {
            switch (entity)
            {
                case Strawberry:
                    if ((entity as Strawberry).Golden)
                    {
                        trailColor = Color.DarkGoldenrod;
                        break;
                    }
                    else if ((entity as Strawberry).Moon)
                    {
                        trailColor = Color.Green;
                        break;
                    }
                    else
                    {
                        trailColor = Color.IndianRed;
                        break;
                    }
                case StrawberrySeed:
                    trailColor = Color.LimeGreen;
                    break;
                case Key:
                    trailColor = Color.DarkGoldenrod;
                    break;
            }
            foreach (TrailColorController controller in entity.Scene.Entities.FindAll<TrailColorController>())
            {
                if (controller.entitiesSplit.Contains<string>(entity.GetType().Name))
                {
                    trailColor = controller.color;
                    break;
                }
            }

            if (entity.SceneAs<Level>().OnInterval(0.1f) && !Session.OverMaxFollowerBuffer)
            {
                TrailManager.Add(entity, trailColor, 1f);
            }
        }

        orig(self);

        if (leaderIsPlayer)
        {
            if (leader.Followers.Count > Settings.MaxFollowersBuffer)
            {
                if (!Session.OverMaxFollowerBuffer)
                {
                    Logger.Warn("FollowerAddons", $"The amount of entites following the player has exceded the limit set in Mod Options (MAX: {Settings.MaxFollowersBuffer}), some features have been temporarily paused for performance.");
                }
                Session.OverMaxFollowerBuffer = true;
            }
            else
            {
                if (Session.OverMaxFollowerBuffer)
                {
                    Logger.Info("FollowerAddons", "Features unpaused.");
                }
                Session.OverMaxFollowerBuffer = false;
            }
        }
    }

    // Strawberry Hooks
    public static void StrawberryCollectOverride(On.Celeste.Strawberry.orig_OnCollect orig, Strawberry self)
    {
        if (Session.StrawberriesOverrideCollect || ((Settings.CollectSpeed == (int)FollowerAddonsModuleSettings.CollectSpeeds.Default) || !Settings.MasterSwitch))
        {
            orig(self);
            Session.StoredStrawberries.Remove(self.ID.Key);
        }
        else if (Settings.MasterSwitch && (Settings.CollectSpeed == (int)FollowerAddonsModuleSettings.CollectSpeeds.Instant))
        {
            orig(self);
            Session.StoredStrawberries.Remove(self.ID.Key);
        }
    }
    public static void StrawberryUpdateOverride(On.Celeste.Strawberry.orig_Update orig, Strawberry self)
    {
        if (Session.OverMaxFollowerBuffer || Session.StopCollect || (self.Follower.Leader == null) || !Settings.MasterSwitch)
        {
            orig(self);
            return;
        }

        if ((Settings.CollectSpeed == (int)FollowerAddonsModuleSettings.CollectSpeeds.Instant) && (!self.Golden || Settings.EvenIfGolden)) // Exclude goldens by default.
        {
            Session.StrawberriesOverrideCollect = true;
            self.OnCollect();
            Session.StrawberriesOverrideCollect = false;
        }

        orig(self);
    }
    public static void StrawberryOnPlayerOverride(On.Celeste.Strawberry.orig_OnPlayer orig, Strawberry self, Player player)
    {
        if ((Settings.MasterSwitch && Settings.KeepOnDeath) || Session.KeepOnDeath)
        {
            (self.Scene as Level).Session.DoNotLoad.Add(self.ID);
        }

        orig(self, player);

        if (self.Follower.Leader == null)
        {
            return;
        }

        if (!Session.StoredStrawberries.Contains(self.ID.Key) && (self.Follower.Leader.Entity.GetType() == typeof(Player)))
        {
            Session.StoredStrawberries.Add(self.ID.Key);
        }
    }
    public static void StrawberrySeedUpdateOverride(On.Celeste.StrawberrySeed.orig_Update orig, StrawberrySeed self)
    {
        if ((Settings.MasterSwitch && Settings.StopSeedPopping) || Session.StopSeedPopping)
        {
            self.canLoseTimer += Engine.DeltaTime;
        }

        orig(self);
    }
    public static void StrawberriesOverrideCollect(On.Celeste.Level.orig_RegisterAreaComplete orig, Level self)
    {
        foreach (Strawberry strawberry in self.Entities.FindAll<Strawberry>())
        {
            if (!strawberry.Follower.HasLeader || strawberry.collected)
            {
                continue;
            }

            if ((strawberry.Follower.Leader.Entity.GetType() == typeof(Player)) && (strawberry.Follower.FollowIndex >= Settings.MaxFollowersBuffer))
            {
                self.Session.Strawberries.Add(strawberry.ID);
                strawberry.RemoveSelf();
            }
        }
        Session.StrawberriesOverrideCollect = true;
        orig(self);
        Session.StrawberriesOverrideCollect = false;
    }

    // Holdable Hooks
    public static void HoldableUpdateOverride(On.Celeste.Holdable.orig_Update orig, Holdable self)
    {
        Entity entity = self.Entity;
        bool hasEntity = (entity != null);
        bool isHeld = self.IsHeld;
        Color trailColor = Color.Transparent;
        const int increasedDepth = Depths.BGTerrain + 100;

        if (!hasEntity)
        {
            orig(self);
            return;
        }

        List<Sprite> sprites = entity.Components.GetAll<Sprite>().ToList();
        bool hasSprite = (sprites.Count > 0);
        List<Image> mTextures = entity.Components.GetAll<Image>().ToList(); // Trying MTexture support
        bool hasMTexture = (mTextures.Count > 0);

        if (!hasSprite && !hasMTexture)
        {
            orig(self);
            return;
        }

        EverestModuleMetadata CelesteNet = new()
        {
            Name = "CelesteNet.Client",
            Version = new Version(2, 0)
        };
        bool isCelesteNetEntity = (Everest.Loader.DependencyLoaded(CelesteNet) && entity.GetType().Module.Name == CelesteNet.Name);

        if (isCelesteNetEntity)
        {
            orig(self);
            return;
        }

        if (entity.Get<EntityDepthFinder>() == null)
        {
            entity.Add(new EntityDepthFinder(entity.Depth));
            Logger.Verbose("FollowerAddons", $"Added new EntityDepthFinder to {entity}.");
        }
        EntityDepthFinder entityDepthFinder = entity.Get<EntityDepthFinder>();
        bool hasEntityDepthFinder = (entityDepthFinder != null);

        if (entity.Get<EntityScaleManager>() == null)
        {
            entity.Add(new EntityScaleManager());
            Logger.Verbose("FollowerAddons", $"Added new EntityScaleManager to {entity}.");
        }
        EntityScaleManager entityScaleManager = entity.Get<EntityScaleManager>();
        bool hasEntityScaleManager = (entityScaleManager != null);

        if (!hasEntityDepthFinder || !hasEntityScaleManager)
        {
            orig(self);
            return;
        }

        foreach (Sprite sprite in sprites)
        {
            if ((!Settings.MasterSwitch || Settings.HoldableScale == 10) && (Session.HoldableSizeOverride == 1))
            {
                if (!entityScaleManager.scaleReset)
                {
                    sprite.Scale = Vector2.One;
                }
                entityScaleManager.scaleReset = true;
            }
            else
            {
                entityScaleManager.scaleReset = false;
            }

            Vector2 scale = Vector2.One;
            if ((Session.HoldableSizeOverride != 1) && (isHeld || Session.HoldablesAlwaysSized))
            {
                scale = Calc.Approach(sprite.Scale, Vector2.One * Session.HoldableSizeOverride / 10, 0.25f);
            }
            else if (Settings.MasterSwitch && isHeld)
            {
                scale = Calc.Approach(sprite.Scale, Vector2.One * Settings.HoldableScale / 10, 0.25f);
            }

            if (!entityScaleManager.scaleReset)
            {
                sprite.Scale = scale;
            }

            if ((entity.Depth != entityDepthFinder.depth) && (entity.Depth != increasedDepth))
            {
                entityDepthFinder.depth = entity.Depth;
            }

            if (Settings.MasterSwitch && Settings.IncreaseDepth && (sprite.Scale != Vector2.One) && isHeld)
            {
                entity.Depth = increasedDepth;
            }
            else
            {
                entity.Depth = entityDepthFinder.depth;
            }
        }

        foreach (Image image in mTextures)
        {
            if ((!Settings.MasterSwitch || Settings.HoldableScale == 10) && (Session.HoldableSizeOverride == 1))
            {
                if (!entityScaleManager.scaleReset)
                {
                    image.Scale = Vector2.One;
                }
                entityScaleManager.scaleReset = true;
            }
            else
            {
                entityScaleManager.scaleReset = false;
            }

            Vector2 scale = Vector2.One;
            if ((Session.HoldableSizeOverride != 1) && (isHeld || Session.HoldablesAlwaysSized))
            {
                scale = Calc.Approach(image.Scale, Vector2.One * Session.HoldableSizeOverride / 10, 0.25f);
            }
            else if (Settings.MasterSwitch && isHeld)
            {
                scale = Calc.Approach(image.Scale, Vector2.One * Settings.HoldableScale / 10, 0.25f);
            }

            if (!entityScaleManager.scaleReset)
            {
                image.Scale = scale;
            }

            if ((entity.Depth != entityDepthFinder.depth) && (entity.Depth != increasedDepth))
            {
                entityDepthFinder.depth = entity.Depth;
            }

            if (Settings.MasterSwitch && Settings.IncreaseDepth && (image.Scale != Vector2.One) && isHeld)
            {
                entity.Depth = increasedDepth;
            }
            else
            {
                entity.Depth = entityDepthFinder.depth;
            }
        }

        if (((Settings.MasterSwitch && Settings.CreateHoldableTrails) || Session.HoldableTrails) && isHeld)
        {
            switch (entity)
            {
                case TheoCrystal:
                    trailColor = Color.ForestGreen;
                    break;
                case Glider:
                    trailColor = Color.CornflowerBlue;
                    break;
            }
            foreach (TrailColorController controller in entity.Scene.Entities.FindAll<TrailColorController>())
            {
                if (controller.entitiesSplit.Contains<string>(entity.GetType().Name))
                {
                    trailColor = controller.color;
                    break;
                }
            }

            if (entity.SceneAs<Level>().OnInterval(0.1f) && !Session.OverMaxFollowerBuffer)
            {
                TrailManager.Add(entity, trailColor, 1f);
            }
        }

        orig(self);
    }

    // Player Hooks
    public static void PlayerUpdateHairOverride(On.Celeste.Player.orig_UpdateHair orig, Player self, bool applyGravity)
    {
        if (((Settings.MasterSwitch && Settings.StopHairFall) || Session.StopHairFall) && self.InControl)
        {
            applyGravity = false;
        }
        orig(self, applyGravity);
    }
    public static void PlayerHairAfterUpdateOverride(On.Celeste.PlayerHair.orig_AfterUpdate orig, PlayerHair self)
    {
        Player player = self.Entity as Player;
        if (player == null)
        {
            orig(self);
            return;
        }

        Leader leader = player.Leader;
        if (leader == null)
        {
            orig(self);
            return;
        }

        if ((leader.PastPoints.Count <= 0) || (self.Nodes.Count <= 0))
        {
            orig(self);
            return;
        }

        if (((Settings.MasterSwitch && Settings.StopHairFall) || Session.StopHairFall) && player.InControl)
        {
            for (int i = 0; i < (Calc.Min(leader.PastPoints.Count, self.Nodes.Count) - 1); i++)
            {
                self.Nodes[i] += (leader.PastPoints[i] - self.Nodes[i]) * Engine.DeltaTime * 2;
            }
        }
        orig(self);
    }
    public static PlayerDeadBody PlayerDieOverride(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        if (!SaveData.Instance.Assists.Invincible)
        {
            if ((Settings.KeepOnDeath && Settings.MasterSwitch) || Session.KeepOnDeath)
            {
                bool hasGolden = false;
                foreach (Follower follower in self.Leader.Followers)
                {
                    if (follower.Entity is Strawberry)
                    {
                        if (follower.EntityAs<Strawberry>().Golden)
                        {
                            hasGolden = true;
                            if (!Settings.EvenIfGolden)
                            {
                                break;
                            }
                        }
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

                Session.StoredStrawberries.Clear();
            }
        }
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }
    public static void PlayerRespawnOverride(Player player)
    {
        if ((Settings.MasterSwitch && Settings.KeepOnDeath) || Session.KeepOnDeath)
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

    // Crystal Heart Hook
    public static void CrystalHeartCollectOverride(On.Celeste.HeartGem.orig_Collect orig, HeartGem self, Player player)
    {
        if ((!Settings.MasterSwitch || !Settings.StopHeartCollect) && !Session.StopHeartCollect)
        {
            MapMetaModeProperties mapMetaModeProperties = self.SceneAs<Level>().Session.MapData.Meta;

            if (mapMetaModeProperties == null)
            {
                orig(self, player);
                return;
            }

            if (mapMetaModeProperties.HeartIsEnd.HasValue)
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

    // Other Hooks
    public static EventInstance AudioCreateInstanceOverride(On.Celeste.Audio.orig_CreateInstance orig, string path, Vector2? position)
    {
        if ((path != "event:/game/general/strawberry_get") || !Session.OverMaxFollowerBuffer)
        {
            return orig(path, position);
        }
        return null;
    }
    public static void LoadLevelOverride(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
    {
        orig(self, playerIntro, isFromLoader);

        Player player = self.Entities.FindFirst<Player>();
        List<string> keys = Session.StoredStrawberries.Distinct().ToList();

        if ((!Settings.SessionStrawberries || !Settings.MasterSwitch) || !isFromLoader || (player == null) || (keys == null))
        {
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            Strawberry strawberry = new Strawberry(new EntityData(), player.Position, new EntityID() { Key = keys[i] });
            self.Add(strawberry);
        }
    }
}