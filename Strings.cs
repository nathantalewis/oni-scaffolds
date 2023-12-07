using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace Scaffolds
{/*
    public class STRINGS
    {
        /*public class Scaffolds
        {
            public static LocString DisplayName = "Scaffold";
            public static LocString Description = "A temporary scaffold that will be built immediately with no materials.";
            public static LocString Effect = "A temporary way to move vertically.";

            public static LocString DeconstructButtonText = "Remove";
            public static LocString DeconstructButtonTooltip = "Instantly remove this scaffold";
            public static LocString SelfDestructButtonText = "Enable Self Destruct";
            public static LocString SelfDestructButtonTooltip = "When enabled, automatically remove scaffold after some time has passed";
            public static LocString SelfDestructButtonCancelText = "Make Permanent";
            public static LocString SelfDestructButtonCancelTooltip = "When enabled, this scaffold will remain until manually deconstructed";
        }*/

    //}
        // Token: 0x02000005 RID: 5
       // [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Loc_Initialize_Patch
        {
            /*
            public static void Prefix()
            {
                Loc_Initialize_Patch.Translate(typeof(ScaffoldConfig));
            /*ScaffoldConfig.DisplayName = STRINGS.Scaffolds.DisplayName;
            ScaffoldConfig.Description = STRINGS.Scaffolds.Description;
            ScaffoldConfig.Effect = STRINGS.Scaffolds.Effect;
            ScaffoldConfig.SelfDestructButtonCancelText = STRINGS.Scaffolds.SelfDestructButtonCancelText;
            ScaffoldConfig.SelfDestructButtonCancelTooltip = STRINGS.Scaffolds.SelfDestructButtonCancelTooltip;
            ScaffoldConfig.SelfDestructButtonText = STRINGS.Scaffolds.SelfDestructButtonText;
            ScaffoldConfig.SelfDestructButtonTooltip = STRINGS.Scaffolds.SelfDestructButtonTooltip; 
            ScaffoldConfig.DeconstructButtonText = STRINGS.Scaffolds.DeconstructButtonText;
            ScaffoldConfig.DeconstructButtonTooltip = STRINGS.Scaffolds.DeconstructButtonTooltip;*/

        //}

            // Token: 0x0600000D RID: 13 RVA: 0x00002558 File Offset: 0x00000758
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

