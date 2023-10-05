﻿using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace StudioPatcher
{
    class Program 
        {
        [DllImport("msvcrt")] static extern int _getch();
        [DllImport("crtdll.dll")] public static extern int _kbhit();
        
        static string resourcesPath = Path.Combine(Environment.CurrentDirectory, "Resources");
        static string iconPath = Path.Combine(Environment.CurrentDirectory, "icon.ico");

        static void ClearDirectory(string path, bool log) {
            if (log) Console.WriteLine("Running clear on \"" + path + "\"...");
            
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);

            foreach (System.IO.FileInfo file in directory.GetFiles()) { 
                if (log == true) {
                    Console.WriteLine("Deleting file \"" + file.FullName + "\"");
                }

                file.Delete(); 
            }

            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) {
                if (log) Console.WriteLine("Deleting directory \"" + subDirectory.FullName + "\"");

                subDirectory.Delete(true);
            }

            if (log) Console.WriteLine("Finished clear on \"" + path + "\"...");
        }

        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(@"
░██████╗████████╗██╗░░░██╗██████╗░██╗░█████╗░  ██████╗░░█████╗░████████╗░█████╗░██╗░░██╗███████╗██████╗░
██╔════╝╚══██╔══╝██║░░░██║██╔══██╗██║██╔══██╗  ██╔══██╗██╔══██╗╚══██╔══╝██╔══██╗██║░░██║██╔════╝██╔══██╗
╚█████╗░░░░██║░░░██║░░░██║██║░░██║██║██║░░██║  ██████╔╝███████║░░░██║░░░██║░░╚═╝███████║█████╗░░██████╔╝
░╚═══██╗░░░██║░░░██║░░░██║██║░░██║██║██║░░██║  ██╔═══╝░██╔══██║░░░██║░░░██║░░██╗██╔══██║██╔══╝░░██╔══██╗
██████╔╝░░░██║░░░╚██████╔╝██████╔╝██║╚█████╔╝  ██║░░░░░██║░░██║░░░██║░░░╚█████╔╝██║░░██║███████╗██║░░██║
╚═════╝░░░░╚═╝░░░░╚═════╝░╚═════╝░╚═╝░╚════╝░  ╚═╝░░░░░╚═╝░░╚═╝░░░╚═╝░░░░╚════╝░╚═╝░░╚═╝╚══════╝╚═╝░░╚═╝
                      _________________________________________________________
                     //                 Fancied up by Ossyence ;)             \\
                    //    To change the studio themes change the json in the   \\
                   //                      resources folder!                    \\
                   ---------------------------------------------------------------
");

                if (!Directory.Exists(resourcesPath)) {
                    Console.WriteLine(@"DOWNLOADING RESOURCES AS THEY DONT EXIST...
                    ");

                    GetLatestResourceFiles(false);
                }

                Console.WriteLine(@"

                ");

                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Patch Studio");
                Console.WriteLine("2. Get Latest Resource Files");
                Console.WriteLine("3. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("");
                        Console.WriteLine("Do you want to create a shortcut on desktop? (Y/N)");

                        string shortcutChoice = Console.ReadLine();
                        
                        Console.WriteLine("");

                        PatchStudio(shortcutChoice);

                        break;

                    case "2":
                        GetLatestResourceFiles(true);
                        break;

                    case "3":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please choose 1, 2 or 3.");
                        Thread.Sleep(2000);
                        break;
                }
            }
        }

        static void PatchStudio(string shortcutChoice)
        {
            Console.WriteLine("Please select RobloxStudioBeta.exe.");

            // Gets the path with RobloxStudioBeta.exe in it
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string versionsFolderPath = Path.Combine(appDataFolderPath, "Roblox", "Versions");
            var versionsDir = Directory.GetDirectories(versionsFolderPath);
            string targetDir = versionsDir.FirstOrDefault(dir => File.Exists(Path.Combine(dir, "RobloxStudioBeta.exe")));

            // Prompt the user to select a file from the target directory
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = targetDir;
            dialog.Filter = "Roblox Studio|RobloxStudioBeta.exe";
            dialog.Title = "Select RobloxStudioBeta.exe";
            dialog.ShowDialog();

            // Load the .exe into a list
            var list = new List<string> { dialog.FileName };
            Console.WriteLine("Loaded .exe, Press Enter to continue!");

            foreach (string item in list)
            {
                string fileName = Path.GetFileName(item);

                if (fileName == "RobloxStudioBeta.exe")
                {
                    Console.WriteLine("Loaded RobloxStudioBeta");
                    Thread.Sleep(1000);
                    Console.WriteLine("Looking for bytes..");
                    Thread.Sleep(1000);
                    byte[] byt = File.ReadAllBytes(item);
                    for (int i = 0; i <= byt.Length - 1; i++)
                    {
                        if (byt[i] == 0x3A)
                        {
                            if (byt[i + 1] == 0x2F)
                            {
                                if (byt[i + 2] == 0x50)
                                {
                                    if (byt[i + 3] == 0x6C)
                                    {
                                        byt[i] = 0x2E;
                                        byt[i + 1] = 0x2F;
                                        byt[i + 2] = 0x50;
                                        byt[i + 3] = 0x6C;
                                        Console.WriteLine("Found byte & Patching " + byt[i]);
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine("Found Bytes and Patched RobloxStudioBeta");
                    Console.WriteLine("Creating Folders");

                    // Get the original path 
                    string originalFilePath = Path.GetFullPath(item);
                    string originalDirectory = Path.GetDirectoryName(originalFilePath);

                    // Define the folder structure
                    string folderPath = Path.Combine(originalDirectory, "Platform", "Base", "QtUI", "themes");

                    if (Directory.Exists(folderPath)) {
                        ClearDirectory(folderPath, true);
                    }

                    // Create all the directories in the specified path
                    Directory.CreateDirectory(folderPath);

                    string darkTheme = Path.Combine(folderPath, "DarkTheme.json");
                    string lightTheme = Path.Combine(folderPath, "LightTheme.json");

                    File.WriteAllText(darkTheme, System.IO.File.ReadAllText(resourcesPath + @"\DarkTheme.json"));
                    File.WriteAllText(lightTheme, System.IO.File.ReadAllText(resourcesPath + @"\LightTheme.json"));

                    // Save the patched file to the original path
                    string patchedFilePath = Path.Combine(originalDirectory, "RobloxStudioPatched.exe");
                    File.WriteAllBytes(patchedFilePath, byt);
                    Console.WriteLine("File has been saved to " + originalDirectory);

                    string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Roblox Studio Patched.lnk");
                    
                    if (File.Exists(shortcutPath)) {
                        File.Delete(shortcutPath);
                    }

                    if (shortcutChoice == "y" || shortcutChoice == "Y") {
                        Console.WriteLine("Creating shortcut on desktop...");

                        Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
                        dynamic shell = Activator.CreateInstance(t);
                        try {
                            var lnk = shell.CreateShortcut(shortcutPath);
                            try {
                                lnk.TargetPath = patchedFilePath;
                                lnk.IconLocation = iconPath;
                                lnk.Save();
                            }
                            finally {
                                Marshal.FinalReleaseComObject(lnk);
                            }
                        }
                        finally {
                            Marshal.FinalReleaseComObject(shell);
                        }

                        Console.WriteLine("Shortcut created.");
                    } else {
                        System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + patchedFilePath + "\"");

                        Console.WriteLine("Opening explorer to path...");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect App Specified");
                }
            }
            Console.WriteLine(@"
All operations complete! Press any key to end...");
            Console.Read();
        }

        static void GetLatestResourceFiles(bool returnAfter)
        {
            if (!Directory.Exists(resourcesPath)) {
                Directory.CreateDirectory(resourcesPath);
            }

            ClearDirectory(resourcesPath, true);
            
            Console.WriteLine("");

            using (WebClient client = new WebClient())
            {
                Console.WriteLine("Downloading resources...");

                string darkThemeURL = "https://raw.githubusercontent.com/MaximumADHD/Roblox-Client-Tracker/roblox/QtResources/Platform/Base/QtUI/themes/DarkTheme.json";
                string lightThemeURL = "https://raw.githubusercontent.com/MaximumADHD/Roblox-Client-Tracker/roblox/QtResources/Platform/Base/QtUI/themes/LightTheme.json";

                string darkThemeFilePath = Path.Combine(resourcesPath, "DarkTheme.json");
                string lightThemeFilePath = Path.Combine(resourcesPath, "LightTheme.json");

                client.DownloadFile(new Uri(darkThemeURL), darkThemeFilePath);
                client.DownloadFile(new Uri(lightThemeURL), lightThemeFilePath);
            }

            Console.WriteLine("Latest resource files downloaded successfully!");

            if (returnAfter) {
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
            }
        }
    }
}
