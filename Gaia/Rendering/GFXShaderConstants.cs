using System;
using System.Collections.Generic;
using System.IO;

namespace Gaia.Rendering
{
    public static class GFXShaderConstants
    {
        public static int VC_MODELVIEW = 0;
        public static int VC_WORLD = 14;
        public static int VC_TEXGEN = 8;
        public static int VC_EYEPOS = 12;
        public static int VC_INVTEXRES = 13;

        public static int PC_EYEPOS = 8;
        public static int PC_FARPLANE = 9;
        public static int PC_LIGHTPOS = 10;
        public static int PC_LIGHTCOLOR = 11;
        public static int PC_LIGHTPARAMS = 12;

        public static int NUM_SPLITS = 4;
        
        public static int NUM_INSTANCES = 60;

        public static int PC_LIGHTMODELVIEW = 13;

        public static int PC_LIGHTCLIPPLANE = PC_LIGHTMODELVIEW + 4 * NUM_SPLITS;

        public static int PC_LIGHTCLIPPOS = PC_LIGHTCLIPPLANE + NUM_SPLITS;

        public static int PC_INVSHADOWRES = PC_LIGHTCLIPPOS + NUM_SPLITS;

        static void WriteCommand(StreamWriter writer, string commandName, int index)
        {
            writer.Write("#define ");
            writer.Write(commandName);
            writer.Write(" C");
            writer.Write(index);
            writer.Write("\n");
        }

        static void WriteDefine(StreamWriter writer, string commandName, int index)
        {
            writer.Write("#define ");
            writer.Write(commandName);
            writer.Write(" ");
            writer.Write(index);
            writer.Write("\n");
        }

        public static void AuthorShaderConstantFile()
        {
            using (FileStream fs = new FileStream("Shaders/ShaderConst.h", FileMode.Create))
            {
                using (StreamWriter wr = new StreamWriter(fs))
                {
                    WriteDefine(wr, "NUM_INSTANCES", NUM_INSTANCES); //Instancing
                    WriteDefine(wr, "NUM_SPLITS", NUM_SPLITS); //Cascade shadow maps
                    WriteCommand(wr, "VC_MODELVIEW", VC_MODELVIEW);
                    WriteCommand(wr, "VC_WORLD", VC_WORLD);
                    WriteCommand(wr, "VC_EYEPOS", VC_EYEPOS);
                    WriteCommand(wr, "VC_INVTEXRES", VC_INVTEXRES);
                    WriteCommand(wr, "VC_TEXGEN", VC_TEXGEN);
                    WriteCommand(wr, "PC_EYEPOS", PC_EYEPOS);
                    WriteCommand(wr, "PC_FARPLANE", PC_FARPLANE);
                    WriteCommand(wr, "PC_LIGHTPOS", PC_LIGHTPOS);
                    WriteCommand(wr, "PC_LIGHTCOLOR", PC_LIGHTCOLOR);
                    WriteCommand(wr, "PC_LIGHTPARAMS", PC_LIGHTPARAMS);
                    WriteCommand(wr, "PC_LIGHTMODELVIEW", PC_LIGHTMODELVIEW);
                    WriteCommand(wr, "PC_LIGHTCLIPPLANE", PC_LIGHTCLIPPLANE);
                    WriteCommand(wr, "PC_LIGHTCLIPPOS", PC_LIGHTCLIPPOS);
                    WriteCommand(wr, "PC_INVSHADOWRES", PC_INVSHADOWRES);
                }
            }

        }
    }
}
