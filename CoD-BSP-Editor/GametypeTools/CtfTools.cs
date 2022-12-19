﻿using CoD_BSP_Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.GametypeTools
{
    public static class CtfTools
    {
        public static string GetAlliesFlagEntities(string modelIndex, string alliesFlagPos)
        {
            List<string> alliesFlagEntities = new List<string>()
            {
                "{", // 1
                "\"model\" \"*MODEL*\"",
                "\"origin\" \"*ALLIES_POS*\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"classname\" \"trigger_multiple\"",
                "\"targetname\" \"jesse24\"",
                "}",
                "{", // 2
                "\"model\" \"*MODEL*\"",
                "\"origin\" \"*ALLIES_POS*\"",
                "\"targetname\" \"axis_ctf_pickup\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"classname\" \"trigger_multiple\"",
                "}",
                "{", // 3
                "\"model\" \"*MODEL*\"",
                "\"origin\" \"*ALLIES_POS*\"",
                "\"targetname\" \"allied_drop\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"classname\" \"trigger_multiple\"",
                "}",
                "{", // 4
                "\"origin\" \"*ALLIES_POS*\"",
                "\"target\" \"jesse25\"",
                "\"targetname\" \"axis_ctf_pickup\"",
                "\"classname\" \"mp_gmi_ctf_flag\"",
                "\"script_gameobjectname\" \"ctf\"",
                "}",
                "{", // 5
                "\"origin\" \"*ALLIES_POS*\"",
                "\"target\" \"axis_ctf_pickup\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"ctf_flag_allies\"",
                "\"model\" \"xmodel/mp_ctf_flag_usa60\"",
                "\"classname\" \"script_model\"",
                "}",
                "{", // 6
                "\"origin\" \"*ALLIES_POS*\"",
                "\"target\" \"allied_drop\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"ctf_flag_allies_mobile\"",
                "\"model\" \"xmodel/mp_ctf_flag_usa40\"",
                "\"classname\" \"script_model\"",
                "}",
            };

            string alliesFlagEntitiesContent = string.Join('\n', alliesFlagEntities);

            alliesFlagEntitiesContent = alliesFlagEntitiesContent.Replace("*MODEL*", modelIndex);
            alliesFlagEntitiesContent = alliesFlagEntitiesContent.Replace("*ALLIES_POS*", alliesFlagPos);

            return alliesFlagEntitiesContent;
        }

        public static string GetAxisFlagEntities(string modelIndex, string axisFlagPos)
        {
            List<string> axisFlagEntities = new List<string>()
            {
                "{", // 1
                "\"model\" \"*MODEL*\"",
                "\"origin\" \"*AXIS_POS*\"",
                "\"classname\" \"trigger_multiple\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"jesse25\"",
                "}",
                "{", // 2
                "\"origin\" \"*AXIS_POS*\"",
                "\"classname\" \"mp_gmi_ctf_flag\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"allies_ctf_pickup\"",
                "\"target\" \"jesse24\"",
                "}",
                "{", // 3
                "\"model\" \"*MODEL*\"",
                "\"origin\" \"*AXIS_POS*\"",
                "\"classname\" \"trigger_multiple\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"allies_ctf_pickup\"",
                "}",
                "{", // 4
                "\"origin\" \"*AXIS_POS*\"",
                "\"model\" \"xmodel/mp_ctf_flag_ge60\"",
                "\"classname\" \"script_model\"",
                "\"targetname\" \"ctf_flag_axis\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"target\" \"allies_ctf_pickup\"",
                "}",
                "{", // 5
                "\"origin\" \"*AXIS_POS*\"",
                "\"target\" \"axis_drop\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"ctf_flag_axis_mobile\"",
                "\"model\" \"xmodel/mp_ctf_flag_ge40\"",
                "\"classname\" \"script_model\"",
                "}",
                "{", // 6
                "\"model\" \"*MODEL*\"",
                "\"origin\" \"*AXIS_POS*\"",
                "\"classname\" \"trigger_multiple\"",
                "\"script_gameobjectname\" \"ctf\"",
                "\"targetname\" \"axis_drop\"",
                "}",
            };

            string axisFlagEntitiesContent = string.Join('\n', axisFlagEntities);

            axisFlagEntitiesContent = axisFlagEntitiesContent.Replace("*MODEL*", modelIndex);
            axisFlagEntitiesContent = axisFlagEntitiesContent.Replace("*AXIS_POS*", axisFlagPos);

            return axisFlagEntitiesContent;
        }
        
        public static ModelData CreateModel()
        {
            ModelData modelData = new ModelData();

            modelData.Shaders.Add( new Shader("textures/common/trigger", 128, 671088641) );
            modelData.Brushes.Add( new BrushVolume(new Vector3(-64, -64, 0), new Vector3(64, 64, 152)) );

            return modelData;
        }
    }
}
