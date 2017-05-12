
namespace Logger.Service
{
    public interface IGeneralLogger
    {
        void Init(string assembly);
        void WriteLog(string lines);
        void WriteLog(string lines, string subject);
        void Dispose();
    }
}