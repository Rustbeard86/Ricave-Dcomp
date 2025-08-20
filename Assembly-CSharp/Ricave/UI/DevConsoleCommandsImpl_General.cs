using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ricave.Core;

namespace Ricave.UI
{
    public static class DevConsoleCommandsImpl_General
    {
        public static void Help(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("All available commands:");
            foreach (DevConsoleCommands.Command command in DevConsoleCommandsList.Commands)
            {
                stringBuilder.AppendLine().Append(command.NameWithArgsNames.PadRight(23)).Append(" - ")
                    .Append(command.Description);
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Log_Specs(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Specs loaded:");
            foreach (Type type in typeof(Spec).AllSubclasses())
            {
                if (!type.IsAbstract)
                {
                    StringBuilder stringBuilder2 = stringBuilder.AppendLine();
                    Type type2 = type;
                    stringBuilder2.Append(((type2 != null) ? type2.ToString() : null) + ": " + Get.Specs.GetAll(type).Count.ToString());
                }
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Log_Specs_OfType(string[] args)
        {
            string text = args[0];
            Type type = TypeUtility.GetType(text);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Specs of type ").Append(text).Append(":");
            foreach (Spec spec in Get.Specs.GetAll(type))
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(spec.SpecID);
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Log_Actors(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Actors:");
            using (IEnumerator<EntitySpec> enumerator = (from x in Get.Specs.GetAll<EntitySpec>()
                                                         where x.IsActor
                                                         orderby x.Actor.GenerateMinFloor
                                                         select x).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    EntitySpec actor = enumerator.Current;
                    string text = string.Join<int>(", ", from x in Enumerable.Range(0, 5)
                                                         select RampUpUtility.ApplyRampUpToInt(actor.MaxHP, x, actor.Actor.MaxHPRampUpFactor));
                    NativeWeaponProps nativeWeaponProps = actor.Actor.NativeWeapons.ElementAtOrDefault<NativeWeaponProps>(0);
                    object obj;
                    if (nativeWeaponProps == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        UseEffects defaultUseEffects = nativeWeaponProps.DefaultUseEffects;
                        obj = ((defaultUseEffects != null) ? defaultUseEffects.GetFirstOfSpec(Get.UseEffect_Damage) : null);
                    }
                    UseEffect_Damage useEffect_Damage = (UseEffect_Damage)obj;
                    stringBuilder.AppendLine().Append(string.Concat(new string[]
                    {
                        actor.SpecID,
                        ": hp=",
                        actor.MaxHP.ToString(),
                        " damage=",
                        ((useEffect_Damage != null) ? new int?(useEffect_Damage.From) : null).ToString(),
                        "-",
                        ((useEffect_Damage != null) ? new int?(useEffect_Damage.To) : null).ToString(),
                        " exp=",
                        actor.Actor.KilledExperience.ToString(),
                        " min_floor=",
                        actor.Actor.GenerateMinFloor.ToString(),
                        " hp_ramp-up=",
                        text
                    }));
                }
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Log_BodyParts(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Body parts:");
            foreach (EntitySpec entitySpec in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec.IsActor)
                {
                    stringBuilder.AppendLine().Append(entitySpec.SpecID + " (hp=" + entitySpec.MaxHP.ToString() + ")");
                    foreach (BodyPartPlacement bodyPartPlacement in entitySpec.Actor.BodyParts)
                    {
                        StringBuilder stringBuilder2 = stringBuilder.AppendLine();
                        string[] array = new string[8];
                        array[0] = "  - ";
                        array[1] = bodyPartPlacement.LabelCap;
                        array[2] = ": hp=";
                        array[3] = Math.Max(Calc.RoundToIntHalfUp((float)entitySpec.MaxHP * (bodyPartPlacement.MaxHPPctOverride ?? bodyPartPlacement.Spec.MaxHPPct)), 1).ToString();
                        array[4] = " condition=";
                        int num = 5;
                        Condition conditionOnDestroyed = bodyPartPlacement.Spec.ConditionOnDestroyed;
                        array[num] = ((conditionOnDestroyed != null) ? conditionOnDestroyed.ToString() : null);
                        array[6] = " conditionAll=";
                        int num2 = 7;
                        Condition conditionOnAllDestroyed = bodyPartPlacement.Spec.ConditionOnAllDestroyed;
                        array[num2] = ((conditionOnAllDestroyed != null) ? conditionOnAllDestroyed.ToString() : null);
                        stringBuilder2.Append(string.Concat(array));
                    }
                }
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Log_Leveling(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Experience required to level up:");
            for (int i = 1; i <= 15; i++)
            {
                int num = ((i == 1) ? 0 : (LevelUtility.GetTotalExperienceRequired(i) - LevelUtility.GetTotalExperienceRequired(i - 1)));
                stringBuilder.AppendInNewLine(string.Concat(new string[]
                {
                    i.ToString(),
                    ": ",
                    num.ToString(),
                    " (total ",
                    LevelUtility.GetTotalExperienceRequired(i).ToString(),
                    ")"
                }));
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Log_Permanent(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Permanent:");
            foreach (EntitySpec entitySpec in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec.IsPermanent)
                {
                    stringBuilder.AppendInNewLine("  - " + entitySpec.SpecID);
                }
            }
            stringBuilder.AppendInNewLine("Permanent filled impassable:");
            foreach (EntitySpec entitySpec2 in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec2.IsPermanentFilledImpassable)
                {
                    stringBuilder.AppendInNewLine("  - " + entitySpec2.SpecID);
                }
            }
            stringBuilder.AppendInNewLine("Semi-permanent:");
            foreach (EntitySpec entitySpec3 in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec3.IsSemiPermanent)
                {
                    stringBuilder.AppendInNewLine("  - " + entitySpec3.SpecID);
                }
            }
            stringBuilder.AppendInNewLine("Semi-permanent filled impassable:");
            foreach (EntitySpec entitySpec4 in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec4.IsSemiPermanentFilledImpassable)
                {
                    stringBuilder.AppendInNewLine("  - " + entitySpec4.SpecID);
                }
            }
            stringBuilder.AppendInNewLine("None gravity structures:");
            foreach (EntitySpec entitySpec5 in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec5.IsStructure && entitySpec5.Structure.FallBehavior == StructureFallBehavior.None)
                {
                    stringBuilder.AppendInNewLine("  - " + entitySpec5.SpecID);
                }
            }
            stringBuilder.AppendInNewLine("None gravity, non-permanent structures:");
            foreach (EntitySpec entitySpec6 in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec6.IsStructure && !entitySpec6.IsPermanent && entitySpec6.Structure.FallBehavior == StructureFallBehavior.None)
                {
                    stringBuilder.AppendInNewLine("  - " + entitySpec6.SpecID);
                }
            }
            Log.Message(stringBuilder.ToString());
        }

        public static void Get_Pref(string[] args)
        {
            string text = args[0];
            Log.Message(text + ": " + Prefs.GetString(text, null));
        }

        public static void Set_Pref(string[] args)
        {
            string text = args[0];
            string text2 = args[1];
            Prefs.SetString(text, text2);
        }

        public static void Get_UIScale(string[] args)
        {
            Log.Message("UI scale: " + Widgets.UIScale.ToString());
        }

        public static void Set_UIScale(string[] args)
        {
            Widgets.UIScaleFactor = float.Parse(args[0]) / (Widgets.UIScale / Widgets.UIScaleFactor);
        }

        public static void Set_FontScale(string[] args)
        {
            Widgets.FontScale = float.Parse(args[0]);
        }

        public static void Set_UI(string[] args)
        {
            DebugUI.HideUI = args[0] == "0";
        }

        public static void Set_TrailerMode(string[] args)
        {
            DebugUI.TrailerMode = args[0] != "0";
        }

        public static void Save(string[] args)
        {
            string text = args[0];
            SaveLoadManager.Save(Get.Run, FilePaths.Savefile(text), "Run");
        }

        public static void Load(string[] args)
        {
            Root.LoadPlayScene(RootOnSceneChanged.LoadRun(args[0]));
        }

        public static void GenerateTranslationTemplate(string[] args)
        {
            TranslationTemplateGenerator.GenerateTranslationTemplate();
        }

        public static void SetLanguage(string[] args)
        {
            string text = args[0];
            Get.Languages.SetActiveLanguageInPrefs(text);
            Get.ModManager.ReloadAllContent();
        }
    }
}