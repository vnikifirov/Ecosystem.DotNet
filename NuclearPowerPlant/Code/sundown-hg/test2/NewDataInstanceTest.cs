using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using corelib;

using System.Windows.Forms;


namespace test2
{
    [TestFixture]
    public class NewDataInstanceTest
    {
        BasicEnv env = new BasicEnv(new DataParamTable(new TupleMetaData(),
            new DataParamTableItem("coordsCalculation", 5),
            new DataParamTableItem("coordsVisualization", 5)), ".", ".");

        [Test]
        public void SelectComponent()
        {
            IDataResource res = NewDataInstance.SelectComponent(env, null);
        }
    }
}
