
namespace Logger.Logger
{
    public interface IMLogger
    {
        void Init(string assembly);
        void WriteLog(string lines);
        void Dispose();
    }
}