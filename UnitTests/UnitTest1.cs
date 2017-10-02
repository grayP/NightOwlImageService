using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nightowlsign.data;

namespace UnitTests
{
    [TestClass]
    public class StoreTests
    {
        public Inightowlsign_Entities mockContext { get; set; }
        public void setupMockContext()
        {
          //  mockContext = new nightowlsign_Entities();

        }

        [TestMethod]
        public void StoreHasProperties()
        {
            setupMockContext();
            var store = new StoreAndSign();
           // store.

            Assert.IsTrue(store.CurrentSchedule.Id == 1);
        }
    }
}
