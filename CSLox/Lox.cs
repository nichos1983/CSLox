using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CSLox
{
    public class Lox
    {
        private static readonly Interpreter _interpreter = new Interpreter();
        static bool _hadError = false;
        static bool _hadRuntimeError = false;

        public static void Main(string[] args)
        {
            if(args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                System.Environment.Exit(64);
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
                System.Environment.Exit(65);

            if(_hadRuntimeError)
                System.Environment.Exit(70);
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
            
            Parser parser = new Parser(tokens);
            List<Stmt> statements  = parser.Parse();

            if(_hadError)
                return;
            
            // Console.WriteLine(new ASTPrinter().Print(expression));

            Resolver resolver = new Resolver(_interpreter);
            resolver.Resolve(statements);

            if(_hadError)
                return;

            _interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if(token.Type == TokenType.EOF)
            {
                Report(token.Line, "at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.WriteLine(error.Message + $"\n[line {error.Token.Line}]");
            _hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }
    }
}
