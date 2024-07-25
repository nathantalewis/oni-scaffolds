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
        decor: new EffectorValues
        {
          amount = -Scaffolds_Patch.Settings.DecorPenaltyValue,
          radius = Scaffolds_Patch.Settings.DecorPenaltyRadius
        },
        noise: NOISE_POLLUTION.NONE
        );

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

#if DEBUG
      Debug.Log($"[Scaffolds] ScaffoldConfig.CreateBuildingDef with decor settings -{Scaffolds_Patch.Settings.DecorPenaltyValue} and {Scaffolds_Patch.Settings.DecorPenaltyRadius}.");
#endif

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

      // Scaffolds are rickety, we have to move more slowly on them
      scaffold.upwardsMovementSpeedMultiplier = (100 - Scaffolds_Patch.Settings.UpwardsSpeedPenalty) / 100.0f;
      scaffold.downwardsMovementSpeedMultiplier = (100 - Scaffolds_Patch.Settings.DownwardsSpeedPenalty) / 100.0f;
#if DEBUG
      Debug.Log($"[Scaffolds] ScaffoldConfig.ConfigureBuildingTemplate with speed settings {scaffold.upwardsMovementSpeedMultiplier} and {scaffold.downwardsMovementSpeedMultiplier}.");
#endif

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

