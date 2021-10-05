using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AsmToBnp
{
    class Program
    {
        static string name = null;
        static string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\asm";
        static string bcml = null;

        static async Task Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "auto=true")
                {
                    File.Create(root + "\\.auto");
                    Environment.Exit(0);
                }
                else if (args[0] == "auto=false")
                {
                    File.Delete(root + "\\.auto");
                    Environment.Exit(0);
                }
            }

            try
            {
                bcml = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\bcml\\settings.json")[7]
                    .Replace("  \"store_dir\": ", "").Replace("\"", "").Replace(",", "");
            }
            catch (Exception e)
            {
                Error("- - Error - -\n\nBCML Settings could not be found.\n\nIs BCML Installed and Setup?\n\t");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("pip install bcml\n");
                Console.ResetColor();

                Console.WriteLine("Details:");

                Error(e.Message);
                Console.ReadLine();
                Environment.Exit(1);
            }

            try
            {
                Directory.CreateDirectory(root);

                if (!File.Exists(root + "\\.cache"))
                {
                    ExtractEmbed("7z.resource", root + "\\.cache");
                }

                if (args.Length != 0)
                {
                    if (args[0].EndsWith(".asm"))
                    {
                        if (File.Exists(root + "\\.auto"))
                        {
                            name = GetName(args[0]).Replace("patch_", "").Replace(".asm\"", "");
                            await Handle(args[0]);
                        }
                        else
                        {
                            Console.WriteLine("Please enter your mod name.");
                            name = Console.ReadLine();

                            if (args[0] == "")
                                args[0] = GetName(args[0]).Replace("patch_", "").Replace(".asm", "");

                            await Handle(args[0]);

                            Console.WriteLine("Complete!");
                            Console.ReadLine();

                            Environment.Exit(1);
                        }
                    }
                }
                else
                {
                    Console.Write("Please drag any");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(" .asm ");
                    Console.ResetColor();
                    Console.Write("file over this window.\n");
                    var file = Console.ReadLine();

                    if (!file.EndsWith(".asm\""))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error\nInvalid file.\n");
                        Console.ResetColor();

                        await Main(args);
                    }

                    Console.WriteLine("Please enter your mod name.");
                    name = Console.ReadLine();

                    if (name == "")
                        name = GetName(file).Replace("patch_", "").Replace(".asm\"", "");

                    await Handle(file.Replace("\"", ""));

                    Console.WriteLine("Complete!");
                    Console.ReadLine();

                    await Main(args);
                }
            }
            catch (Exception e)
            {
                Directory.Delete(root + "\\.create", true);
                Error(e.Message);
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private static async Task Handle(string file)
        {
            Directory.CreateDirectory(root + "\\.create");
            Directory.CreateDirectory(root + "\\.create\\patches");
            File.WriteAllText(root + "\\.create\\info.json", Info(name, "wiiu", ModCount()));

            if (!File.Exists(GetPath(file) + "rules.txt"))
            {
                File.WriteAllText(root + "\\.create\\patches\\rules.txt", Rules(name, "", "The Legend of Zelda: Breath of the Wild/Patches/" + name));
            }
            else
            {
                File.Copy(GetPath(file) + "rules.txt", root + "\\.create\\patches\\rules.txt");
            }

            File.Copy(file, root + "\\.create\\patches\\" + GetName(file));

            Process _proc = new();
            _proc.StartInfo.FileName = root + "\\.cache";
            _proc.StartInfo.Arguments = "a " + name[0] + ".bnp info.json patches\\";
            _proc.StartInfo.CreateNoWindow = true;
            _proc.StartInfo.WorkingDirectory = root + "\\.create";

            _proc.Start();
            await _proc.WaitForExitAsync();

            File.Move(root + "\\.create\\" + name[0] + ".bnp", GetPath(file) + "\\" + name + ".bnp");
            Directory.Delete(root + "\\.create", true);
        }

        private static void ExtractEmbed(string fileName, string output)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("AsmToBnp." + fileName))
            using (BinaryReader binaryReader = new(stream))
            using (FileStream fileStream = new(output, FileMode.OpenOrCreate))
            using (BinaryWriter binaryWriter = new(fileStream))
                binaryWriter.Write(binaryReader.ReadBytes((int)stream.Length));
        }

        private static string GetPath(string input)
        {
            string[] arr = input.Split('\\');
            return input.Replace(arr[arr.Length - 1], "");
        }

        private static string GetName(string input)
        {
            string[] arr = input.Split('\\');
            return arr[arr.Length - 1];
        }

        public static string Info(string name, string platform, string priority)
        {
            return "{" +
            "\n    \"name\": \"" + name + "\"," +
            "\n    \"image\": \"\"," +
            "\n    \"url\": \"\"," +
            "\n    \"desc\": \"\"," +
            "\n    \"version\": \"1.0.0\"," +
            "\n    \"options\": {}," +
            "\n    \"depends\": []," +
            "\n    \"showCompare\": false," +
            "\n    \"showConvert\": false," +
            "\n    \"platform\": \"" + platform + "\"," +
            "\n    \"priority\": \"" + priority + "\"," +
            "\n    \"id\": \"\"" +
            "\n}";
        }

        public static string Rules(string name, string description, string path, string _default = "false")
        {
            return
                "[Definition]\n" +
                "titleIds = 00050000101C9300,00050000101C9400,00050000101C9500\n" +
                "name = \"" + name + "\"\n" +
                "path = \"" + path + "\"\n" +
                "description = \"" + description + "\"\n" +
                "version = 7\n" +
                "default = " + _default;
        }

        public static string ModCount()
        {
            string result = "0100";
            double count = Math.Floor(Math.Log10(Directory.GetDirectories(bcml + "\\mods").Length) + 1);
            int mods = Directory.GetDirectories(bcml + "\\mods").Length - 1;
            string modCount = mods.ToString();
            if (count == 1) { result = "010" + modCount; }
            else if (count == 2) { result = "01" + modCount; }
            else if (count == 3) { result = "0" + modCount; }
            else if (count == 4) { result = modCount; }
            else { Console.WriteLine("Error: BCML mod limit reached."); }

            return result;
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
