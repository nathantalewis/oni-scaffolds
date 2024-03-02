using TUNING;
using UnityEngine;

namespace Scaffolds
{
  public class ScaffoldConfig : IBuildingConfig
  {
    public static LocString Free_insta_build = "Free insta-build!";

    public const string ID = "Scaffold";
    public static LocString DisplayName = "Scaffold";
    public static LocString Description = "A temporary scaffold that will be built immediately with no materials.";
    public static LocString Effect = "A temporary way to move vertically.";

    public static LocString DeconstructButtonText = "Remove";
    public static LocString DeconstructButtonTooltip = "Instantly remove this scaffold";
    public static LocString SelfDestructButtonText = "Enable Self Destruct";
    public static LocString SelfDestructButtonTooltip = "When enabled, automatically remove scaffold after some time has passed";
    public static LocString SelfDestructButtonCancelText = "Make Permanent";
    public static LocString SelfDestructButtonCancelTooltip = "When enabled, this scaffold will remain until manually deconstructed";
    public static float TimeToLive = 10 * 600f;

    public static ObjectLayer ObjectLayer = ObjectLayer.FillPlacer; // This layer doesn't seem to be used anywhere else... hopefully

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef scaffoldDef = BuildingTemplates.CreateBuildingDef(
        ID,
        1, 1,
        "scaffold_kanim",
        10,
        1f,
        new float[1] { 1f }, //building mass is 1kg (of vacuum, imagine that) - less than 1kg causes graphical issues, zero mass causes error
        MATERIALS.ANY_BUILDABLE,
        9999f,
        BuildLocationRule.NotInTiles,
        noise: NOISE_POLLUTION.NONE,
        decor: BUILDINGS.DECOR.PENALTY.TIER1); // decor -10 because scaffolding is an eye sore
                                               // decor: BUILDINGS.DECOR.NONE); // Maybe make it configurable?

      BuildingTemplates.CreateLadderDef(scaffoldDef);
      scaffoldDef.ContinuouslyCheckFoundation = false; // Needed  since we are using "NotInTiles"
      scaffoldDef.ObjectLayer = ObjectLayer;
      scaffoldDef.Repairable = false;
      scaffoldDef.Disinfectable = false;
      scaffoldDef.Invincible = true; // nothing but the player can destroy the mighty Scaffold

      // Taken from LadderConfig
      scaffoldDef.Floodable = false;
      scaffoldDef.Overheatable = false;
      scaffoldDef.AudioCategory = "Metal";
      scaffoldDef.AudioSize = "small";
      scaffoldDef.DragBuild = true;

      return scaffoldDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
      // Taken from Ladder
      GeneratedBuildings.MakeBuildingAlwaysOperational(go);
      Ladder scaffold = go.AddOrGet<Scaffold>();
      AnimTileable animTileable = go.AddOrGet<AnimTileable>();

      // Custom

      animTileable.objectLayer = ObjectLayer;

      // TODO: Make this configurable
      scaffold.upwardsMovementSpeedMultiplier = 0.75f; // Scaffolds are rickety, we have to move more slowly on them
      scaffold.downwardsMovementSpeedMultiplier = 0.75f;

      go.AddOrGet<CopyBuildingSettings>();

      Deconstructable deconstructable = go.GetComponent<Deconstructable>();
      if (deconstructable != null)
      {
        Reconstructable reconstructable = go.GetComponent<Reconstructable>();
        if (reconstructable != null)
        {
          Object.Destroy(reconstructable); // remove vanilla reconstructable, no way to change materials here
        }
        Object.Destroy(deconstructable); // remove vanilla deconstructable, we'll be replacing it with our own below 
      }

      go.AddOrGet<DeconstructableScaffold>();

    }

    public override void DoPostConfigureComplete(GameObject go)
    {
    }
  }
}

