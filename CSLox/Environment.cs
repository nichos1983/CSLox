using System.Collections.Generic;

namespace CSLox
{
    public class Environment
    {
        private readonly Environment? _enclosing = null;
        private readonly Dictionary<string, object?> _values = new Dictionary<string, object?>();

        public Environment(Environment? enclosing)
        {
            _enclosing = enclosing;
        }

        public object? Get(Token name)
        {
            if(_values.ContainsKey(name.Lexeme))
                return _values[name.Lexeme];
            
            if(_enclosing != null)
                return _enclosing.Get(name);

            throw new RuntimeError(name, $"Undefined variable '{name}'.");
        }

        public void Define(string name, object? value)
        {
            _values[name] = value;
        }

        public void Assign(Token name, object? value)
        {
            if(_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if(_enclosing != null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}