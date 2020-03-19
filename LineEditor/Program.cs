using LineEditor.Properties;
using System;
using System.IO;
using System.Linq;

namespace LineEditor
{
    /*
     * I didn't use oop style patterns like 'commands' and factory of commands
     * because there is no necessary to make queue and interactions between commands like powershell '|'.
     * Solution looks simple because task was simple
    */
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine(Resources.FilePathEmpty);
                return;
            }
            
            var path = args.First();
            if (!File.Exists(path))
            {
                Console.WriteLine(Resources.FileNotFound);
                return;
            }

            ProcessDialog(path);
        }

        static void ProcessDialog(string path)
        {
            var lines = File.ReadAllLines(path);
            if (!lines.Any())
            {
                Console.WriteLine(Resources.FileEmpty);
                return;
            }

            while (true)
            {
                var command = Console.ReadLine();
                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Resources.CommandEmpty);
                    continue;
                }

                switch (command)
                {
                    case "quit":
                        return;
                    case "save":
                        try
                        {
                            File.WriteAllLines(path, lines);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"{ex.GetType()} thrown: {ex.Message}");
                        }
                        continue;
                    case "list":
                        for (var i = 0; i < lines.Length; i++)
                            Console.WriteLine($"{i+1}: {lines[i]}");
                        continue;
                    default:
                        break;
                }

                var result = TryParseInsertDelete(lines, command);
                if (result == null)
                {
                    Console.WriteLine(Resources.WrongCommand);
                    continue;
                }
                lines = result;
            }
        }

        static string[] TryParseInsertDelete(string[] lines, string command)
        {
            var lineNumber = ParseLineNumber(command);
            if (!lineNumber.HasValue || lineNumber < 1 || lineNumber > lines.Length)
                return null;

            var newLines = lines.ToList();
            if (command.StartsWith("ins "))
            {
                newLines.Insert(lineNumber.Value, "new line");
                return newLines.ToArray();
            }
            else if (command.StartsWith("del "))
            {
                newLines.RemoveAt(lineNumber.Value);
                return newLines.ToArray();
            }

            return null;
        }

        static int? ParseLineNumber(string command)
        {
            var parts = command.Split(' ');
            if (!string.IsNullOrEmpty(parts[1]) && int.TryParse(parts[1], out int result))
                return result - 1;
            return null;
        }
    }
}