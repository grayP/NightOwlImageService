namespace Logger.Logger
{
    public interface IMLogger
    {
        void WriteLog(string lines);
        void Dispose();
    }
}