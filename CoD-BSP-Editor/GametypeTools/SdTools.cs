using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.GametypeTools
{
    public static class SdTools
    {
        public static string GetBombsiteEntities()
        {
            List<string> bombsiteEntitiesList = new()
            {
                "{", // 1 - Bombzone A
                "\"classname\" \"trigger_multiple\"",
                "\"targetname\" \"bombzone_A\"",
                "\"script_gameobjectname\" \"bombzone\"",
                "\"model\" \"*1\"",
                "}",
                "{", // 2 - Bombzone B
                "\"classname\" \"trigger_multiple\"",
                "\"targetname\" \"bombzone_B\"",
                "\"script_gameobjectname\" \"bombzone\"",
                "\"model\" \"*1\"",
                "}",
                "{", // 3 - Bomb
                "\"classname\" \"trigger_lookat\"",
                "\"targetname\" \"bombtrigger\"",
                "\"script_gameobjectname\" \"bombzone\"",
                "\"model\" \"*1\"",
                "}",
            };

            string bombsiteEntities = string.Join('\n', bombsiteEntitiesList);
            return bombsiteEntities;
        }
    }
}
