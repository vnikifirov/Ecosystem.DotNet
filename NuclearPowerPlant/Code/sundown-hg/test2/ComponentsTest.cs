using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using System.Reflection;

using NUnit.Framework;

using corelib;

using System.Windows.Forms;

namespace test2
{
    [TestFixture]
    public class ComponentsTest
    {
        [Test]
        public void SingleFileDataProviderTest()
        {
            DataComponents c = new DataComponents(".");

#if !DOTNET_V11
            List<DataComponents.DataComponent> items = new List<DataComponents.DataComponent>();
#else
            ArrayList items = new ArrayList();
#endif

            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (DataComponents.DataComponent component in c.Components)
            {
                if (component.Info.FileFilter.Length > 0)
                {
                    items.Add(component);
                    if (first)
                        first = false;
                    else
                        sb.Append("|");
                    sb.Append(component.Info.FileFilter);

                }
            }


            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = sb.ToString();
            //fd.Fil
            

        }
    }
}
