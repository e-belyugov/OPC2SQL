using Ninject;
using Ninject.Modules;
using OPC2SQL.Types.Abstract;
using OPC2SQL.Types.Concrete;
using System.IO;
using System.Reflection;
using log4net;

namespace OPC2SQL.Client.Infrastructure
{
    /// <summary>Class for Ninject bindings.</summary>
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string s = Path.GetDirectoryName(assembly.Location);
            string filename = s + "\\Settings.xml";

            Bind<ILog>().ToMethod(x => log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType))
                .InSingletonScope();
            Bind<IO2SConfigReader>().To<MyO2SConfigReader>().InSingletonScope().WithConstructorArgument("filename", filename);
        }
    }
}
