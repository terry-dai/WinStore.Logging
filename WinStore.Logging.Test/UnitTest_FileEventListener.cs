using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Diagnostics.Tracing;
using Windows.ApplicationModel;
using Windows.Storage;
using WinStore.Logging.Test.Mock;

namespace WinStore.Logging.Test
{
    [TestClass]
    public class UnitTest_FileEventListener
    {
        [TestMethod]
        public void TestMethod_1_GetLogName_Default()
        {
            FileEventListener listener = new FileEventListener();
            string expectedName = Package.Current.Id.Name + "_log.csv";
            Assert.AreEqual<string>(expectedName, listener.GetLogName());
        }

        [TestMethod]
        public void TestMethod_2_GetLogName_abc()
        {
            FileEventListener listener = new FileEventListener("abc");
            string expectedName = "abc" + "_log.csv";
            Assert.AreEqual<string>(expectedName, listener.GetLogName());
        }

        [TestMethod]
        public void TestMethod_3_GetLogName_ASpaceB()
        {
            FileEventListener listener = new FileEventListener("A B");
            string expectedName = "A_B" + "_log.csv";
            Assert.AreEqual<string>(expectedName, listener.GetLogName());
        }
    }
}
