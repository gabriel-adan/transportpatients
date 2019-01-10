using log4net;
using System;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Logger.Layer.Log.Service
{
    public class LoggerManager
    {
        protected static ILog logger = LogManager.GetLogger(typeof(LoggerManager));

        public static void HandleException(Exception e)
        {
            logger.Error(e);
        }
    }
}
