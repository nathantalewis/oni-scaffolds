using System.Collections.Generic;
using System;
using HarmonyLib;
using STRINGS;
using KMod;
using UnityEngine;
using System.Reflection;


namespace Scaffolds
{

  public class Scaffolds_Patch : UserMod2
  {
    public static class ScaffoldsPatches
    {
      [HarmonyPatch(typeof(GeneratedBuildings))]
      [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
      public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
      {
        public static void Prefix()
        {
                    Loc_Initialize_Patch.Translate(typeof(ScaffoldConfig));
                    Utils.AddBuildingStrings(ScaffoldConfig.Id, ScaffoldConfig.DisplayName, ScaffoldConfig.Description, ScaffoldConfig.Effect);
          // Add scaffold to build menu with the help of utils below

          Utils.AddPlan("Base", "storage", ScaffoldConfig.Id, "StorageLocker");
        }
      }

      [HarmonyPatch(typeof(ProductInfoScreen))]
      [HarmonyPatch(nameof(ProductInfoScreen.SetMaterials))]
      public static class ProductInfoScreen_SetMaterials_Patch
      {
        public static void Postfix(BuildingDef def, ref ProductInfoScreen __instance)
        {
          if (def.name == "Scaffold")
          {
            __instance.materialSelectionPanel.gameObject.SetActive(false); //remove material selector since no materials

          }
        }
      }

      [HarmonyPatch(typeof(ResourceRemainingDisplayScreen))]
      [HarmonyPatch(nameof(ResourceRemainingDisplayScreen.GetString))]
      public static class ResourceRemainingDisplayScreen_Patch
      {
        // This patch overwrites the 'sandstone 500/1kg' on the hover text card when building scaffolds.
        // It checks the BuildingDef in the hover card, since it's public, rather than BuildTool itself
        // It also checks to make sure that it's exactly 1kg mass - draggable items (wires, pipes etc) otherwise cause errors since they use a drag tool not build tool
        public static string Postfix(string __result, Recipe ___currentRecipe)
        {
          if (___currentRecipe.Ingredients[0].amount == -1f)
          {
            if (BuildTool.Instance.GetComponent<BuildToolHoverTextCard>().currentDef.name == "Scaffold")
            {
              __result = ScaffoldConfig.Free_insta_build; // TODO: Internationalization?
            }
          }
          return __result;
        }
      }


      [HarmonyPatch(typeof(BuildingDef))]
      [HarmonyPatch(nameof(BuildingDef.Instantiate))]
      public static class BuildingDef_Instantiate_Patch
      {
        public static bool Prefix(Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer, BuildingDef __instance, ref GameObject __result)
        {
          // This instantiate function is used to create the construction site when a valid building spot is selected
          // since we want instant build, it detects if a scaffold is being built and instant-builds if so
          // It uses same build command called in sandbox

          BuildingDef def = __instance;
          if (__instance.name != "Scaffold")
          { return true; }
          else
          {
            selected_elements[0] = TagManager.Create("Vacuum"); //sets to vacuum element to prevent heat exchange... this must be dealt with at deconstruct or it will cause a crash
            __instance.Build(Grid.PosToCell(pos), orientation, null, selected_elements, 293.15f, playsound: false, GameClock.Instance.GetTime());
            return false; // Any better ideas for ways to accomplish this instant build?
          }

        }
      }
            [HarmonyPatch(typeof(BuildingDef))]
            [HarmonyPatch(nameof(BuildingDef.PostProcess))]
            public static class BuildingDef_PostProcess_Patch
            {
                public static bool Prefix(BuildingDef __instance)
                {
                    BuildingDef def = __instance;
                    if (__instance.name != "Scaffold")
                    { return true; }
                    else
                    {
                        __instance.CraftRecipe = new Recipe(__instance.BuildingComplete.PrefabID().Name, 1f, (SimHashes)0, __instance.Name);
                        __instance.CraftRecipe.Icon = __instance.UISprite;
                        Recipe.Ingredient item = new Recipe.Ingredient(__instance.MaterialCategory[0], -1);//set it -1 in the build menu
                        __instance.CraftRecipe.Ingredients.Add(item);
                        return false; 
                    }

                }
            }
        }
  }

  public static class Utils
  {
    // Copied many times, originally from romen
    public static void AddBuildingStrings(string buildingId, string name, string description, string effect)
    {

      Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, buildingId));
      Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
      Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
    }

    //thanks psyko for new building adding methods
    public static void AddPlan(HashedString category, string subcategory, string idBuilding, string addAfter = null)
    {
      Debug.Log("Adding " + idBuilding + " to category " + category);
      foreach (PlanScreen.PlanInfo menu in TUNING.BUILDINGS.PLANORDER)
      {
        if (menu.category == category)
        {
          AddPlanToCategory(menu, subcategory, idBuilding, addAfter);
          return;
        }
      }

      Debug.Log($"Unknown build menu category: ${category}");
    }

    private static void AddPlanToCategory(PlanScreen.PlanInfo menu, string subcategory, string idBuilding, string addAfter = null)
    {
      List<KeyValuePair<string, string>> data = menu.buildingAndSubcategoryData;
      if (data != null)
      {
        if (addAfter == null)
        {
          data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
        }
        else
        {
          int index = data.IndexOf(new KeyValuePair<string, string>(addAfter, subcategory));
          if (index == -1)
          {
            Debug.Log($"Could not find building {subcategory}/{addAfter} to add {idBuilding} after. Adding at the end !");
            data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
            return;
          }
          data.Insert(index + 1, new KeyValuePair<string, string>(idBuilding, subcategory));
        }
      }
    }
  }
}
