using log4net;
using System;
using System.Reflection;
using System.Windows;

namespace Gacha_Transfer_Code_Manager
{

    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Gacha Transfer Code Manager\";

        public static void InitLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
