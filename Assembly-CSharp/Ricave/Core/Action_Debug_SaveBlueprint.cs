using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_SaveBlueprint : Action
    {
        public Vector3Int StartPosition
        {
            get
            {
                return this.startPos;
            }
        }

        public string BlueprintName
        {
            get
            {
                return this.blueprintName;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<Vector3Int, int>(this.startPos, 647823741);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.blueprintName);
            }
        }

        protected Action_Debug_SaveBlueprint()
        {
        }

        public Action_Debug_SaveBlueprint(ActionSpec spec, Vector3Int startPos, string blueprintName)
            : base(spec)
        {
            this.startPos = startPos;
            this.blueprintName = blueprintName;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            CellCuboid cellCuboid = this.FindBlueprintBounds();
            this.SaveBlueprint(cellCuboid);
            yield break;
        }

        private CellCuboid FindBlueprintBounds()
        {
            Vector3Int vector3Int = this.startPos;
            Vector3Int vector3Int2 = this.startPos + new Vector3Int(2, 0, 2);
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < 3; i++)
                {
                    Vector3Int vector3Int3 = vector3Int;
                    Vector3Int vector3Int4 = vector3Int2;
                    switch (i)
                    {
                        case 0:
                            {
                                int num = vector3Int4.x;
                                vector3Int4.x = num + 1;
                                break;
                            }
                        case 1:
                            {
                                int num = vector3Int4.z;
                                vector3Int4.z = num + 1;
                                break;
                            }
                        case 2:
                            {
                                int num = vector3Int4.y;
                                vector3Int4.y = num + 1;
                                break;
                            }
                    }
                    if (this.HasAnyEntityInExpansion(vector3Int, vector3Int2, vector3Int3, vector3Int4))
                    {
                        vector3Int = vector3Int3;
                        vector3Int2 = vector3Int4;
                        flag = true;
                    }
                }
            }
            Vector3Int vector3Int5 = vector3Int2 - vector3Int + Vector3Int.one;
            return new CellCuboid(vector3Int, vector3Int5.x, vector3Int5.y, vector3Int5.z);
        }

        private bool HasAnyEntityInExpansion(Vector3Int oldMin, Vector3Int oldMax, Vector3Int newMin, Vector3Int newMax)
        {
            for (int i = newMin.x; i <= newMax.x; i++)
            {
                for (int j = newMin.y; j <= newMax.y; j++)
                {
                    for (int k = newMin.z; k <= newMax.z; k++)
                    {
                        Vector3Int vector3Int = new Vector3Int(i, j, k);
                        if ((i < oldMin.x || i > oldMax.x || j < oldMin.y || j > oldMax.y || k < oldMin.z || k > oldMax.z) && vector3Int.InBounds() && Get.World.AnyEntityAt(vector3Int))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void SaveBlueprint(CellCuboid bounds)
        {
            Dictionary<char, EntitySpec> dictionary = new Dictionary<char, EntitySpec>();
            Dictionary<char, List<EntitySpec>> dictionary2 = new Dictionary<char, List<EntitySpec>>();
            int num = 0;
            List<string> list = new List<string>();
            for (int i = 0; i < bounds.height; i++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int j = bounds.depth - 1; j >= 0; j--)
                {
                    if (j != bounds.depth - 1)
                    {
                        stringBuilder.AppendLine();
                    }
                    for (int k = 0; k < bounds.width; k++)
                    {
                        Vector3Int vector3Int = bounds.Position + new Vector3Int(k, i, j);
                        List<Entity> list2 = (vector3Int.InBounds() ? Get.World.GetEntitiesAt(vector3Int) : new List<Entity>());
                        if (list2.Count > 0)
                        {
                            char c = ' ';
                            if (list2.Count > 1)
                            {
                                List<EntitySpec> list3 = list2.Select<Entity, EntitySpec>((Entity e) => e.Spec).ToList<EntitySpec>();
                                foreach (KeyValuePair<char, List<EntitySpec>> keyValuePair in dictionary2)
                                {
                                    if (keyValuePair.Value.Count == list3.Count && !keyValuePair.Value.Except<EntitySpec>(list3).Any<EntitySpec>() && !list3.Except<EntitySpec>(keyValuePair.Value).Any<EntitySpec>())
                                    {
                                        c = keyValuePair.Key;
                                        break;
                                    }
                                }
                                if (c == ' ')
                                {
                                    if (num >= Action_Debug_SaveBlueprint.CharacterPool.Length)
                                    {
                                        Log.Error(string.Format("Blueprint has too many unique entity combinations ({0}). Maximum supported is {1}.", num + 1, Action_Debug_SaveBlueprint.CharacterPool.Length), false);
                                        c = '?';
                                    }
                                    else
                                    {
                                        c = Action_Debug_SaveBlueprint.CharacterPool[num++];
                                    }
                                    dictionary2[c] = list3;
                                }
                            }
                            else
                            {
                                Entity entity = list2[0];
                                foreach (KeyValuePair<char, EntitySpec> keyValuePair2 in dictionary)
                                {
                                    if (keyValuePair2.Value == entity.Spec)
                                    {
                                        c = keyValuePair2.Key;
                                        break;
                                    }
                                }
                                if (c == ' ')
                                {
                                    if (num >= Action_Debug_SaveBlueprint.CharacterPool.Length)
                                    {
                                        Log.Error(string.Format("Blueprint has too many unique entity types ({0}). Maximum supported is {1}.", num + 1, Action_Debug_SaveBlueprint.CharacterPool.Length), false);
                                        c = '?';
                                    }
                                    else
                                    {
                                        c = Action_Debug_SaveBlueprint.CharacterPool[num++];
                                    }
                                    dictionary[c] = entity.Spec;
                                }
                            }
                            stringBuilder.Append(c);
                        }
                        else
                        {
                            stringBuilder.Append('.');
                        }
                    }
                }
                list.Add(stringBuilder.ToString());
            }
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.AppendLine("<Specs>");
            stringBuilder2.AppendLine("  ");
            stringBuilder2.AppendLine("  <BlueprintSpec>");
            stringBuilder2.AppendLine("    <specID>" + this.blueprintName + "</specID>");
            stringBuilder2.AppendLine("    <blueprint>");
            stringBuilder2.AppendLine("      <mapStrings>");
            foreach (string text in list)
            {
                stringBuilder2.AppendLine("        <li>");
                foreach (string text2 in text.Split('\n', StringSplitOptions.None))
                {
                    stringBuilder2.AppendLine("          " + text2.Trim());
                }
                stringBuilder2.AppendLine("        </li>");
            }
            stringBuilder2.AppendLine("      </mapStrings>");
            if (dictionary.Count > 0)
            {
                stringBuilder2.AppendLine("      <mapCharToEntity>");
                foreach (KeyValuePair<char, EntitySpec> keyValuePair3 in dictionary.OrderBy<KeyValuePair<char, EntitySpec>, char>((KeyValuePair<char, EntitySpec> x) => x.Key))
                {
                    stringBuilder2.AppendLine(string.Format("        <{0}>{1}</{2}>", keyValuePair3.Key, keyValuePair3.Value.SpecID, keyValuePair3.Key));
                }
                stringBuilder2.AppendLine("      </mapCharToEntity>");
            }
            if (dictionary2.Count > 0)
            {
                stringBuilder2.AppendLine("      <mapCharToEntities>");
                foreach (KeyValuePair<char, List<EntitySpec>> keyValuePair4 in dictionary2.OrderBy<KeyValuePair<char, List<EntitySpec>>, char>((KeyValuePair<char, List<EntitySpec>> x) => x.Key))
                {
                    stringBuilder2.AppendLine(string.Format("        <{0}>", keyValuePair4.Key));
                    foreach (EntitySpec entitySpec in keyValuePair4.Value)
                    {
                        stringBuilder2.AppendLine("          <li>" + entitySpec.SpecID + "</li>");
                    }
                    stringBuilder2.AppendLine(string.Format("        </{0}>", keyValuePair4.Key));
                }
                stringBuilder2.AppendLine("      </mapCharToEntities>");
            }
            stringBuilder2.AppendLine("    </blueprint>");
            stringBuilder2.AppendLine("  </BlueprintSpec>");
            stringBuilder2.AppendLine();
            stringBuilder2.AppendLine("</Specs>");
            File.WriteAllText("Blueprint.txt", stringBuilder2.ToString());
            Log.Message("Blueprint saved to: Blueprint.txt");
        }

        [Saved]
        private Vector3Int startPos;

        [Saved]
        private string blueprintName;

        private static readonly char[] CharacterPool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    }
}