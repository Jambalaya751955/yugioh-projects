using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Data.SQLite;

using CDBUpdater.Helpers;
using CDBUpdater.Object;
using CDBUpdater.SQLite;
using CDBUpdater.SQLite.CDB;

namespace CDBUpdater
{
    class Menu
    {
        public static Object.Input GetInput()
        {
            Console.Title = "CDBUpdater © Snrk (YGOPro Percy Team) 2016";
            Object.Input Input;

            while (true)
            {
                Input = new Input();
                input = new List<string>();

                while (true)
                {
                    Console.Clear(); PrintStage(1); Input.CdbPath = Console.ReadLine();
                    if (Input.CdbPath.IsEmpty() || !File.Exists(Input.CdbPath))
                    {
                        Console.WriteLine("\n [!] The entered Path does not exist.\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    input.Add(Input.CdbPath);
                    break;
                }
                while (true)
                {
                    Console.Clear(); PrintStage(2); Input.SavingDirectory = Console.ReadLine();
                    if (Input.SavingDirectory.IsEmpty() || !Directory.Exists(Input.SavingDirectory))
                    {
                        Console.WriteLine("\n [!] The entered Path does not exist.\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    input.Add(Input.SavingDirectory);
                    break;
                }
                Input.SavingDirectory = Input.SavingDirectory.TrimEnd(new char[] { '\\', '/' });
                while (true)
                {
                    Console.Clear(); PrintStage(3); Input.FormatDirectory = Console.ReadLine();
                    Input.FormatDirectory = Input.FormatDirectory.TrimEnd(new char[] { '\\', '/' });
                    if (Input.FormatDirectory.IsEmpty() || !File.Exists(Input.FormatDirectory + "/desc.format.txt"))
                    {
                        Console.WriteLine("\n [!] The entered Directory doesn't contain 'desc.format.txt'.\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    input.Add(Input.FormatDirectory);
                    break;
                }
                
                Regex langReg = new Regex("^(en|ja|de|fr|it|es)$");
                while (true)
                {
                    Console.Clear(); PrintStage(4); Input.Language = Console.ReadLine();
                    if (Input.Language.IsEmpty() || !langReg.IsMatch(Input.Language.ToLower()))
                    {
                        Console.WriteLine("\n [!] The entered language is not supported.\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    input.Add(Input.Language);
                    break;
                }
                Input.Language = Input.Language.ToLower();

                Console.Clear(); PrintStage(5); string inpLoadWikia = Console.ReadLine(); input.Add(inpLoadWikia);
                Console.Clear(); PrintStage(6); string inpLoadYGODB = Console.ReadLine(); input.Add(inpLoadYGODB);
                Input.Load.Wikia = inpLoadWikia.IsEmpty() || inpLoadWikia.ToLower() == "y";
                Input.Load.YGODB = inpLoadWikia.IsEmpty() || inpLoadYGODB.ToLower() == "y";

                Console.Clear(); PrintStage(7); string inpLoadPack = Console.ReadLine(); input.Add(inpLoadPack);
                Input.Load.Pack = inpLoadPack.IsEmpty() || inpLoadPack.ToLower() == "y";

                string boolInp = "";
                while (true)
                {
                    Console.Clear(); PrintStage(8); boolInp = Console.ReadLine().ToLower().RemoveWhitespace();
                    if (boolInp.Length < 3 && !boolInp.IsEmpty())
                    {
                        Console.WriteLine("\n [!] The input is invalid.\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    input.Add(boolInp);
                    boolInp = boolInp.Length >= 3 ? boolInp.Substring(0, 3) : boolInp;
                    break;
                }

                if (boolInp.IsEmpty())
                    Input.Load.Name = Input.Load.Desc = Input.Load.Ot = true;
                else
                {
                    Input.Load.Name = boolInp.Substring(0, 1) == "y" ? true : false;
                    Input.Load.Desc = boolInp.Substring(1, 1) == "y" ? true : false;
                    Input.Load.Ot = boolInp.Substring(2, 1) == "y" ? true : false;
                }

                Console.Clear(); PrintStage(9); string inpLoadAll = Console.ReadLine().ToLower(); input.Add(inpLoadAll);
                Input.Load.All = inpLoadAll.IsEmpty() || inpLoadAll.ToLower() == "y";

                while (!Input.Load.All)
                {
                    Console.Clear(); PrintStage(10); string inpMonths = Console.ReadLine().ToLower();
                    int months = 0;
                    if (!int.TryParse(inpMonths, out months))
                    {
                        Console.WriteLine("\n [!] Invalid input: NaN\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    if (months < 1)
                    {
                        Console.WriteLine("\n [!] Number cannot be less than 1\n (Press any key to re-enter)");
                        Console.ReadLine(); continue;
                    }
                    input.Add(inpMonths);
                    Input.Months = months;
                    
                    while (true)
                    {
                        Console.Clear(); PrintStage(11); Input.PackPath = Console.ReadLine();
                        if (Input.PackPath.IsEmpty() || !File.Exists(Input.PackPath))
                        {
                            Console.WriteLine("\n [!] The entered Path does not exist. (Press any key to re-enter)\n (Press any key to re-enter)");
                            Console.ReadLine(); continue;
                        }
                        Input.Packs = SQLite.Commands.ExecuteMethod(Input.PackPath, Load.Table, "pack", new string[] { "id", "date" });
                        if (Input.Packs == null || Input.Packs.Count < 1)
                        {
                            Console.WriteLine("\n [!] The entered Cdb is empty or has an invalid format\n (Press any key to re-enter)");
                            Console.ReadLine(); continue;
                        }

                        input.Add(Input.PackPath);
                        break;
                    }
                    break;
                }

                Console.Clear();

                Console.Write(" " + String.Concat(Enumerable.Repeat('_', Console.WindowWidth - 2)) + " " +
                    '\n' + stages[0] + Input.CdbPath +
                    '\n' + stages[1] + Input.SavingDirectory +
                    '\n' + stages[2] + Input.FormatDirectory +
                    '\n' + stages[3] + Input.Language +
                    '\n' + stages[4].Replace("(y/n)", "") + Input.Load.Wikia.ToString().ToLower() +
                    '\n' + stages[5].Replace("(y/n)", "") + Input.Load.YGODB.ToString().ToLower() +
                    '\n' + stages[6].Replace("(y/n)", "") +
                    '\n' + stages[7].Replace("(yyy-nnn)", "") + Input.Load.Name.ToString().ToLower() + ", " +
                        Input.Load.Desc.ToString().ToLower() + ", " + Input.Load.Ot.ToString().ToLower() +
                    '\n' + stages[8].Replace("(y/n)", "") + Input.Load.All.ToString().ToLower() +
                    (!Input.Load.All ?
                        "\n >" + stages[9] + Input.Months +
                        "\n >" + stages[10] + Input.PackPath
                    : "" ) +
                    "\n " + String.Concat(Enumerable.Repeat('_', Console.WindowWidth - 2)) + " " +
                    "\n Confirm Input (y/n)?: ");
                string inp = Console.ReadLine();
                if (inp.IsEmpty() || inp.ToLower() == "y")
                    break;
            }
            
            Console.Clear();
            return Input;
        }

        private static List<string> input;
        private static string[] stages = new string[] {
                " Path to input cdb: ", " Directory of output cdb: ", " Directory of desc.format.txt: ",
                " Language of output cdb: ", " Load from 'yugioh.wikia.com'?(y/n): ",
                " Load from 'db.yugioh-card.com'?(y/n): ", " Load packs a card appeared in?(y/n): ",
                " Load the following: 'name', 'desc', 'ot'?(yyy-nnn): ", " Update all cards?(y/n): ",
                " Update from how many months ago?: ", " Path to pack cdb to retrieve release dates: "};

        private static void PrintStage(int stage)
        {
            Console.WriteLine("\n Languages: English (en), German (de), Italian (it), French (fr),\n            Spanish (es), Japanese (ja)");
            Console.WriteLine(" " + String.Concat(Enumerable.Repeat('_', Console.WindowWidth - 2)) + " ");
            
            for (int i = 0; i < stage; ++i)
                if (i == stage - 1)
                    Console.Write(stages[i] + (input.Count > i ? input[i] : string.Empty));
                else
                    Console.WriteLine(stages[i] + (input.Count > i ? input[i] : string.Empty));
        }
    }
}
