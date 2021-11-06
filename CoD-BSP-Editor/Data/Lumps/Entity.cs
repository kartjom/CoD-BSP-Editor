using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Data
{
    public class Entity
    {
        public override string ToString() => this.Classname;

        public string Classname { get; set; }
        public List<KeyValuePair<string, string>> KeyValues = new();

        public Entity(string classname = "")
        {
            this.Classname = classname;
        }

        public string WriteEntity()
        {
            string entityString = "{\n";
            entityString += $"\"classname\" \"{this.Classname}\"\n";
            foreach (var (key, value) in KeyValues)
            {
                if (string.IsNullOrEmpty(key)) continue;

                entityString += $"\"{key}\" \"{value}\"\n";
            }
            entityString += "}";

            return entityString;
        }

        public static string WriteStringEntityLump(List<Entity> entities)
        {
            List<string> writtenEntsList = new List<string>();
            foreach (Entity ent in entities)
            {
                string writtenEnt = ent.WriteEntity();
                writtenEntsList.Add(writtenEnt);
            }

            string newEntityLump = String.Join("\n", writtenEntsList);
            newEntityLump += "\n\0\0\0";
            newEntityLump = newEntityLump.Replace("\n", "\r\n");

            return newEntityLump;
        }

        public static List<Entity> ParseEntitiesData(string _entityData)
        {
            List<Entity> parsedEntsList = new List<Entity>();

            string normalizedStr = _entityData
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            string[] rawEnts = normalizedStr.Split('{', '}');

            foreach (string raw in rawEnts)
            {
                string trimmedEnt = raw.Trim().Trim('\0');
                if (String.IsNullOrWhiteSpace(trimmedEnt)) continue;

                Entity parsedEntity = Entity.ParseRawEntity(trimmedEnt);
                parsedEntsList.Add(parsedEntity);
            }

            return parsedEntsList;
        }

        public static Entity ParseRawEntity(string _rawEntity)
        {
            Entity ent = new Entity();

            string[] keyValueLine = _rawEntity.Split('\n');

            foreach (string line in keyValueLine)
            {
                string trimmedLine = line.Trim().Trim('\t').Trim('"');
                string[] keyValue = trimmedLine.Split("\" \"");

                if (keyValue[0] == "classname")
                {
                    ent.Classname = keyValue[1];
                    continue;
                }

                if (keyValue.Length == 1)
                {
                    string key = keyValue[0].Replace("\"", "");
                    ent.KeyValues.Add(new(key, ""));
                    continue;
                }

                ent.KeyValues.Add( new (keyValue[0], keyValue[1]) );
            }

            return ent;
        }

        public bool HasKey(string keyName)
        {
            keyName = keyName.ToLower();

            foreach (var field in this.KeyValues)
            {
                if (field.Key.ToLower() == keyName)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetValue(string keyName)
        {
            keyName = keyName.ToLower();

            foreach (var field in this.KeyValues)
            {
                if (field.Key.ToLower() == keyName)
                {
                    return field.Value;
                }
            }

            return null;
        }
    
        public Entity DeepCopy()
        {
            Entity copy = new Entity();

            copy.Classname = this.Classname;

            foreach (var (key, value) in this.KeyValues)
            {
                copy.KeyValues.Add(new(key, value));
            }

            return copy;
        }
    }
}
