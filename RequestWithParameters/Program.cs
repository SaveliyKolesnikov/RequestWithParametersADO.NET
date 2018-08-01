using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ParametrizedRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Parametized request");
            Console.ForegroundColor = ConsoleColor.White;

            var connectionString = CreateConnectionString();
            var commandProvider = new CommandProvider(connectionString);

            do
            {
                var result = commandProvider.InputNewRequest().GetAwaiter().GetResult();
                Console.WriteLine(result);
                Console.WriteLine("Enter 0 to close the program or 1 to continue.");
            } while (Console.ReadLine() != "0");
        }

        private static string CreateConnectionString()
        {

            var projectPath = Directory.GetCurrentDirectory();
            var pathToBinary = Regex.Match(projectPath, "bin.*$", RegexOptions.IgnoreCase).Value;
            if (pathToBinary.Length != 0)
                projectPath = projectPath.Replace(pathToBinary, string.Empty);
            var appDataPath = Path.Combine(projectPath, "AppData\\RecordsDb.mdf");
            var connectionString =
                $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"{appDataPath}\";Integrated Security=True;Connect Timeout=30";

            return connectionString;
        }
    }
}
