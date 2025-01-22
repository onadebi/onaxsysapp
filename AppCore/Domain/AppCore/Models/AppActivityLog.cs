using AppCore.Domain.AppCore.Config;

namespace AppCore.Domain.AppCore.Dto
{
    public class AppActivityLog
    {
        private readonly string _operation;
        public AppActivityLog(AppOperation operation) { 
            _operation = operation.ToString();
        }

        
    }
}
