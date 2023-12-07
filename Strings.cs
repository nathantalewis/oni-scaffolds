using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace Scaffolds
{
  // [HarmonyPatch(typeof(Localization), "Initialize")]
  public static class Loc_Initialize_Patch
  {
    public static void Translate(Type root)
    {
      Localization.RegisterForTranslation(root);
      string text = Loc_Initialize_Patch.LoadStrings();
      LocString.CreateLocStringKeys(root, null);
      Localization.GenerateStringsTemplate(root, text);
    }

    // Token: 0x0600000E RID: 14 RVA: 0x00002580 File Offset: 0x00000780
    private static string LoadStrings()
    {
      string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      int num = directoryName.IndexOf("archived_versions");
      string path;
      if (num != -1)
      {
        path = directoryName.Substring(0, num - 1);
      }
      else
      {
        path = directoryName;
      }
      string text = Path.Combine(path, "translations");
      string path2 = text;
      Localization.Locale locale = Localization.GetLocale();
      string text2 = Path.Combine(path2, ((locale != null) ? locale.Code : null) + ".po");
      if (File.Exists(text2))
      {
        Localization.OverloadStrings(Localization.LoadStringsFile(text2, false));
      }
      return text;
    }
  }
}

