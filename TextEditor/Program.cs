using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TextEditor
{
    class Program
    {
        static long LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();//Last User Key Input Timestamp
        static bool StandbyOn = false;//TestComment
        static string WindowName = "Strawberry!";
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.Title = WindowName;

            string UserName = "User";
            bool CodeFeatures = true;
            bool AutoDisableCodeFeaturesForTXT = true;

            string State = "Menu";
            string CurrentFileName = "";
            string CurrentFilePath = "";
            List<string> Lines = [];
            List<int> UpdatedLines = [];
            List<int> SavedLines = [];
            int CurrentLine = 0;
            int CurrentChar = 0;
            int TempCurrentChar = 0;
            char LastChar = ' ';
            bool Copy = false;
            (int, int) StartOfCopy = (0, 0);
            (int, int) EndOfCopy = (0, 0);
            List<string> CopyContent = [];
            bool CompletionDisplay = true;
            int CompletionAmount = 0;
            int SelectedCompletion = 0;
            List<string> MatchingKeywords = [];
            string CompletionUserInput = "";
            Console.CursorVisible = false;

            Thread StandbyThread = new(new ThreadStart(Standby));
            LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (true)
            {
                if (State == "Menu")
                {
                    int CurrentMenuOption = 0;
                    Console.Clear();
                    for (int i = 0; i < Console.WindowHeight; i++)
                    {
                        Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                    }
                    Console.SetCursorPosition(0,0);
                    Console.WriteLine("\n\u001b[2;4H\u001b[48;2;2;82;2m\u001b[38;2;8;166;8m//\u001b[48;2;227;9;42m\u001b[38;2;156;6;29m .. ...\u001b[48;2;60;5;15m\u001b[38;2;215;120;140m    █████  █████  ████    ███   █   █   █  ████   █████  ████   ████   █   █  █");
                    Console.WriteLine("\u001b[3;2H\u001b[48;2;2;82;2m\u001b[38;2;8;166;8m///\u001b[48;2;227;9;42m\u001b[38;2;156;6;29m  ... . .\u001b[48;2;60;5;15m\u001b[38;2;215;120;140m   █        █    █   █  █   █  █   █   █  █   █  █      █   █  █   █  █   █  █");
                    Console.WriteLine("\u001b[4;4H\u001b[48;2;2;82;2m\u001b[38;2;8;166;8m//\u001b[48;2;227;9;42m\u001b[38;2;156;6;29m  . ...  \u001b[48;2;60;5;15m\u001b[38;2;215;120;140m  █████    █    ████   █████   █ █ █ █   █████  █████  ████   ████   █████  █");
                    Console.WriteLine("\u001b[5;7H\u001b[48;2;227;9;42m\u001b[38;2;156;6;29m. ... . \u001b[48;2;60;5;15m\u001b[38;2;215;120;140m      █    █    █ █    █   █   █ █ █ █   █   █  █      █ █    █ █        █");
                    Console.WriteLine("\u001b[6;8H\u001b[48;2;227;9;42m\u001b[38;2;156;6;29m . . . \u001b[48;2;60;5;15m\u001b[38;2;215;120;140m  █████    █    █  █   █   █    █   █    ████   █████  █  █   █  █   █████  █\n");
                    Console.WriteLine($"\n  It's nice to see you back, {UserName}!\n\u001b[m");
                    Console.WriteLine($"\u001b[48;2;60;5;15m    {(CurrentMenuOption==0? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Open a new file{(CurrentMenuOption == 0 ? " <<" : "")}      \n\n    {(CurrentMenuOption == 1 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Open an existing file{(CurrentMenuOption == 1 ? " <<" : "")}        \n\n    {(CurrentMenuOption == 2 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Settings{(CurrentMenuOption == 2 ? " <<" : "")}       \n\n    {(CurrentMenuOption == 3 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Posts/Updates{(CurrentMenuOption == 3 ? " <<" : "")}       \n\n\u001b[38;2;50;50;50m  Use arrow keys to change selection ▼ ▲\u001b[m");
                    ConsoleKeyInfo UserKeyInput = Console.ReadKey(true);
                    while (UserKeyInput.Key != ConsoleKey.Enter)
                    {
                        if (UserKeyInput.Key == ConsoleKey.DownArrow && CurrentMenuOption < 3)
                        {
                            CurrentMenuOption++;
                        }
                        if (UserKeyInput.Key == ConsoleKey.UpArrow && CurrentMenuOption > 0)
                        {
                            CurrentMenuOption--;
                        }
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine($"\u001b[48;2;60;5;15m    {(CurrentMenuOption == 0 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Open a new file{(CurrentMenuOption == 0 ? " <<" : "")}      \n\n    {(CurrentMenuOption == 1 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Open an existing file{(CurrentMenuOption == 1 ? " <<" : "")}        \n\n    {(CurrentMenuOption == 2 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Settings{(CurrentMenuOption == 2 ? " <<" : "")}       \n\n    {(CurrentMenuOption == 3 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Posts/Updates{(CurrentMenuOption == 3 ? " <<" : "")}       \n\n\u001b[38;2;50;50;50m  Use arrow keys to change selection ▼ ▲\u001b[m");

                        UserKeyInput = Console.ReadKey(true);
                    }

                    if (CurrentMenuOption == 0)
                    {
                        Console.CursorVisible = true;
                        Console.Title = "Strawberry! | New File";
                        OpenNewFileSetup(ref CurrentFileName, ref State, ref Lines, ref CurrentFilePath, ref UpdatedLines, ref SavedLines, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                        Console.Title = "Strawberry! | Editor | " + CurrentFileName;
                        WindowName = "Strawberry! | Editor | " + CurrentFileName;
                        LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
                        StandbyThread.Start();
                    }
                    else if (CurrentMenuOption == 1)
                    {
                        Console.CursorVisible = true;
                        Console.Title = "Strawberry! | Open File";
                        OpenExistingFileSetup(ref CurrentFileName, ref State, ref Lines, ref CurrentFilePath, ref CurrentLine, ref CurrentChar, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                        Console.Title = "Strawberry! | Editor | " + CurrentFileName;
                        WindowName = "Strawberry! | Editor | " + CurrentFileName;
                        LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
                        StandbyThread.Start();
                    }
                    else if (CurrentMenuOption == 2)
                    {
                        Console.Title = "Strawberry! | Settings";
                        Settings(ref State, ref UserName, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                        Console.Title = "Strawberry!";
                    }
                    else if (CurrentMenuOption == 3)
                    {
                        Console.Title = "Strawberry! | Posts";
                    }
                }
                if (State == "Edit")
                {
                    EditState(ref Lines, ref CurrentLine, ref CurrentChar, ref State, CurrentFileName, ref UpdatedLines, ref SavedLines, ref TempCurrentChar, ref LastChar, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                }
                if (State == "Move/Menu/Save")
                {
                    MoveState(ref Lines, ref CurrentLine, ref CurrentChar, ref State, CurrentFileName, CurrentFilePath, ref UpdatedLines, ref SavedLines, ref TempCurrentChar, ref LastChar, ref StartOfCopy, ref EndOfCopy, ref Copy, ref CopyContent, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                }
            }
        }

        static void OpenNewFileSetup(ref string CurrentFileName, ref string State,ref List<string> Lines, ref string CurrentFilePath, ref List<int> UpdatedLines, ref List<int> SavedLines, ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            Console.Clear();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
            }
            Console.SetCursorPosition(2, 1);
            Console.Write("\u001b[48;2;60;5;15mEnter file name (Including file extension):\n\n  >>");
            CurrentFileName = Console.ReadLine();
            while (CurrentFileName == "" || Regex.IsMatch(CurrentFileName, @"[\\/:*?<>|]") || CurrentFileName.Contains("\""))
            {
                Console.SetCursorPosition(2, 1);
                Console.Write($"Invalid name (Name can't contain any of the following characters: \\ / : * ? \" < > |),\n  Enter new file name (Including file extension):\n{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\u001b[4;5H\n  >>{String.Concat(Enumerable.Repeat(" ",Console.WindowWidth - 4))}\u001b[5;5H");
                CurrentFileName = Console.ReadLine();
            }
            Console.Clear();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
            }
            Console.SetCursorPosition(2, 1);
            Console.Write("\u001b[48;2;60;5;15mEnter file path or leave blank:\n\n  >>");
            CurrentFilePath = Console.ReadLine();
            while (Regex.IsMatch(CurrentFilePath, @"[/*?<>|]")) 
            {
                Console.SetCursorPosition(2, 1);
                Console.Write($"\u001b[48;2;60;5;15mInvalid path (file path can't contain any of the following characters:  / * ? \" < > |),\n  Enter new file path:\n{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\n  >>{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\u001b[5;5H");
                CurrentFilePath = Console.ReadLine();
            }
            while (File.Exists(CurrentFilePath + CurrentFileName))
            {
                Console.Clear();
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                }
                Console.SetCursorPosition(2, 1);
                Console.Write("\u001b[48;2;60;5;15mThis file already exists. Enter new name and path.\n  Enter new file name:\n        \n  >>");
                CurrentFileName = Console.ReadLine();
                while (CurrentFileName == "" || Regex.IsMatch(CurrentFileName, @"[\\/:*?<>|]") || CurrentFileName.Contains("\""))
                {
                    Console.SetCursorPosition(2, 1);
                    Console.Write("Invalid name (Name can't contain any of the following characters: \\ / : * ? \" < > |),\n  Enter new file name (Including file extension):\n        \n  >>");
                    CurrentFileName = Console.ReadLine();
                }
                Console.Clear();
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                }
                Console.SetCursorPosition(2, 1);
                Console.Write("\u001b[48;2;60;5;15mEnter file path or leave blank:\n\n  >>");
                CurrentFilePath = Console.ReadLine();
                while (Regex.IsMatch(CurrentFilePath, @"[/*?<>|]"))
                {
                    Console.SetCursorPosition(2, 1);
                    Console.Write("\u001b[48;2;60;5;15mInvalid path (file path can't contain any of the following characters:  / * ? \" < > |),\n  Enter new file path:\n        \n  >>");
                    CurrentFilePath = Console.ReadLine();
                }
            }
            if (AutoDisableCodeFeaturesForTXT && CurrentFileName.Contains("."))
            {
                if (CurrentFileName[CurrentFileName.LastIndexOf(".")..] == ".txt")
                {
                    CodeFeatures = false;
                }
            }
            Console.Write("\u001b[m");
            State = "Edit";
            Console.Clear();
            Lines.Add("");
            UpdatedLines.Add(0);
            SavedLines.Add(0);
            Console.WriteLine("|Edit|  |Current file: " + CurrentFileName + "|       |Ln : 1  Ch: 1|" + "  |" + (Console.WindowWidth - 9) + "(" + Console.WindowWidth + ")" + "ch x " + (Console.WindowHeight - 3) + "(" + Console.WindowHeight + ")" + "ch" + "|" + "\n");
            for (int i = 0; i < Lines.Count; i++)
            {
                Console.Write("1     |");
            }
            Console.SetCursorPosition(7, 2);
        }

        static void OpenExistingFileSetup(ref string CurrentFileName, ref string State, ref List<string> Lines, ref string CurrentFilePath, ref int CurrentLine, ref int CurrentChar, ref List<int> UpdatedLines, ref List<int> SavedLines, ref bool Copy, ref (int, int) StartOfCopy, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref List<string> MatchingKeywords, ref string CompletionUserInput,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            Console.Clear();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
            }
            Console.SetCursorPosition(2, 1);
            Console.Write("\u001b[48;2;60;5;15mEnter file name (Including file extension):\n\n  >>");
            CurrentFileName = Console.ReadLine();
            while (CurrentFileName == "" || Regex.IsMatch(CurrentFileName, @"[\\/:*?<>|]") || CurrentFileName.Contains("\""))
            {
                Console.SetCursorPosition(2, 1);
                Console.Write($"Invalid name (Name can't contain any of the following characters: \\ / : * ? \" < > |),\n  Enter new file name (Including file extension):\n{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\u001b[4;5H\n  >>{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\u001b[5;5H");
                CurrentFileName = Console.ReadLine();
            }
            Console.Clear();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
            }
            Console.SetCursorPosition(2, 1);
            Console.Write("\u001b[48;2;60;5;15mEnter file path or leave blank:\n\n  >>");
            CurrentFilePath = Console.ReadLine();
            while (Regex.IsMatch(CurrentFilePath, @"[/*?<>|]"))
            {
                Console.SetCursorPosition(2, 1);
                Console.Write($"\u001b[48;2;60;5;15mInvalid path (file path can't contain any of the following characters:  / * ? \" < > |),\n  Enter new file path:\n{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\n  >>{String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 4))}\u001b[5;5H");
                CurrentFilePath = Console.ReadLine();
            }
            while (!File.Exists(CurrentFilePath + CurrentFileName))
            {
                Console.Clear();
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                }
                Console.SetCursorPosition(2, 1);
                Console.Write("\u001b[48;2;60;5;15mThis file doesn't exist. Enter new name and path.\n  Enter new file name:\n        \n  >>");
                CurrentFileName = Console.ReadLine();
                while (CurrentFileName == "" || Regex.IsMatch(CurrentFileName, @"[\\/:*?<>|]") || CurrentFileName.Contains("\""))
                {
                    Console.SetCursorPosition(2, 1);
                    Console.Write("Invalid name (Name can't contain any of the following characters: \\ / : * ? \" < > |),\n  Enter new file name (Including file extension):\n        \n  >>");
                    CurrentFileName = Console.ReadLine();
                }
                Console.Clear();
                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                }
                Console.SetCursorPosition(2, 1);
                Console.Write("\u001b[48;2;60;5;15mEnter file path or leave blank:\n\n  >>");
                CurrentFilePath = Console.ReadLine();
                while (Regex.IsMatch(CurrentFilePath, @"[/*?<>|]"))
                {
                    Console.SetCursorPosition(2, 1);
                    Console.Write("\u001b[48;2;60;5;15mInvalid path (file path can't contain any of the following characters:  / * ? \" < > |),\n  Enter new file path:\n        \n  >>");
                    CurrentFilePath = Console.ReadLine();
                }
            }
            if (AutoDisableCodeFeaturesForTXT && CurrentFileName.Contains("."))
            {
                if (CurrentFileName[CurrentFileName.LastIndexOf(".")..] == ".txt")
                {
                    CodeFeatures = false;
                }
            }
            Console.Write("\u001b[m");
            State = "Edit";
            Console.Clear();
            using (StreamReader InputFile = new(CurrentFilePath + CurrentFileName))
            {
                string Line;
                while ((Line = InputFile.ReadLine()) != null)
                {
                    Lines.Add(Line);
                    UpdatedLines.Add(0);
                    SavedLines.Add(0);
                }
            }
            int TempCursorTop = Console.CursorTop;
            UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("|Edit|            |[1] Save " + CurrentFileName + "|  |Ln : 1  Ch: 1" + "|" + "  |" + (Console.WindowWidth - 9) + "(" + Console.WindowWidth + ")" + "ch x " + (Console.WindowHeight - 3) + "(" + Console.WindowHeight + ")" + "ch" + "|" + "\n");
            Console.SetCursorPosition(7,2);
        }

        static void Settings(ref string State, ref string UserName,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            State = "Settings";
            int CurrentMenuOption = 1;
            int CurrentSettingOption = 0;
            Console.Clear();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
            }
            Console.SetCursorPosition(2, 1);
            Console.Write("\u001b[48;2;60;5;15m\u001b[38;2;50;50;50m[ESC] to return to menu | Use arrow keys to change selected tab/setting ◄ ► ▼ ▲ | [ENTER] to change a setting\u001b[m");
            Console.SetCursorPosition(2,3);
            Console.WriteLine($"\u001b[48;2;60;5;15m{(CurrentMenuOption == 0 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Current Project{(CurrentMenuOption == 0 ? " <<" : "")}  \u001b[38;2;255;255;255m|  {(CurrentMenuOption == 1 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}General{(CurrentMenuOption == 1 ? " <<" : "")}  \u001b[38;2;255;255;255m|  {(CurrentMenuOption == 2 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Behaviour{(CurrentMenuOption == 2 ? " <<" : "")}  \u001b[38;2;255;255;255m|  {(CurrentMenuOption == 3 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Customisation{(CurrentMenuOption == 3 ? " <<" : "")}\u001b[m");
            Console.SetCursorPosition(0, 5);
            Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat("─", Console.WindowWidth)) + "\u001b[m");
            Console.SetCursorPosition(2, 7);
            Console.Write("\u001b[48;2;60;5;15m\u001b[38;2;50;50;50mNothing here yet...\u001b[m");
            ConsoleKeyInfo UserKeyInput = Console.ReadKey(true);
            while (UserKeyInput.Key != ConsoleKey.Escape)
            {
                if (UserKeyInput.Key == ConsoleKey.RightArrow && CurrentMenuOption < 3)
                {
                    CurrentMenuOption++;
                    CurrentSettingOption = 0;
                }
                if (UserKeyInput.Key == ConsoleKey.LeftArrow && CurrentMenuOption > 0)
                {
                    CurrentMenuOption--;
                    CurrentSettingOption = 0;
                }
                Console.SetCursorPosition(2, 3);
                Console.WriteLine($"\u001b[48;2;60;5;15m{(CurrentMenuOption == 0 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Current Project{(CurrentMenuOption == 0 ? " <<" : "")}  \u001b[38;2;255;255;255m|  {(CurrentMenuOption == 1 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}General{(CurrentMenuOption == 1 ? " <<" : "")}  \u001b[38;2;255;255;255m|  {(CurrentMenuOption == 2 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Behaviour{(CurrentMenuOption == 2 ? " <<" : "")}  \u001b[38;2;255;255;255m|  {(CurrentMenuOption == 3 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Customisation{(CurrentMenuOption == 3 ? " <<" : "")}\u001b[m");
                if (CurrentMenuOption == 0)
                {
                    if (UserKeyInput.Key == ConsoleKey.DownArrow && CurrentSettingOption < 0)
                    {
                        CurrentSettingOption++;
                    }
                    if (UserKeyInput.Key == ConsoleKey.UpArrow && CurrentSettingOption > 0)
                    {
                        CurrentSettingOption--;
                    }
                    Console.SetCursorPosition(0,6);
                    for (int i = 6; i < Console.WindowHeight; i++)
                    {
                        Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                    }
                    Console.SetCursorPosition(2, 7);
                    Console.Write("\u001b[48;2;60;5;15m\u001b[38;2;50;50;50mOpen a project to see these settings\u001b[m");
                }
                if (CurrentMenuOption == 1)
                {
                    if (UserKeyInput.Key == ConsoleKey.DownArrow && CurrentSettingOption < 0)
                    {
                        CurrentSettingOption++;
                    }
                    if (UserKeyInput.Key == ConsoleKey.UpArrow && CurrentSettingOption > 0)
                    {
                        CurrentSettingOption--;
                    }
                    Console.SetCursorPosition(0, 6);
                    for (int i = 6; i < Console.WindowHeight; i++)
                    {
                        Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                    }
                    Console.SetCursorPosition(2, 7);
                    Console.Write("\u001b[48;2;60;5;15m\u001b[38;2;50;50;50mNothing here yet...\u001b[m");
                }
                else if (CurrentMenuOption == 2)
                {
                    if (UserKeyInput.Key == ConsoleKey.Enter)
                    {
                        if (CurrentSettingOption == 0)
                        {
                            CodeFeatures = CodeFeatures == true ? false : true;
                        }
                        if (CurrentSettingOption == 1)
                        {
                            AutoDisableCodeFeaturesForTXT = AutoDisableCodeFeaturesForTXT == true ? false : true;
                        }
                    }
                    if (UserKeyInput.Key == ConsoleKey.DownArrow && CurrentSettingOption < 1)
                    {
                        CurrentSettingOption++;
                    }
                    if (UserKeyInput.Key == ConsoleKey.UpArrow && CurrentSettingOption > 0)
                    {
                        CurrentSettingOption--;
                    }
                    Console.SetCursorPosition(0, 6);
                    for (int i = 6; i < Console.WindowHeight; i++)
                    {
                        Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                    }
                    Console.SetCursorPosition(2, 7);
                    Console.WriteLine($"\u001b[48;2;60;5;15m{(CurrentSettingOption == 0 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}Coding features (e.g. Syntax Highlighting): {(CodeFeatures == true ? "On" : "Off")}{(CurrentSettingOption == 0 ? " <<" : "")}\n\n    {(CurrentSettingOption == 1 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}└─> Auto disable coding features for .txt files: {(AutoDisableCodeFeaturesForTXT == true?"On":"Off")}{(CurrentSettingOption == 1 ? " <<" : "")}\u001b[m");
                }
                else if (CurrentMenuOption == 3)
                {
                    if (UserKeyInput.Key == ConsoleKey.Enter)
                    {
                        if (CurrentSettingOption == 0)
                        {
                            Console.CursorVisible = true;
                            Console.SetCursorPosition(21 + UserName.Length, 7);
                            Console.Write("\u001b[m");
                            Console.Write(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - (21 + UserName.Length))));
                            Console.SetCursorPosition(25 + UserName.Length, 7);
                            Console.Write("\u001b[38;2;50;50;50m30 characters max | Leave empty to dismiss\u001b[m");
                            Console.SetCursorPosition(21 + UserName.Length, 7);
                            Console.Write(">>");
                            string TempUserName = Console.ReadLine();
                            while (TempUserName.Length > 30)
                            {
                                Console.SetCursorPosition(21 + UserName.Length, 7);
                                Console.Write("\u001b[m");
                                Console.Write(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - (21 + UserName.Length))));
                                Console.SetCursorPosition(25 + UserName.Length, 7);
                                Console.Write("\u001b[38;2;250;50;50m30 characters max \u001b[38;2;50;50;50m| Leave empty to dismiss\u001b[m");
                                Console.SetCursorPosition(21 + UserName.Length, 7);
                                Console.Write(">>");
                                TempUserName = Console.ReadLine();
                            }
                            UserName = TempUserName == "" ? UserName : TempUserName;
                            Console.CursorVisible = false;
                        }
                    }
                    if (UserKeyInput.Key == ConsoleKey.DownArrow && CurrentSettingOption < 0)
                    {
                        CurrentSettingOption++;
                    }
                    if (UserKeyInput.Key == ConsoleKey.UpArrow && CurrentSettingOption > 0)
                    {
                        CurrentSettingOption--;
                    }
                    Console.SetCursorPosition(0, 6);
                    for (int i = 6; i < Console.WindowHeight; i++)
                    {
                        Console.Write("\u001b[48;2;60;5;15m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                    }
                    Console.SetCursorPosition(2, 7);
                    Console.WriteLine($"\u001b[48;2;60;5;15m{(CurrentSettingOption == 0 ? "\u001b[38;2;255;255;255m>> " : "\u001b[38;2;100;100;100m")}User name: {UserName}{(CurrentSettingOption == 0 ? " <<" : "")}\u001b[m");
                }
                UserKeyInput = Console.ReadKey(true);
            }
            State = "Menu";
        }

        static void EditState(ref List<string> Lines, ref int CurrentLine, ref int CurrentChar, ref string State, string CurrentFileName, ref List<int> UpdatedLines, ref List<int> SavedLines, ref int TempCurrentChar, ref char LastChar, ref bool Copy, ref (int, int) StartOfCopy, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref List<string> MatchingKeywords, ref string CompletionUserInput,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            ConsoleKeyInfo EditStateUserKeyInput = Console.ReadKey(true);
            LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (StandbyOn)
            {
                Thread.Sleep(200);
                int TempCursorTop = Console.CursorTop;
                UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                UpdateStateAndLnAndCh(ref State, ref CurrentFileName, ref CurrentLine, ref CurrentChar, ref Lines);
                EditStateUserKeyInput = Console.ReadKey(true);
                LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
            }
            if (EditStateUserKeyInput.Key == ConsoleKey.Escape)
            {
                State = "Move/Menu/Save";
                (int, int) TempCursorPosition = Console.GetCursorPosition();
                Console.SetCursorPosition(0, 0);
                Console.Write("|Move/Menu/Save|  |[1] Save " + CurrentFileName + "|  |Ln : " + (CurrentLine + 1) + "  Ch: " + (CurrentChar + 1) + "|\x1b[?12l");
                Console.SetCursorPosition(TempCursorPosition.Item1,TempCursorPosition.Item2);
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.Backspace)
            {
                EditStateBackspace(ref Lines, ref CurrentLine, ref CurrentChar, ref State, ref CurrentFileName, ref UpdatedLines, ref SavedLines, ref LastChar, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.Enter)
            {
                EditStateEnter(ref Lines, ref CurrentLine, ref CurrentChar, ref State, ref CurrentFileName, ref UpdatedLines, ref SavedLines, ref LastChar, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            }
            else
            {
                EditStateType(ref Lines, ref CurrentLine, ref CurrentChar, ref EditStateUserKeyInput, ref State, ref CurrentFileName, ref UpdatedLines, ref SavedLines, ref TempCurrentChar, ref LastChar, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            }
        }

        static void MoveState(ref List<string> Lines, ref int CurrentLine, ref int CurrentChar, ref string State, string CurrentFileName, string CurrentFilePath, ref List<int> UpdatedLines, ref List<int> SavedLines, ref int TempCurrentChar, ref char LastChar, ref (int, int) StartOfCopy, ref (int, int) EndOfCopy, ref bool Copy, ref List<string> CopyContent, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref List<string> MatchingKeywords, ref string CompletionUserInput,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            int TempCursorTop = Console.CursorTop;
            ConsoleKeyInfo MoveStateUserKeyInput = Console.ReadKey(true);
            LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (StandbyOn)
            {
                Thread.Sleep(200);
                UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                UpdateStateAndLnAndCh(ref State, ref CurrentFileName, ref CurrentLine, ref CurrentChar, ref Lines);
                MoveStateUserKeyInput = Console.ReadKey(true);
                LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
            }
            switch (MoveStateUserKeyInput.Key)
            {
                case ConsoleKey.E:
                    Copy = false;
                    State = "Edit";
                    Console.Write("\x1b[?12h");
                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    break;

                case ConsoleKey.D1:
                    for (int i = 0; i < UpdatedLines.Count; i++)
                    {
                        if (UpdatedLines[i] == 1)
                        {
                            SavedLines[i] = 1;
                        }
                    }
                    for (int i = 0; i < UpdatedLines.Count; i++)
                    {
                        UpdatedLines[i] = 0;
                    }
                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    using (StreamWriter OutputFile = new(CurrentFilePath + CurrentFileName, false))
                    {
                        for (int i = 0; i < Lines.Count; i++)
                        {
                            if (i != Lines.Count - 1)
                            {
                                OutputFile.Write(Lines[i] + "\n");
                            }
                            else
                            {
                                OutputFile.Write(Lines[i]);
                            }
                        }
                    }
                    break;

                case ConsoleKey.C://A rare fatal error happens because index was -1 i think. look into this issue and fix it
                    if (Copy)
                    {
                        for (int i = 0; i < Lines.Count; i++)
                        {
                            if (Lines[i] == "\u2009")
                            {
                                Lines[i] = "";
                            }
                        }

                        EndOfCopy = (CurrentLine, CurrentChar);

                        if (StartOfCopy.Item1 != EndOfCopy.Item1)
                        {
                            if (StartOfCopy.Item1 < EndOfCopy.Item1)
                            {
                                CopyContent.Add(ReturnSubstring(Lines[StartOfCopy.Item1], StartOfCopy.Item2 - 1, Lines[StartOfCopy.Item1].Length));
                                for (int i = 0; i < Math.Abs(StartOfCopy.Item1 - EndOfCopy.Item1) - 1; i++)
                                {
                                    CopyContent.Add(Lines[StartOfCopy.Item1 + i + 1]);
                                }
                                CopyContent.Add(ReturnSubstring(Lines[EndOfCopy.Item1], 0, EndOfCopy.Item2));
                            }
                            else
                            {
                                CopyContent.Add(ReturnSubstring(Lines[EndOfCopy.Item1], EndOfCopy.Item2 - 1, Lines[EndOfCopy.Item1].Length));
                                for (int i = Math.Abs(StartOfCopy.Item1 - EndOfCopy.Item1) - 1; i > 0; i--)
                                {
                                    CopyContent.Add(Lines[StartOfCopy.Item1 + i - 2]);
                                }
                                CopyContent.Add(ReturnSubstring(Lines[StartOfCopy.Item1], 0, StartOfCopy.Item2));
                            }
                        }
                        else
                        {
                            (int, int) StartOrEndChar = EndOfCopy.Item2 > StartOfCopy.Item2 ? StartOfCopy : EndOfCopy;
                            (int, int) StartOrEndCharInverse = EndOfCopy.Item2 > StartOfCopy.Item2 ? EndOfCopy : StartOfCopy;
                            CopyContent.Add(ReturnSubstring(Lines[StartOfCopy.Item1], StartOrEndChar.Item2 - 1, StartOrEndCharInverse.Item2));
                        }
                        Copy = false;
                        if (Lines[CurrentLine] == "")
                        {
                            CurrentChar = 0;
                        }
                        UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                        break;
                    }
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (Lines[i] == "")
                        {
                            Lines[i] = "\u2009";
                        }
                    }
                    if (CurrentChar == 0)
                    {
                        CurrentChar = 1;
                        TempCurrentChar++;
                    }
                    Copy = true;
                    CopyContent.Clear();
                    StartOfCopy = (CurrentLine, CurrentChar);
                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    break;

                case ConsoleKey.V:
                    if (CopyContent.Count != 0)
                    {
                        if (CopyContent.Count > 1)
                        {
                            string PasteSubstringBefore = ReturnSubstring(Lines[CurrentLine], 0, CurrentChar);
                            string PasteSubstringAfter = ReturnSubstring(Lines[CurrentLine], CurrentChar, Lines[CurrentLine].Length);
                            UpdatedLines[CurrentLine] = 1;
                            Lines[CurrentLine] = PasteSubstringBefore + CopyContent[0];
                            for (int i = 1; i < CopyContent.Count; i++)
                            {
                                Lines.Insert(CurrentLine + i, CopyContent[i]);
                                UpdatedLines.Insert(CurrentLine + i, 1);
                                SavedLines.Insert(CurrentLine + i, 0);
                            }
                            Lines[CurrentLine + CopyContent.Count - 1] = Lines[CurrentLine + CopyContent.Count - 1] + PasteSubstringAfter;
                            CurrentLine += CopyContent.Count - 1;
                            CurrentChar = CopyContent[CopyContent.Count - 1].Length;
                        }
                        else
                        {
                            string PasteSubstringBefore = ReturnSubstring(Lines[CurrentLine], 0, CurrentChar);
                            string PasteSubstringAfter = ReturnSubstring(Lines[CurrentLine], CurrentChar, Lines[CurrentLine].Length);
                            Lines[CurrentLine] = PasteSubstringBefore + CopyContent[0] + PasteSubstringAfter;
                            CurrentChar += CopyContent[0].Length;
                        }
                        UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    }
                    break;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if (TempCurrentChar == -1)
                    {
                        TempCurrentChar = CurrentChar;
                    }
                    if (CurrentLine != 0)
                    {
                        CurrentLine--;
                        if (Lines[CurrentLine].Length >= CurrentChar)
                        {
                            if (Lines[CurrentLine].Length >= TempCurrentChar)
                            {
                                CurrentChar = TempCurrentChar;
                            }
                            else
                            {
                                CurrentChar = Lines[CurrentLine].Length;
                            }
                        }
                        else if (Lines[CurrentLine].Length >= TempCurrentChar && TempCurrentChar != 0)
                        {
                            CurrentChar = TempCurrentChar;
                        }
                        else
                        {
                            CurrentChar = Lines[CurrentLine].Length;
                        }
                    }
                    LastChar = ' ';
                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    TempCurrentChar = -1;
                    if (CurrentChar != (Copy ? Lines[CurrentLine].Length == 0 ? 0 : 1 : 0))
                    {
                        CurrentChar--;
                    }
                    else if (CurrentLine != 0)
                    {
                        CurrentLine--;
                        CurrentChar = Lines[CurrentLine].Length;
                    }
                    LastChar = ' ';

                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (TempCurrentChar == -1)
                    {
                        TempCurrentChar = CurrentChar;
                    }
                    if (CurrentLine != Lines.Count - 1)
                    {
                        CurrentLine++;
                        if (Lines[CurrentLine].Length >= CurrentChar)
                        {
                            if (Lines[CurrentLine].Length >= TempCurrentChar)
                            {
                                CurrentChar = TempCurrentChar;
                            }
                            else
                            {
                                CurrentChar = Lines[CurrentLine].Length;
                            }
                        }
                        else if (Lines[CurrentLine].Length >= TempCurrentChar && TempCurrentChar != 0)
                        {
                            CurrentChar = TempCurrentChar;
                        }
                        else
                        {
                            CurrentChar = Lines[CurrentLine].Length;
                        }
                    }
                    LastChar = ' ';
                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if (CurrentChar != Lines[CurrentLine].Length)
                    {
                        TempCurrentChar = -1;
                        CurrentChar++;
                    }
                    else if (CurrentLine != Lines.Count - 1)
                    {
                        CurrentLine++;
                        CurrentChar = Copy ? Lines[CurrentLine].Length == 0 ? 0 : 1 : 0;
                    }
                    LastChar = ' ';

                    UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
                    break;
            }
            LastChar = ' ';
            UpdateStateAndLnAndCh(ref State, ref CurrentFileName, ref CurrentLine, ref CurrentChar, ref Lines);
        }

        static void EditStateBackspace(ref List<string> Lines, ref int CurrentLine, ref int CurrentChar, ref string State, ref string CurrentFileName, ref List<int> UpdatedLines, ref List<int> SavedLines, ref char LastChar, ref bool Copy, ref (int, int) StartOfCopy, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref List<string> MatchingKeywords, ref string CompletionUserInput,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            if (CurrentChar != 0)
            {
                int TempCursorTop = Console.CursorTop;
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar - 1) + Lines[CurrentLine].Substring(CurrentChar);
                CurrentChar--;
                UpdatedLines[CurrentLine] = 1;
                CompletionDisplay = true;
                UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            }
            else if (CurrentLine != 0)
            {
                int TempCursorTop = Console.CursorTop;
                string TempLine = Lines[CurrentLine];
                Lines.RemoveAt(CurrentLine);
                UpdatedLines.RemoveAt(CurrentLine);
                SavedLines.RemoveAt(CurrentLine);
                CurrentLine--;
                UpdatedLines[CurrentLine] = 1;
                CurrentChar = Lines[CurrentLine].Length;
                Lines[CurrentLine] += TempLine;
                CompletionDisplay = true;
                UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            }
            else
            {
                int TempCursorTop = Console.CursorTop;
                UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);
            }
            LastChar = ' ';
            UpdateStateAndLnAndCh(ref State, ref CurrentFileName, ref CurrentLine, ref CurrentChar, ref Lines);
        }

        static void EditStateEnter(ref List<string> Lines, ref int CurrentLine, ref int CurrentChar, ref string State, ref string CurrentFileName, ref List<int> UpdatedLines, ref List<int> SavedLines, ref char LastChar, ref bool Copy, ref (int, int) StartOfCopy, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref List<string> MatchingKeywords, ref string CompletionUserInput,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            int TempCursorTop = Console.CursorTop;
            if (CompletionDisplay && CompletionUserInput != "")
            {
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar - CompletionUserInput.Length) + MatchingKeywords[SelectedCompletion] + Lines[CurrentLine].Substring(CurrentChar);
                UpdatedLines[CurrentLine] = 1;
                CurrentChar += MatchingKeywords[SelectedCompletion].Length - CompletionUserInput.Length;
                LastChar = ' ';
                CompletionDisplay = false;
            }
            else if (LastChar == '{')
            {
                string RemainingStringOnCurrentLine = Lines[CurrentLine].Substring(0, CurrentChar);
                string StringToNextLine = Lines[CurrentLine].Substring(CurrentChar++);
                Lines[CurrentLine] = RemainingStringOnCurrentLine;
                Lines.Insert(CurrentLine + 1, "");
                UpdatedLines.Insert(CurrentLine + 1, 1);
                SavedLines.Insert(CurrentLine + 1, 0);
                Lines.Insert(CurrentLine + 2, "}");
                UpdatedLines.Insert(CurrentLine + 2, 1);
                SavedLines.Insert(CurrentLine + 2, 0);
                UpdatedLines[CurrentLine] = 1;
                CurrentLine++;
                CurrentLine++;
                Lines[CurrentLine] = StringToNextLine;
                CurrentLine--;
                CurrentChar = 0;
                LastChar = ' ';
                CompletionDisplay = true;
            }
            else
            {
                string RemainingStringOnCurrentLine = Lines[CurrentLine].Substring(0, CurrentChar);
                string StringToNextLine = Lines[CurrentLine].Substring(CurrentChar);
                Lines[CurrentLine] = RemainingStringOnCurrentLine;
                Lines.Insert(CurrentLine + 1, "");
                UpdatedLines.Insert(CurrentLine + 1, 0);
                SavedLines.Insert(CurrentLine + 1, 0);
                UpdatedLines[CurrentLine] = 1;
                CurrentLine++;
                UpdatedLines[CurrentLine] = 1;
                CurrentChar = 0;
                Lines[CurrentLine] = StringToNextLine;
                CompletionDisplay = true;
            }

            UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);

            UpdateStateAndLnAndCh(ref State, ref CurrentFileName, ref CurrentLine, ref CurrentChar, ref Lines);
        }

        static void EditStateType(ref List<string> Lines, ref int CurrentLine, ref int CurrentChar, ref ConsoleKeyInfo EditStateUserKeyInput, ref string State, ref string CurrentFileName, ref List<int> UpdatedLines, ref List<int> SavedLines, ref int TempCurrentChar, ref char LastChar, ref bool Copy, ref (int, int) StartOfCopy, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref List<string> MatchingKeywords, ref string CompletionUserInput,ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            int TempCursorTop = Console.CursorTop;
            if (CompletionDisplay && EditStateUserKeyInput.Key == ConsoleKey.Tab)
            {
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar - CompletionUserInput.Length) + MatchingKeywords[SelectedCompletion] + Lines[CurrentLine].Substring(CurrentChar);
                UpdatedLines[CurrentLine] = 1;
                CurrentChar += MatchingKeywords[SelectedCompletion].Length - CompletionUserInput.Length;
                LastChar = ' ';
                CompletionDisplay = false;
            }
            else if (CompletionDisplay && EditStateUserKeyInput.Key == ConsoleKey.Spacebar)
            {
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar - CompletionUserInput.Length) + MatchingKeywords[SelectedCompletion] + " " + Lines[CurrentLine].Substring(CurrentChar);
                UpdatedLines[CurrentLine] = 1;
                CurrentChar += MatchingKeywords[SelectedCompletion].Length - CompletionUserInput.Length + 1;
                LastChar = ' ';
                CompletionDisplay = false;
            }
            else if (EditStateUserKeyInput.KeyChar == '{')
            {
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar) + "{}" + Lines[CurrentLine].Substring(CurrentChar);
                CurrentChar ++;
                LastChar = '{';
                CompletionDisplay = true;
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.Tab)
            {
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar) + "    " + Lines[CurrentLine].Substring(CurrentChar);
                UpdatedLines[CurrentLine] = 1;
                CurrentChar += 4;
                LastChar = ' ';
                CompletionDisplay = true;
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.UpArrow)
            {
                if (!CompletionDisplay || !CodeFeatures)
                {
                    if (TempCurrentChar == -1)
                    {
                        TempCurrentChar = CurrentChar;
                    }
                    if (CurrentLine != 0)
                    {
                        CurrentLine--;
                        if (Lines[CurrentLine].Length >= CurrentChar)
                        {
                            if (Lines[CurrentLine].Length >= TempCurrentChar)
                            {
                                CurrentChar = TempCurrentChar;
                            }
                            else
                            {
                                CurrentChar = Lines[CurrentLine].Length;
                            }
                        }
                        else if (Lines[CurrentLine].Length >= TempCurrentChar && TempCurrentChar != 0)
                        {
                            CurrentChar = TempCurrentChar;
                        }
                        else
                        {
                            CurrentChar = Lines[CurrentLine].Length;
                        }
                    }
                    LastChar = ' ';
                }
                else
                {
                    SelectedCompletion = SelectedCompletion > 0 ? SelectedCompletion - 1 : 0;
                }
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.DownArrow)
            {
                if (!CompletionDisplay || !CodeFeatures)
                {
                    if (TempCurrentChar == -1)
                    {
                        TempCurrentChar = CurrentChar;
                    }
                    if (CurrentLine != Lines.Count - 1)
                    {
                        CurrentLine++;
                        if (Lines[CurrentLine].Length >= CurrentChar)
                        {
                            if (Lines[CurrentLine].Length >= TempCurrentChar)
                            {
                                CurrentChar = TempCurrentChar;
                            }
                            else
                            {
                                CurrentChar = Lines[CurrentLine].Length;
                            }
                        }
                        else if (Lines[CurrentLine].Length >= TempCurrentChar && TempCurrentChar != 0)
                        {
                            CurrentChar = TempCurrentChar;
                        }
                        else
                        {
                            CurrentChar = Lines[CurrentLine].Length;
                        }
                    }
                    LastChar = ' ';
                }
                else
                {
                    SelectedCompletion = SelectedCompletion < CompletionAmount ? SelectedCompletion + 1 : CompletionAmount;
                }
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.LeftArrow)
            {
                TempCurrentChar = -1;
                if (CurrentChar != 0)
                {
                    CurrentChar--;
                }
                else if (CurrentLine != 0)
                {
                    CurrentLine--;
                    CurrentChar = Lines[CurrentLine].Length;
                }
                LastChar = ' ';
            }
            else if (EditStateUserKeyInput.Key == ConsoleKey.RightArrow)
            {
                if (CurrentChar != Lines[CurrentLine].Length)
                {
                    TempCurrentChar = -1;
                    CurrentChar++;
                }
                else if (CurrentLine != Lines.Count - 1)
                {
                    CurrentLine++;
                    CurrentChar = 0;
                }
                LastChar = ' ';
            }
            else
            {
                Lines[CurrentLine] = Lines[CurrentLine].Substring(0, CurrentChar) + (EditStateUserKeyInput.KeyChar == '\"' ? "\"\"" : EditStateUserKeyInput.KeyChar == '(' ? "()" : EditStateUserKeyInput.KeyChar) + Lines[CurrentLine].Substring(CurrentChar);
                UpdatedLines[CurrentLine] = 1;
                CurrentChar++;
                LastChar = ' ';
                CompletionDisplay = true;
            }
            UpdateTextEditor(ref CurrentLine, ref CurrentChar, ref Lines, ref TempCursorTop, ref UpdatedLines, ref SavedLines, ref Copy, ref StartOfCopy, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, ref AutoDisableCodeFeaturesForTXT, ref CodeFeatures);

            UpdateStateAndLnAndCh(ref State, ref CurrentFileName, ref CurrentLine, ref CurrentChar, ref Lines);
        }

        static void UpdateTextEditor(ref int CurrentLine, ref int CurrentChar, ref List<string> Lines, ref int TempCursorTop, ref List<int> UpdatedLines, ref List<int> SavedLines, ref bool Copy, ref (int, int) StartOfCopy, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref string State, ref List<string> MatchingKeywords, ref string CompletionUserInput, ref bool AutoDisableCodeFeaturesForTXT, ref bool CodeFeatures)
        {
            List<string> SyntaxHighlightingList = [];
            if (CodeFeatures)
            {
                SyntaxHighlightingList = UpdateSyntaxHighlighting(ref Lines);
            }
            string SyntaxHighlightingString = "";
            foreach (string Line in SyntaxHighlightingList)
            {
                SyntaxHighlightingString += Line + "+";
            }
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 2);
            for (int i = CurrentLine > (Console.WindowHeight/2 - 4) ? CurrentLine - (Console.WindowHeight/2 - 4) : 0; i < (CurrentLine > Console.WindowHeight/2 - 3 ? CurrentLine + 1 + (Console.WindowHeight - Console.WindowHeight / 2) : Lines.Count < Console.WindowHeight/2 - 3 ? Lines.Count : CurrentLine == Console.WindowHeight/2 - 3 ? Console.WindowHeight - 2 : Console.WindowHeight - 3); i++)
            {
                int StartIndex = CurrentChar < Console.WindowWidth - 9 ? 0 : CurrentChar - (Console.WindowWidth - 9) > Lines[i >= Lines.Count ? Lines.Count - 1 : i].Length ? Lines[i >= Lines.Count ? Lines.Count - 1 : i].Length : CurrentChar - (Console.WindowWidth - 9);
                int EndIndex = CurrentChar < (Console.WindowWidth - 9) ? (Console.WindowWidth - 9) : CurrentChar;
                int EndIndexForLines = 0;
            
                if (i < Lines.Count)
                {
                    EndIndexForLines = CurrentChar < (Console.WindowWidth - 9) ? Lines[i].Length > Console.WindowWidth - 9 ? Console.WindowWidth - 9 : Lines[i].Length : Lines[i].Length > CurrentChar ? CurrentChar : Lines[i].Length;
                    Console.Write(i + 1 + string.Concat(Enumerable.Repeat(" ", 5 - (i + 1).ToString().Length)) + ((ReturnSubstring(Lines[i], StartIndex, EndIndex) == "") && Lines[i].Length > (Copy ? Lines[i] == "\u2009" ? 1 : 0 : 0) ? "\x1b[38;2;0;0;0m\x1b[48;2;255;255;255m<\x1b[m" : " ") + (UpdatedLines[i] == 1 ? "\u001b[38;2;230;177;34m|\u001b[m" : SavedLines[i] == 1 ? "\u001b[38;2;53;201;30m|\u001b[m" : "|"));
                    for (int ii = StartIndex; ii < EndIndexForLines; ii++)
                    {
                        if (Copy)
                        {
                            if (i == CurrentLine)
                            {
                                if (i == StartOfCopy.Item1)
                                {
                                    if (ii == StartOfCopy.Item2 - 1)
                                    {
                                        Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                    }
                                    else if (ii > CurrentChar - 2 && ii < StartOfCopy.Item2)
                                    {
                                        Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                    }
                                    else if (ii < CurrentChar && ii > StartOfCopy.Item2 - 1)
                                    {
                                        Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                    }
                                    else
                                    {
                                        Console.Write((SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[38;2;" + SyntaxHighlightingString.Substring(SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) + 1, SyntaxHighlightingString.IndexOf("+", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - 1) + "m" : "") + Lines[i][ii] + (SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[m" : ""));
                                    }
                                }
                                else if (i < StartOfCopy.Item1 && ii > CurrentChar - 2)
                                {
                                    Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                }
                                else if (i > StartOfCopy.Item1 && ii < CurrentChar)
                                {
                                    Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                }
                                else
                                {
                                    Console.Write((SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[38;2;" + SyntaxHighlightingString.Substring(SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) + 1, SyntaxHighlightingString.IndexOf("+", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - 1) + "m" : "") + Lines[i][ii] + (SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[m" : ""));
                                }
                            }
                            else if (i != StartOfCopy.Item1)
                            {
                                if (i < StartOfCopy.Item1 && i > CurrentLine)
                                {
                                    Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                }
                                else if (i > StartOfCopy.Item1 && i < CurrentLine)
                                {
                                    Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                }
                                else
                                {
                                    Console.Write((SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[38;2;" + SyntaxHighlightingString.Substring(SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) + 1, SyntaxHighlightingString.IndexOf("+", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - 1) + "m" : "") + Lines[i][ii] + (SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[m" : ""));
                                }
                            }
                            else if (i == StartOfCopy.Item1)
                            {
                                if (CurrentLine < StartOfCopy.Item1 && ii < StartOfCopy.Item2)
                                {
                                    Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                }
                                else if (CurrentLine > StartOfCopy.Item1 && ii > StartOfCopy.Item2 - 2)
                                {
                                    Console.Write("\u001b[38;2;0;0;0m\u001b[48;2;" + (Lines[i] == "\u2009" ? "50;50;50" : "255;255;255") + "m" + Lines[i][ii] + "\u001b[m");
                                }
                                else
                                {
                                    Console.Write((SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[38;2;" + SyntaxHighlightingString.Substring(SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) + 1, SyntaxHighlightingString.IndexOf("+", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - 1) + "m" : "") + Lines[i][ii] + (SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[m" : ""));
                                }
                            }
                            else
                            {
                                Console.Write((SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[38;2;" + SyntaxHighlightingString.Substring(SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) + 1, SyntaxHighlightingString.IndexOf("+", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - 1) + "m" : "") + Lines[i][ii] + (SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[m" : ""));
                            }
                        }
                        else
                        {
                            Console.Write((SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[38;2;" + SyntaxHighlightingString.Substring(SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) + 1, SyntaxHighlightingString.IndexOf("+", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - SyntaxHighlightingString.IndexOf("#", SyntaxHighlightingString.IndexOf($"@{i}:{ii}#")) - 1) + "m" : "") + Lines[i][ii] + (SyntaxHighlightingString.Contains($"@{i}:{ii}#") ? "\u001b[m" : ""));
                        }
                    }
                    Console.WriteLine(String.Concat(Enumerable.Repeat(" ", Lines[i].Length < (Console.WindowWidth - 9) ? (Console.WindowWidth - ReturnSubstring(Lines[i], StartIndex, EndIndex).Length - 9) : Console.WindowWidth - 9 - ReturnSubstring(Lines[i], StartIndex, EndIndex).Length)));
                }
                else
                {
                    Console.WriteLine(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth - 2)));
                }
            }
            for (int i = 0; i < Console.WindowHeight - 3 - Lines.Count; i++)
            {
                Console.WriteLine(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
            }
            Console.SetCursorPosition(CurrentChar + 7 < Console.WindowWidth - 2 ? CurrentChar + 7 : Console.WindowWidth - 2, CurrentLine < Console.WindowHeight/2 - 3 ? CurrentLine + 2 : TempCursorTop);
            if (State == "Edit" && CompletionDisplay && CodeFeatures)
            {
                DisplayCompletions(Lines[CurrentLine], ref CurrentLine, ref CurrentChar, ref CompletionDisplay, ref CompletionAmount, ref SelectedCompletion, ref State, ref MatchingKeywords, ref CompletionUserInput, SyntaxHighlightingString);
            }
            Console.CursorVisible = true;
        }

        static void UpdateStateAndLnAndCh(ref string State, ref string CurrentFileName, ref int CurrentLine, ref int CurrentChar, ref List<string> Lines)
        {
            if (State == "Edit")
            {
                (int, int) TempCursorPosition = Console.GetCursorPosition();
                Console.SetCursorPosition(0, 0);
                Console.Write("|Edit|  |Current file: " + CurrentFileName + "|       |Ln : " + (CurrentLine + 1) + "  Ch: " + (CurrentChar + 1) + "|" + "  |"  + (Console.WindowWidth - 9) + "(" + Console.WindowWidth + ")" +  "ch x " + (Console.WindowHeight - 3) + "(" + Console.WindowHeight + ")" + "ch" + "|" + String.Concat(Enumerable.Repeat(" ", CurrentLine.ToString().Length)) + "\n");
                Console.WriteLine(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
                Console.SetCursorPosition(TempCursorPosition.Item1, TempCursorPosition.Item2);
            }
            else if (State == "Move/Menu/Save")
            {
                (int, int) TempCursorPosition = Console.GetCursorPosition();
                Console.SetCursorPosition(0, 0);
                Console.Write("|Move/Menu/Save|  |[1] Save " + CurrentFileName + "|  |Ln : " + (CurrentLine + 1) + "  Ch: " + (CurrentChar + 1) + "|" + "  |" + (Console.WindowWidth - 9) +  "(" + Console.WindowWidth +  ")" + "ch x " + (Console.WindowHeight - 3) + "(" + Console.WindowHeight + ")" + "ch" + "|" + String.Concat(Enumerable.Repeat(" ", CurrentLine.ToString().Length)) + "\n");
                Console.WriteLine(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
                Console.SetCursorPosition(TempCursorPosition.Item1, TempCursorPosition.Item2);
            }
        }

        static void Standby()
        {
            int TimeUntilStandby = 600;
            while (true)
            {
                if (LUKIT + TimeUntilStandby < DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    StandbyOn = true;
                    (int, int) TempCursorPos = Console.GetCursorPosition();
                    string TempTitle = WindowName;
                    Console.Title = "Strawberry! | Standby";
                    Console.CursorVisible = false;
                    Console.Clear();
                    for (int i = 0; i < Console.WindowHeight; i++)
                    {
                        Console.Write("\u001b[48;2;0;0;0m" + String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)) + "\u001b[m" + (i == Console.WindowHeight - 1 ? "" : "\n"));
                    }
                    while (LUKIT + TimeUntilStandby < DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        int x = new Random().Next(0, Console.WindowWidth - 42);
                        int y = new Random().Next(0, Console.WindowHeight - 3);
                        int r = new Random().Next(50, 255);
                        int g = new Random().Next(50, 255);
                        int b = new Random().Next(50, 255);
                        while (r+g+b < 200)
                        {
                            r = new Random().Next(50, 255);
                            g = new Random().Next(50, 255);
                            b = new Random().Next(50, 255);
                        }
                        Console.SetCursorPosition(x, y);
                        for (int i = 0; i < 255; i++)
                        {
                            if (LUKIT + TimeUntilStandby > DateTimeOffset.Now.ToUnixTimeSeconds())
                            {
                                break;
                            }
                            Console.Write($"\u001b[38;2;{r * i / 255};{g * i / 255};{b * i / 255}m\u001b[48;2;0;0;0m" + "┌────────────────────────────────────────┐");
                            Console.SetCursorPosition(Console.CursorLeft - 42,Console.CursorTop + 1);
                            Console.Write($"│On standby. I hope to see you back soon!│");
                            Console.SetCursorPosition(Console.CursorLeft - 42, Console.CursorTop + 1);
                            Console.Write($"└────────────────────────────────────────┘" + "\u001b[m");
                            Thread.Sleep(20);
                            Console.SetCursorPosition(x, y);
                        }
                        if (LUKIT + TimeUntilStandby > DateTimeOffset.Now.ToUnixTimeSeconds())
                        {
                            break;
                        }
                        for (int i = 0; i < 100; i++)
                        {
                            if (LUKIT + TimeUntilStandby > DateTimeOffset.Now.ToUnixTimeSeconds())
                            {
                                break;
                            }
                            Thread.Sleep(10);
                        }
                        if (LUKIT + TimeUntilStandby > DateTimeOffset.Now.ToUnixTimeSeconds())
                        {
                            break;
                        }
                        for (int i = 255; i > 0; i--)
                        {
                            if (LUKIT + TimeUntilStandby > DateTimeOffset.Now.ToUnixTimeSeconds())
                            {
                                break;
                            }
                            Console.Write($"\u001b[38;2;{r * i / 255};{g * i / 255};{b * i / 255}m\u001b[48;2;0;0;0m" + "┌────────────────────────────────────────┐");
                            Console.SetCursorPosition(Console.CursorLeft - 42, Console.CursorTop + 1);
                            Console.Write($"│On standby. I hope to see you back soon!│");
                            Console.SetCursorPosition(Console.CursorLeft - 42, Console.CursorTop + 1);
                            Console.Write($"└────────────────────────────────────────┘" + "\u001b[m");
                            Thread.Sleep(20);
                            Console.SetCursorPosition(x, y);
                        }
                    }
                    LUKIT = DateTimeOffset.Now.ToUnixTimeSeconds();
                    Console.SetCursorPosition(TempCursorPos.Item1,TempCursorPos.Item2);
                    Console.Title = TempTitle;
                    Console.CursorVisible = true;
                    Console.Clear();
                    StandbyOn = false;
                }
            }
        }
        static List<string> UpdateSyntaxHighlighting(ref List<string> Lines)// maybe make this return the string instead of a list
        {
            List<string> UpdatedSyntaxHighlightingList = [];
            List<string> KeywordAndColor = [
                "Console!:50;200;50",
                "List!:50;200;50",
                "Enumerable!:50;200;50",
                "Thread!:50;200;50",
                "WriteLine!Console:250;200;60",
                "Write!Console:250;200;60",
                "static!:50;50;200",
                "void!:50;50;200",
                "for!:202;72;224",
                "foreach!:202;72;224",
                "while!:202;72;224",
                "return!:202;72;224",
                "int!:50;50;200",
                "string!:50;50;200",
                "bool!:50;50;200",
                "ref!:50;50;200",
                "double!:50;50;200",
                "ToString!:250;200;60",
                "Add!:250;200;60",
                "AddRange!:250;200;60",
                "Matches!Regex:250;200;60",
                "Concat!String:250;200;60",
                "Repeat!Enumerable:250;200;60",
                "SetCursorPosition!Console:250;200;60",
                "Contains!:250;200;60",
                "ReadLine!Console:250;200;60",
                "ReadKey!Console:250;200;60",
                "Clear!Console:250;200;60",
                "in!:202;72;224",
                "break!:202;72;224",
                "continue!:202;72;224",
                "if!:202;72;224",
                "char!:50;50;200",
                "ConsoleKeyInfo!:105;179;96",
                "class!:50;50;200",
                "using!:50;50;200",
                "namespace!:50;50;200",
                "long!:50;50;200",
                "ToInt32!:250;200;60",
                "Convert!:50;200;50",
                "Regex!:50;200;50"
                ];
            for(int i = 0; i < Lines.Count; i++)
            {
                List<string> matches = Regex.Matches(Lines[i], @$"(List<)?\b(int|string|bool|double|char|ConsoleKeyInfo)\b>?( *)([a-zA-Z_])(,?\w*)+").Cast<Match>().Select(s => s.Value.Substring(s.Value.LastIndexOf(" ") + 1)).ToList();
                foreach (string match in matches)
                {
                    string[] OnlyNames = match.Split(",");
                    foreach (string Name in OnlyNames)
                    {
                        if (Name != "")
                        {
                            KeywordAndColor.Add(Name + "!:37;196;217");
                        }
                    }
                }
            }
            for (int i = 0; i < Lines.Count; i++)
            {
                List<string> matches = Regex.Matches(Lines[i], @$"(List<)?\b(int|string|bool|double|void|char)\b>?( *)([a-zA-Z_])(,?\w*)+\(.*\)").Cast<Match>().Select(s => (s.Value[..s.Value.LastIndexOf("(")])[((s.Value[..s.Value.LastIndexOf("(")]).LastIndexOf(" ") + 1)..]).ToList();
                foreach (string Match in matches)
                {
                    if (KeywordAndColor.Contains(Match + "!:37;196;217"))
                    {
                        KeywordAndColor[KeywordAndColor.IndexOf(Match + "!:37;196;217")] = Match + "!:250;200;60";
                    }
                    else
                    {
                        KeywordAndColor.Add(Match + "!:250;200;60");
                    }
                }
            }
            for (int i = 0; i < Lines.Count; i++)
            {
                List<string> matches = Regex.Matches(Lines[i], @$"\b(class)\b( *)([a-zA-Z_])(\w*)").Cast<Match>().Select(s => s.Value[(s.Value.LastIndexOf(" ") + 1)..]).ToList();
                foreach (string Match in matches)
                {
                    KeywordAndColor.Add(Match + "!:50;200;50");
                }
            }
            for (int i = 0; i < Lines.Count; i++)
            {
                List<Match> ClassKeyWordsLightMatches = [];
                ClassKeyWordsLightMatches.AddRange(Regex.Matches(Lines[i], @$"//.*").ToList());
                ClassKeyWordsLightMatches.AddRange(Regex.Matches(Lines[i], @$"""(.*?)""").ToList());
                ClassKeyWordsLightMatches.AddRange(Regex.Matches(Lines[i],@$"\b(\d+)\b").ToList());
                foreach (string Keyword in KeywordAndColor)
                {
                    ClassKeyWordsLightMatches.AddRange(Regex.Matches(Lines[i], @$"{(Keyword.IndexOf(":") - Keyword.IndexOf("!") == 1 ? @$"\b" : (ReturnSubstring(Keyword, Keyword.IndexOf("!") + 1, Keyword.IndexOf(":"))) + ".")}{Keyword.Substring(0, Keyword.IndexOf('!'))}\b").ToList());
                    foreach (Match Match in ClassKeyWordsLightMatches)
                    {
                        if (Match.ToString().All(char.IsDigit))
                        {
                            for (int ii = 0; ii < Match.Value.Length; ii++)
                            {
                                UpdatedSyntaxHighlightingList.Add($"@{i}:{Match.Index + ii}#178;222;173");
                            }
                            continue;
                        }
                        if (Match.ToString()[0] == '/' && Match.ToString()[1] == '/')
                        {
                            for (int ii = 0; ii < Match.Value.Length; ii++)
                            {
                                UpdatedSyntaxHighlightingList.Add($"@{i}:{Match.Index + ii}#26;77;9");
                            }
                            continue;
                        }
                        else if (Match.ToString()[0] == '\"')
                        {
                            for (int ii = 0; ii < Match.Value.Length; ii++)
                            {
                                UpdatedSyntaxHighlightingList.Add($"@{i}:{Match.Index + ii}#245;155;59");
                            }
                            continue;
                        }
                        else
                        {
                            for(int ii = (Match.Value.Contains(".") ? Match.Value.IndexOf(".") + 1 : 0); ii < Match.Value.Length;  ii++)
                            {
                                UpdatedSyntaxHighlightingList.Add($"@{i}:{Match.Index + ii}#{Keyword.Substring(Keyword.IndexOf(':') + 1)}");
                            }
                            continue;
                        }
                    }
                    ClassKeyWordsLightMatches.Clear();
                }
            }
            return UpdatedSyntaxHighlightingList;
        }

        static void DisplayCompletions(string LineToSubstring,ref int CurrentLine, ref int CurrentChar, ref bool CompletionDisplay, ref int CompletionAmount, ref int SelectedCompletion, ref string State, ref List<string> MatchingKeywords, ref string CompletionUserInput, string SyntaxHighlightingString)
        {
            (int, int) SavedCursorPositions = Console.GetCursorPosition();
            bool NoLettersOrDigits = LineToSubstring.Length == CurrentChar ? true : !Char.IsLetter(LineToSubstring[CurrentChar]) ? !Char.IsDigit(LineToSubstring[CurrentChar]) ? true : false : false;
            bool InClass = false;
            string Line = LineToSubstring[..CurrentChar];
            Dictionary<string,string> KeywordsInKeyword = [];
            KeywordsInKeyword.Add("None", "Console,while,for,foreach,else,break,continue,if,return,string,int,bool,double,char,static,public,void,using,long,namespace,ConsoleKeyInfo,in,List,Regex,Cheese1,Cheese2,Cheese3,Cheese4,Cheese5,Cheese6");
            KeywordsInKeyword.Add("Console", "ReadLine,ReadKey,WriteLine,Write,SetCursorPosition,Clear,CursorLeft,CursorTop");
            KeywordsInKeyword.Add("Regex", "Matches,IsMatch");
            int HeightOfDisplay = 10;
            List<Match> Matches = Regex.Matches(Line, @$"[a-zA-Z_]+\.").ToList();
            MatchingKeywords.Clear();
            if (Matches.Count != 0)
            {
                InClass = true;
                if (KeywordsInKeyword.ContainsKey(Matches[Matches.Count - 1].Value[..Matches[Matches.Count - 1].Value.IndexOf(".")]))
                {
                    foreach (string Value in KeywordsInKeyword[Matches[Matches.Count - 1].Value[..Matches[Matches.Count - 1].Value.IndexOf(".")]].Split(","))
                    {
                        CompletionUserInput = Line[(Line.LastIndexOf(".") + 1)..].ToLower();
                        if (Value.ToLower().Contains(Line[(Line.LastIndexOf(".") + 1)..].ToLower()) || Line[(Line.LastIndexOf(".") + 1)..] == "")
                        {
                            MatchingKeywords.Add(Value);
                        }
                    }
                }
            }
            Matches = Regex.Matches(Line, @$"[a-zA-Z_]+").ToList();
            if (MatchingKeywords.Count == 0)
            {
                MatchingKeywords.Clear();
            }
            if (Matches.Count != 0 && NoLettersOrDigits && !InClass && Char.IsLetter(Line[CurrentChar - 1]) && !SyntaxHighlightingString.Contains($"@{CurrentLine}:{CurrentChar-1}#37;196;217") && !SyntaxHighlightingString.Contains($"@{CurrentLine}:{CurrentChar - 1}#26;77;9") && !SyntaxHighlightingString.Contains($"@{CurrentLine}:{CurrentChar - 1}#245;155;59"))
            {
                foreach (string Value in KeywordsInKeyword["None"].Split(","))
                {
                    CompletionUserInput = Matches[Matches.Count - 1].ToString().ToLower();
                    if (Value.ToLower().Contains(Matches[Matches.Count - 1].ToString().ToLower()))
                    {
                        MatchingKeywords.Add(Value);
                    }
                }
            }
            if (MatchingKeywords.Count != 0)
            {
                List<bool> SideBar = [];
                CompletionAmount = MatchingKeywords.Count - 1;
                for (int ii = 0; ii < 10; ii++)
                {
                    SideBar.Add(false);
                }
                if (SelectedCompletion == 0)
                {
                    SideBar[0] = true;
                    for (int i = 1; i < SideBar.Count; i++)
                    {
                        SideBar[i] = false;
                    }
                }
                if (SelectedCompletion == CompletionAmount)
                {
                    for (int i = 0; i < SideBar.Count; i++)
                    {
                        SideBar[i] = false;
                    }
                    SideBar[SideBar.Count - 1] = true;
                }
                if (SelectedCompletion > 0 && SelectedCompletion < CompletionAmount)
                {
                    for (int i = 0; i < SideBar.Count; i++)
                    {
                        SideBar[i] = false;
                    }
                    int num = Convert.ToInt32(Math.Floor(Convert.ToDecimal(SelectedCompletion) * (Convert.ToDecimal(HeightOfDisplay) / Convert.ToDecimal(CompletionAmount))));
                    if (CompletionAmount + 1 > HeightOfDisplay)
                    {
                        SideBar[Convert.ToInt32(Math.Floor(Convert.ToDecimal(SelectedCompletion) * (Convert.ToDecimal(HeightOfDisplay) / Convert.ToDecimal(CompletionAmount + 1))) == 0 ? 1 : Math.Floor(Convert.ToDecimal(SelectedCompletion) * (Convert.ToDecimal(HeightOfDisplay) / Convert.ToDecimal(CompletionAmount))) == SideBar.Count - 1 ? SideBar.Count - 2 : Math.Floor(Convert.ToDecimal(SelectedCompletion) * (Convert.ToDecimal(HeightOfDisplay) / Convert.ToDecimal(CompletionAmount))))] = true;
                    }
                }
                CompletionDisplay = true;
                MatchingKeywords.Sort();
                int x = Console.CursorLeft + 1;
                int y = Console.CursorTop + 1;
                Console.SetCursorPosition(x, y);
                if (CompletionAmount + 1 > HeightOfDisplay)
                {
                    for (int ii = (SelectedCompletion + 1 > HeightOfDisplay ? SelectedCompletion + 1 - HeightOfDisplay : 0); ii < (SelectedCompletion + 1 > HeightOfDisplay ? SelectedCompletion + 1 - HeightOfDisplay : 0) + HeightOfDisplay; ii++)
                    {
                        Console.Write("\u001b[38;2;" + (ii == SelectedCompletion ? "255;255;255" : "125;125;125") + "m\u001b[48;2;50;50;50m" + MatchingKeywords[ii] + String.Concat(Enumerable.Repeat(" ", 30 - MatchingKeywords[ii].Length)) + "\u001b[m" + "\u001b[48;2;" + (SideBar[ii - (SelectedCompletion + 1 > HeightOfDisplay ? SelectedCompletion + 1 - HeightOfDisplay : 0)] == true ? "200;200;200" : "20;20;20") + "m" + " " + "\u001b[m");
                        Console.SetCursorPosition(x, y + ii + 1 - (SelectedCompletion + 1 > HeightOfDisplay ? SelectedCompletion + 1 - HeightOfDisplay : 0));
                    }
                }
                else
                {
                    for (int ii = 0; ii < CompletionAmount + 1; ii++)
                    {
                        Console.Write("\u001b[38;2;" + (ii == SelectedCompletion ? "255;255;255" : "125;125;125") + "m\u001b[48;2;50;50;50m" + MatchingKeywords[ii] + String.Concat(Enumerable.Repeat(" ", 30 - MatchingKeywords[ii].Length)) + "\u001b[m");
                        Console.SetCursorPosition(x, y + ii + 1);
                    }
                }
                Console.SetCursorPosition(SavedCursorPositions.Item1, SavedCursorPositions.Item2);
            }
            else
            {
                SelectedCompletion = 0;
                CompletionDisplay = false;
            }
        }

        static string ReturnSubstring(string InputString, int StartIndex, int EndIndex)
        {
            string Output = "";
            if (InputString.Length < EndIndex)
            {
                for (int i = StartIndex; i < InputString.Length; i++)
                {
                    Output += InputString[i]; 
                }
            }
            else
            {
                for (int i = StartIndex; i < EndIndex; i++)
                {           
                    Output += InputString[i];
                }
            }
            return Output;
        }

        static string ANSIEscapeSequenceColors()
        {
            string Arrow = "<";
            for (int i = 255; i > 0; i--) 
            {
                Arrow = ($"\x1b[38;2;0;0;0m\x1b[48;2;255;255;255m<\x1b[m"); 
                //if (i % 1 == 0) 
                    Thread.Sleep(1); 
                return Arrow;
            }
            return Arrow;
        }
    }
}