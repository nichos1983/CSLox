namespace CSLox
{
    class Return : SystemException
    {
        public readonly object? Value;

        public Return(object? value)
        {
            Value = value;
        }
    }
}