using System.Collections.Generic;

namespace CSLox
{
    public class Scanner
    {
        private static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>()
        {
            {"and", TokenType.AND},
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE},
        };

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while(!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch(c)
            {
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if(Match('/'))
                    {
                        SingleLineComment();
                    }
                    else if(Match('*'))
                    {
                        MultiLineComment();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    _line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if(IsDigit(c))
                    {
                        Number();
                    }
                    else if(IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if(IsAtEnd())
                return false;
            if(_source[_current] != expected)
                return false;

            _current++;
            return true;
        }

        private char Peek()
        {
            if(IsAtEnd())
                return '\0';
            return _source[_current];
        }

        private void String()
        {
            while(!IsAtEnd() && Peek() != '"')
            {
                if(Peek() == '\n')
                    _line++;
                Advance();
            }
            
            if(IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            Advance();
            
            string value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.STRING, value);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Number()
        {
            while(IsDigit(Peek()))
                Advance();

            if(Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while(IsDigit(Peek()))
                    Advance();
            }

            double value = double.Parse(_source.Substring(_start, _current - _start));
            AddToken(TokenType.NUMBER, value);
        }

        private char PeekNext()
        {
            if(_current + 1 >= _source.Length)
                return '\0';
            return _source[_current + 1];
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c == '_');
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private void Identifier()
        {
            while(IsAlphaNumeric(Peek()))
                Advance();

            TokenType type = TokenType.IDENTIFIER;
            string text = _source.Substring(_start, _current - _start);
            if(_keywords.ContainsKey(text))
                type = _keywords[text];
            
            AddToken(type);
        }

        private void SingleLineComment()
        {
            while(Peek() != '\n' && !IsAtEnd())
                Advance();
        }

        private void MultiLineComment()
        {
            while(!IsAtEnd() && !(Peek() == '*' && PeekNext() == '/'))
            {
                if(Peek() == '\n')
                    _line++;
                Advance();
            }

            if(IsAtEnd())
            {
                Lox.Error(_line, "Unterminated multi line comment.");
                return;
            }

            Advance();
            Advance();
        }
    }
}