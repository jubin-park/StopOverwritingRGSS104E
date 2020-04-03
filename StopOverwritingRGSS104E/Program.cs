using System;
using System.IO;
using Microsoft.Win32;

namespace StopOverwritingRGSS104E
{
    class Program
    {
        static void Main(string[] args)
        {
            string installPath = GetSteamInstallPathOrNull();
            if (installPath == null)
            {
                Console.WriteLine("Steam 이 설치되지 않았습니다.");
                return;
            }
            string vdfPath = installPath + "\\steamapps\\libraryfolders.vdf";
            if (!File.Exists(vdfPath))
            {
                Console.WriteLine($"\"{vdfPath}\" 파일이 발견되지 않았습니다.");
                return;
            }
            string rpgxpPath = GetRPGXPPathOrNull(vdfPath);
            if (rpgxpPath == null)
            {
                Console.WriteLine("Steam RPGXP 가 설치되지 않았습니다.");
                return;
            }
            string rgss104ePath = rpgxpPath + "\\RGSS104E.dll";
            string line;
            Console.WriteLine("> 명령어를 입력하세요. (d/r/o)");
            Console.WriteLine("> d : 삭제");
            Console.WriteLine("> r : 복구");
            Console.WriteLine("> o : 탐색기 실행");
            Console.WriteLine(new string('-', 20));
            Console.Write("> ");
            while ((line = Console.ReadLine()) != null)
            {
                string cmd = line.Trim();
                if (cmd == "d" || cmd == "D")
                {
                    if (File.Exists(rgss104ePath))
                    {
                        Console.WriteLine($"{rgss104ePath} 파일을 삭제합니다..");
                        File.Delete(rgss104ePath);
                    }
                    else
                    {
                        Console.WriteLine($"{rgss104ePath} 파일이 이미 지워진 상태입니다.");
                    }
                }
                else if (cmd == "r" || cmd == "R")
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\RGSS104E.dll", rgss104ePath, true);
                    Console.WriteLine($"{rgss104ePath} 파일을 복구했습니다.");
                }
                else if (cmd == "o" || cmd == "O")
                {
                    System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(rgss104ePath));
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }
                Console.Write("> ");
            }
        }

        static string GetRPGXPPathOrNull(string path)
        {
            var s = File.ReadAllText(path);
            string[] lines = s.Split('\n');
            foreach (var format in lines)
            {
                if (format[0] != '\t') continue;
                string libraryPath = format.Remove(0, 2).Split('\t')[2];
                libraryPath = libraryPath.Remove(0, 1);
                libraryPath = libraryPath.Remove(libraryPath.Length - 1);
                string rpgxpPath = libraryPath + "\\steamapps\\common\\RPGXP";
                if (Directory.Exists(rpgxpPath))
                {
                    return rpgxpPath;
                }
            }
            return null;
        }

        static string GetSteamInstallPathOrNull()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                RegistryKey reg64 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam", true);
                if (reg64 == null)
                {
                    return null;
                }
                return (string)reg64.GetValue("InstallPath");
            }
            else
            {
                RegistryKey reg32 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam", true);
                if (reg32 == null)
                {
                    return null;
                }
                return (string)reg32.GetValue("InstallPath");
            }
        }
    }
}
