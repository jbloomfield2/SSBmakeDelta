using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using xdelta3.net;

namespace SSBmakeDelta
{
    class Program
    {
        static void Main(string[] args)
        {
            bool rdb = false;
            if (args.Length > 1)
                if (args[1] == "-rdb")
                    rdb = true;
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
                if (type == ".z64" && rdb)
                    makeRDB(selected);
                else if (type == ".z64")
                {
                    Console.WriteLine("Creating Xdelta patch...");
                    encode(smashRom, selected);
                }
                else if (type == ".xdelta")
                {
                    Console.WriteLine("Patching rom...");
                    decode(smashRom, selected);
                }
                    
            }
        }

        private static void makeRDB(string selected)
        {
            downloadRDB();

            FileStream fs = new FileStream(selected,FileMode.Open, FileAccess.Read);
            string header = "";
            string hex;
            int hexin;
            
            for (int i = 0; i < 24; i++)
            {
                hexin = fs.ReadByte();
                hex = string.Format("{0:X2}", hexin);
                header += hex;
            }
            string crc1 = header.Substring(32, 8);
            string crc2 = header.Substring(40, 8);
            Console.WriteLine("Enter Goodname:");
            string goodname = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Enter Internal Name:");
            string intName = Console.ReadLine();

            if (File.Exists(Path.Combine(Path.GetTempPath(), "pj64.rdb")))
                {
                StreamWriter s = File.AppendText(Path.Combine(Path.GetTempPath(), "pj64.rdb"));


                s.WriteLine("");
                s.WriteLine("[" + crc1 + "-" + crc2 + "C:45}");
                s.WriteLine("Counter Factor=1");
                s.WriteLine("Culling=1");
                s.WriteLine("Good Name="+goodname);
                s.WriteLine("Internal Name="+intName);
                s.WriteLine("RDRAM Size=8");
                s.WriteLine("SMM-Cache=0");
                s.WriteLine("SMM-FUNC=0");
                s.WriteLine("SMM-TLB=0");
                s.WriteLine("Status=Compatible");
                s.WriteLine("ViRefresh=2200");
                s.WriteLine("");
                s.Close();
            }
            string path = Path.GetDirectoryName(selected);
            File.Copy(Path.Combine(Path.GetTempPath(), "pj64.rdb"),Path.Combine(path,"pj64.rdb"),true);


        }

        

        private static void downloadRDB()
        {
            string location = Path.Combine(Path.GetTempPath(), "pj64.rdb");
            string url = "https://raw.githubusercontent.com/smash64-dev/project64k-legacy/main/Cfg/pj64.rdb";
            WebClient wc = new WebClient();
            wc.DownloadFile(url, location);
        }

        private static void encode(string orig, string changed)
        {
            var source = File.ReadAllBytes(orig);
            var target = File.ReadAllBytes(changed);
            var delta = Xdelta3Lib.Encode(source, target);
            var fileName = Path.ChangeExtension(changed, ".xdelta");
            Console.WriteLine(fileName);
            File.WriteAllBytes(fileName, delta.ToArray());
        }

        private static void decode(string orig, string delta)
        {
            var source = File.ReadAllBytes(orig);
            var target = File.ReadAllBytes(delta);
            var fileName = Path.ChangeExtension(delta, ".z64");
            Console.WriteLine(fileName);
            var rom = Xdelta3Lib.Decode(source, target);
            File.WriteAllBytes(fileName, rom.ToArray());
        }
    }
}
