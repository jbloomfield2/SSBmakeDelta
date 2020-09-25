using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xdelta3.net;

namespace SSBmakeDelta
{
    class Program
    {
        static void Main(string[] args)
        {
            string smashRom = @"C:\GEEdit4\Super Smash Bros. (USA).z64";
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\rompath.ini"))
            {
                smashRom = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\rompath.ini");
            }
            else
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\rompath.ini", smashRom);

            string selected = args.First();
            string type = Path.GetExtension(selected);
            if(File.Exists(selected) && File.Exists(smashRom))
            {
                if (type == ".z64")
                    encode(smashRom, selected);
                if (type == ".xdelta")
                    decode(smashRom, selected);
            }
        }

        private static void encode(string orig, string changed)
        {
            var source = File.ReadAllBytes(orig);
            var target = File.ReadAllBytes(changed);
            var delta = Xdelta3Lib.Encode(source, target);
            var fileName = Path.ChangeExtension(changed, ".xdelta");
            File.WriteAllBytes(fileName, delta.ToArray());
        }

        private static void decode(string orig, string delta)
        {
            var source = File.ReadAllBytes(orig);
            var target = File.ReadAllBytes(delta);
            var fileName = Path.ChangeExtension(delta, ".z64");
            var rom = Xdelta3Lib.Decode(source, target);
            File.WriteAllBytes(fileName, rom.ToArray());
        }
    }
}
