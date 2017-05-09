
namespace Logger.Logger
{
    public interface IMLogger
    {
        void Init(string assembly);
        void WriteLog(string lines);
        void WriteLog(string lines, string subject);
        void Dispose();
    }
}