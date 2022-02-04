namespace CSLox
{
    public class RuntimeError : System.SystemException
    {
        public readonly Token Token;
        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}