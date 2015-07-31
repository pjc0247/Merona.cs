using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeronaTest
{
    [TestClass]
    public class TestBase
    {
        [AssemblyInitialize]
        public static void SetupTest(TestContext tc)
        {
            Console.WriteLine("TestBase::Setup");

            Assembly assembly = Assembly.GetExecutingAssembly();

            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);  
        }
    }
}
