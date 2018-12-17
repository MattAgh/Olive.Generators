using System;

namespace OliveGenerator
{
    class Helper
    {
        static void ShowLine(string text, ConsoleColor color) => Show(text + Environment.NewLine, color);

        static void Show(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(" " + text);
            Console.ResetColor();
        }

        static void Param(string name, string description)
        {
            Show(name + ": ", ConsoleColor.Yellow);
            Console.WriteLine(description);
        }

        public static int ShowHelp()
        {
            ShowLine("Usage pattern:\n", ConsoleColor.Red);
            Show("   accelerate-package ", ConsoleColor.White);
            ShowLine(" extract --out packages.json  | restore packages.json", ConsoleColor.Yellow);
            Console.WriteLine();

            ShowLine("PARAMETERS: ", ConsoleColor.Red);
            Console.WriteLine("--------------------");

            Param("extract", @"It will find all .csproj files in the solution, parse them, and extract a unique list of all nuget packages used across them.");
            Console.WriteLine();

            Param("--out", @"It indicate we will have result in a file");
            Console.WriteLine();

            Param("packages.json", @"it will use as Input with ""Restor"" command and Output as ""Extract"" command");
            Console.WriteLine();

            Console.WriteLine();
            Console.ReadKey(intercept: true);
            return -1;
        }

        public static void ShowError(Exception ex, ConsoleColor color = ConsoleColor.Red)
        {
            Console.ForegroundColor = color;
            Console.Write(" " + ex.Message);
            Console.ResetColor();
        }
    }
}