using System.Collections.Generic;

namespace CSLox
{
    public class Environment
    {
        // public readonly Environment Enclosing;
        private readonly Dictionary<string, object?> _values = new Dictionary<string, object?>();

        public object? Get(Token name)
        {
            if(_values.ContainsKey(name.Lexeme))
                return _values[name.Lexeme];

            throw new RuntimeError(name, $"Undefined variable '{name}'.");
        }

        public void Define(string name, object? value)
        {
            _values[name] = value;
        }
    }
}