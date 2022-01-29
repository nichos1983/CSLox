using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CSLox
{
    public class Lox
    {
        private static bool _hadError = false;

        public static void Main(string[] args)
        {
            if(args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                Environment.Exit(64);
            }
            else if(args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            if(!File.Exists(path))
                throw new IOException($"File does not exist: {path}");
            
            string fullPath = Path.GetFullPath(path);
            byte[] bytes = File.ReadAllBytes(fullPath);
            string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Run(str);

            if(_hadError)
                Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            for(;;)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if(line == null)
                    break;
                Run(line);
                _hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach(Token token in tokens)
            {
                Console.WriteLine(token.ToString());
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }
    }
}
