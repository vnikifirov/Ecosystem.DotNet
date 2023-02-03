using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using NUnit.Framework;

using corelib;

using KentavrPlugin;

namespace test2
{

    [TestFixture]
    public class KentavrTest
    {

        const string testFile = "3proezd.dat";

        [Test]
        public void OpenTest()
        {
            KentavrSingleProvider fl = new KentavrSingleProvider(testFile);

#if SHOW
            DataTupleVisualizer dv = new DataTupleVisualizer();
            ListMultiDataProvider l = new ListMultiDataProvider();
            l.PushDataMulti(fl.GetDataMulti());
            dv.SetDataProvider(l);
            System.Windows.Forms.Form dppppp = new System.Windows.Forms.Form();
            dv.Dock = System.Windows.Forms.DockStyle.Fill;
            dppppp.Controls.Add(dv);
            dppppp.ShowDialog();
#endif

        }
    }
}
