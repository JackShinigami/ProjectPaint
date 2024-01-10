namespace IKleFileContract
{
    public abstract class IKleFile
    {
        public abstract string ToKleString(int index = 0);
        public abstract object? FromKleString(string kleString);
    }
}
