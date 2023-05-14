// Main.cs created with MonoDevelop
// User: serg at 11:07 P 5/07/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using System.Reflection;
using System.Security.Permissions;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text.RegularExpressions;

using System.Windows.Forms;

using corelib;

using RockMicoPlugin;

using PgSqlStorage;

using Algorithms;

using System.Diagnostics;
//using AkgoSqlPlugin;

using DataGrid = corelib.DataGrid;

namespace test
{
    class Class1
    {

        public static void nav_LeftClickCoord(object res, CoodrsEventArgs e)
        {
            Console.WriteLine(e.Coords.HumaneLabelYX);
        }


        static void AddSubSearch(string path, string pattern, ArrayList founded)
        {
            string[] dir = System.IO.Directory.GetDirectories(path);
            foreach (string d in dir)
                AddSubSearch(d, pattern, founded);

            string[] files = System.IO.Directory.GetFiles(path, pattern);
            foreach (string f in files)
                founded.Add(f);
        }

        static string[] FixGetFiles(string path, string pattern)
        {
            ArrayList a = new ArrayList();
            AddSubSearch(path, pattern, a);

            string[] res = new string[a.Count];
            a.CopyTo(res);
            return res;
        }


        static void TestDarwMuliFiles(string path)
        {
#if !DOTNET_V11
            string[] files = System.IO.Directory.GetFiles(path, "KATKR.INI", System.IO.SearchOption.AllDirectories);
#else
            string[] files = FixGetFiles(path, "KATKR.INI");
#endif
            ArrayList d = new ArrayList();
            ArrayList u = new ArrayList();

            foreach (string f in files)
            {
                string q = f.Replace("KATKR.INI", "");
                RockMicroFile frockl = new RockMicroFile(q, false);
                d.Add(frockl.GetDataTuple());
                u.Add(frockl.GetDataTuple().GetTimeDate());
            }

            DateTime[] ut = new DateTime[d.Count];
            u.Sort();
            u.CopyTo(ut);            

            int idx = 0;
            DataTuple[] dtem = new DataTuple[d.Count];
            d.CopyTo(dtem);

            DataTuple[] t = new DataTuple[d.Count];
            foreach (DateTime dtt in ut)
            {
                for (int k = 0; k < d.Count; k++)
                {
                    if (dtem[k].GetTimeDate() == dtt)
                        t[idx++] = dtem[k];
                }
            }

            DataTupleVisualizer dv = new DataTupleVisualizer();
            dv.SetMultiTuple(t);

            Form dppppp = new Form();
            dppppp.Controls.Add(dv);
            dv.Dock = DockStyle.Fill;
            dppppp.Show();

			Application.Run(dppppp);

			//dppppp.ShowDialog();
          
        }


        static void TestKgoRecovery()
        {
            String path = @".";

            // Components
            DataComponents dc = new DataComponents(path);
            AlogComponents ac = new AlogComponents(path);
            //GuiComponents gc = new GuiComponents(path);

            // Algo resource
            AlogComponents.AlgoResource convZagr = ac.CreateResource("zagrConvertToInternal", "zagrConvertToInternal");
            AlogComponents.AlgoResource kgoDetectMaxPos = ac.CreateResource("kgoDetectMaxPos", "kgoDetectMaxPos");
            AlogComponents.AlgoResource wockr = ac.CreateResource("calculateMiddlePower", "calculateMiddlePower");
            AlogComponents.AlgoResource evalutCoeff = ac.CreateResource("evaluteAzotDpCoeff", "evaluteAzotDpCoeff");
            AlogComponents.AlgoResource startG = ac.CreateResource("calculateStartG", "calculateStartG");
            AlogComponents.AlgoResource kgoCompactMax = ac.CreateResource("kgoCompactMax", "kgoCompactMax");
            AlogComponents.AlgoResource evaluteAzotDpFlow = ac.CreateResource("evaluteAzotDpFlow", "evaluteAzotDpFlow");
            AlogComponents.AlgoResource kgoDeCompactMax = ac.CreateResource("kgoDeCompactMax", "kgoDeCompactMax");

            // PVK length
            ParseLenPvk dLenPvk = new ParseLenPvk(@"../../../RBMK-Data/KNPP1.csv");

            // RockMicro
            RockMicroFile rockMickro1 = new RockMicroFile(@"../../../RBMK-Data/kgo-23-04-2008/1B23-4-2008_15ч37м/", false);
            IDataTuple zagr1 = rockMickro1.GetDataTuple();

            RockMicroFile rockMickro2 = new RockMicroFile(@"../../../RBMK-Data/kgo-24-04-2008/1B24-4-2008_10ч35м/", false);
            IDataTuple zagr2 = rockMickro2.GetDataTuple();


#if flase
            // AKGO
            ParamTuple parametersConn = new ParamTuple("params");
            parametersConn.Add("akgoConnectionString", "DSN=azotdb;");
            parametersConn.Add("akgoDbSystem", "default");
            ISuperDataResource rc = dc.CreateSuperResource("AkgoSqlProvider", "AkgoSqlProvider", parametersConn);

            IResource akgoDates = rc.CreateDateSource("akgo_dates");
            IResource akgoConsts = rc.CreateContSource("akgo_consts");
            IResource akgoAzot = rc.CreateNamesSource("akgo_azot", "kgoprp_info", "kgoprp_azot");
            ITuple akgoTupleDates = akgoDates.Receive();
            DateTime[] akgoIds = (DataArray)akgoTupleDates.GetParam("multi_dates");
            ITuple akgoConstants = akgoConsts.Receive();
            DataArray somePvkScheme = (DataArray)akgoConstants.GetParam("pvk_scheme");
            ArrayPvkInfo pvk_scheme = new ArrayPvkInfo(somePvkScheme);

            DataTuple pvkTuple = new DataTuple("const", new DateTime(), somePvkScheme);
            pvkTuple.SaveToFile("pvkTuple.tup");

            akgoAzot.Send(new ParamTuple("in", "date", akgoIds[0]));
            IDataTuple[] akgoAzotData2 = (IDataTuple[])akgoAzot.ReceiveMulti();

            akgoAzot.Send(new ParamTuple("in", "date", akgoIds[1]));
            IDataTuple[] akgoAzotData1 = (IDataTuple[])akgoAzot.ReceiveMulti();


            DataTuple.SaveMultiToFile(akgoAzotData2, "akgoAzotData2.tup");
            DataTuple.SaveMultiToFile(akgoAzotData1, "akgoAzotData1.tup");
#else
            IDataTuple[] akgoAzotData2 = DataTuple.LoadMultiFromFile("akgoAzotData2.tup");
            IDataTuple[] akgoAzotData1 = DataTuple.LoadMultiFromFile("akgoAzotData1.tup");
            DataTuple pvkTuple = DataTuple.LoadFromFile("pvkTuple.tup");
            DataArray somePvkScheme = (DataArray)pvkTuple["pvk_scheme"];
            ArrayPvkInfo pvk_scheme = new ArrayPvkInfo(somePvkScheme);


            //IGuiResource graph = gc.CreateResource("GraphComponent", "GraphComponent");
            
            /*
            DataTuple[] graphs = new DataTuple[akgoAzotData1.Length];
            for (int i = 0; i < akgoAzotData1.Length; i++)
            {
                DataParamTable tbl = (DataParamTable)akgoAzotData1[i]["kgoprp_info"];
                graphs[i] = new DataTuple("temp", new DateTime(),
                    new DataParamTable("name", "", new DateTime(), "humaneName",
                        String.Format("Нитка {0} [{1}]", tbl["fiberNum"], DateTime.FromOADate((double)tbl["prpDate"]))),
                        akgoAzotData1[i]["kgoprp_azot"].CastTo(graph.Rules.InputRules("datas").Rule.GetTypeInfo("data") ));
            }
*/
            //graph.SendMulti(graphs);

         //   DialogProvider dppppp = new DialogProvider((Control)graph.BaseInstance);
        //    dppppp.ShowDialog();

#endif
            // Set PVK scheme before operation
            zagr1.SetPvkInfo(pvk_scheme);
            zagr2.SetPvkInfo(pvk_scheme);

#if TESTCARTDIFF
            DataCartogram aaa = (DataCartogram)zagr1["flow"];
            DataCartogram bbb = (DataCartogram)zagr2["flow"];
            DataCartogram ccc = DataCartogram.AbsoluteDiff(aaa, bbb);

            CartView vvv = new CartView();
            vvv.SetDiffLevelCart();
            vvv.SetCart(ccc);
            DialogProvider dppppp = new DialogProvider(vvv);
            dppppp.ShowDialog();

            DataTupleVisualizer dv33 = new DataTupleVisualizer();
            //dv3.SetMultiTuple(koeffs);
            dv33.SetMultiTuple(DataTuple.Group(zagr1, zagr2));
            DialogProvider dppppp33 = new DialogProvider(dv33);
            dppppp33.ShowDialog();
#endif


            // // Detect maximums 1
            IDataTuple[] akgoAnswer1 = kgoDetectMaxPos.CallMultiIntelli(pvk_scheme, convZagr.CallIntelli(zagr1), akgoAzotData1);
            IDataTuple akgoCart1 = kgoCompactMax.CallIntelli(somePvkScheme, akgoAzotData1, akgoAnswer1);

            // //Power wockr
            IDataTuple tmp_wockr1 = wockr.CallIntelli(zagr1);
            // //Evalute adaptation coefficients
            IDataTuple[] koeffs = evalutCoeff.CallMultiIntelli(pvk_scheme, zagr1, tmp_wockr1, dLenPvk.PvkLength, akgoAnswer1, akgoAzotData1);

            // start G
            startG.SendNamed("prev", zagr1);
            startG.SendNamed("curr", zagr2);
            DataTuple start_flow = (DataTuple)startG.Receive();

            // // Detect maximums 2
            IDataTuple[] akgoAnswer2 = kgoDetectMaxPos.CallMultiIntelli(pvk_scheme, convZagr.CallIntelli(zagr2), akgoAzotData2);
            IDataTuple akgoCart2 = kgoCompactMax.CallIntelli(somePvkScheme, akgoAzotData2, akgoAnswer2);

            // //Power wockr 2
            IDataTuple tmp_wockr2 = wockr.CallIntelli(zagr2);
            // //Evalute adaptation coefficients
            IDataTuple[] koeffs2 = evalutCoeff.CallMultiIntelli(pvk_scheme, zagr2, tmp_wockr2, dLenPvk.PvkLength, akgoAnswer2, akgoAzotData2);

            // Recovery
            IDataTuple[] recovered =  evaluteAzotDpFlow.CallMultiIntelli(pvk_scheme, zagr2, tmp_wockr2, dLenPvk.PvkLength, akgoAnswer2, akgoAzotData2,
                DataTuple.GetMultiItem(koeffs, "bet1"), DataTuple.GetMultiItem(koeffs, "kpd"),
                DataTuple.GetMultiItem(koeffs2, "gamma"));

            IDataTuple[] recovered2 = evaluteAzotDpFlow.CallMultiIntelli(pvk_scheme, zagr2, tmp_wockr2, dLenPvk.PvkLength, akgoAnswer2, akgoAzotData2,
                DataTuple.GetMultiItem(koeffs, "bet1"), DataTuple.GetMultiItem(koeffs, "kpd"),
                DataTuple.GetMultiItem(koeffs, "gamma"));

            IDataTuple[] recovered3 = evaluteAzotDpFlow.CallMultiIntelli(pvk_scheme, zagr2, tmp_wockr2, dLenPvk.PvkLength, akgoAnswer2, akgoAzotData2,
                DataTuple.GetMultiItem(koeffs2, "bet1"), DataTuple.GetMultiItem(koeffs, "kpd"),
                DataTuple.GetMultiItem(koeffs, "gamma"));

            IDataTuple flowReovery_DP = kgoCompactMax.CallIntelli(pvk_scheme, new TupleMaps("flow_dp as pvk_maxes, gamma as kgoprp_info"), recovered, somePvkScheme, koeffs);

            IDataTuple flowReovery = kgoCompactMax.CallIntelli(pvk_scheme, new TupleMaps("flow_az1 as pvk_maxes, gamma as kgoprp_info"), recovered, somePvkScheme, koeffs);
            IDataTuple flowReovery2 = kgoCompactMax.CallIntelli(pvk_scheme, new TupleMaps("flow_az1 as pvk_maxes, gamma as kgoprp_info"), recovered2, somePvkScheme, koeffs);
            IDataTuple flowReovery3 = kgoCompactMax.CallIntelli(pvk_scheme, new TupleMaps("flow_az1 as pvk_maxes, gamma as kgoprp_info"), recovered3, somePvkScheme, koeffs);


            IDataTuple[] orig = kgoDeCompactMax.CallMultiIntelli(pvk_scheme, new TupleMaps("pvk_maxes as pvk_maxes_cart"), flowReovery);

    //        DataTupleVisualizer dv = new DataTupleVisualizer();
  //          dv.SetMultiTuple(koeffs );

            //dv.SetMultiTuple(recovered);
            //dv.SetMultiTuple(koeffs);
            //dv.SetMultiTuple(new IDataTuple[] { zagr1, zagr2, start_flow });
            //dv.SetTuple( zagr1);
            //dv.SetTuple(evalMicro);
//            DialogProvider dppppp = new DialogProvider(dv);
        //    dppppp.ShowDialog();



            DataTupleVisualizer dv3 = new DataTupleVisualizer();
            //dv3.SetMultiTuple(koeffs);
            dv3.SetMultiTuple(DataTuple.Group(zagr1, zagr2, flowReovery_DP, flowReovery, flowReovery2, flowReovery3, akgoCart1, akgoCart2));
            //DialogProvider dppppp3 = new DialogProvider(dv3);
            //dppppp3.ShowDialog();

#if false
            DataTupleVisualizer dv3 = new DataTupleVisualizer();
            //dv3.SetMultiTuple(koeffs);
            dv3.SetMultiTuple(DataTuple.Group(zagr1, zagr2));
            DialogProvider dppppp3 = new DialogProvider(dv3);
            dppppp3.Show();

            DataTupleVisualizer dv2 = new DataTupleVisualizer();
            //dv2.SetMultiTuple(recovered); SetTuple
            dv2.SetMultiTuple(DataTuple.Group(flowReovery_DP, flowReovery, flowReovery2, flowReovery3, akgoCart1, akgoCart2));
            DialogProvider dppppp2 = new DialogProvider(dv2);
            dppppp2.Show();

            Application.Run();
#endif

        }

	    [PermissionSetAttribute(SecurityAction.Demand, Name="FullTrust")]
        [STAThread]
	    public static void Main(string[] args)
        {

            DataGrid d = new DataGrid();

            d.Columns.Add("Index", typeof(int));
            d.Columns.Add("Value", typeof(double));

            Debug.Assert(d.ColumnCount == 2);

            d.Rows.Add(5, 3.0);
            d.Rows.Add(15, 23.0);
            d.Rows.Add(25, 13.0);
            d.Rows.Add(65, 13.33);

            Debug.Assert(d.RowCount == 4);

            foreach (DataGrid.Row r in d.StringRows)
            {
                Debug.WriteLine(r);
            }

            Debug.WriteLine("");
            Debug.WriteLine(d.DumpCSV());

            d.Columns.Add("Coords", typeof(Coords));

            for (int j = 0; j < d.RowCount; j++ )
                d.Rows[j][2] = Coords.FromLinear1884(j);

            d.Columns.Add("Date", typeof(DateTime));
            d.Columns.Add("Help", typeof(string));

            d.Rows[3][3] = DateTime.Now;

            d.ExportExcel();

            Debug.WriteLine("");
            Debug.WriteLine(d.DumpCSV());

            return;

            Application.EnableVisualStyles();

            /*
            DataTuple tt = DataTuple.LoadFromFile("pvk_Const.tup");
            DataTupleVisualizer dv2 = new DataTupleVisualizer();
            dv2.SetTuple(tt);

            DialogProvider dppppp2 = new DialogProvider(dv2);
            Application.Run(dppppp2);
            return;
            */

            RockMicroFile frockl22 = new RockMicroFile(@"../../../../RBMK-Data/kgo-24-04-2008/1B24-4-2008_10ч41м/", false);
            RockMicroFile frockl23 = new RockMicroFile(@"../../../../RBMK-Data/kgo-24-04-2008/1B24-4-2008_10ч35м/", false);

            IDataTuple testTuple = frockl23.GetDataTuple();
            IDataTuple testTuple2 = frockl22.GetDataTuple();
            DataTupleVisualizer dv3 = new DataTupleVisualizer();
            dv3.SetMultiTuple(new IDataTuple[] { testTuple, testTuple2 });
         //   DialogProvider dppppp3 = new DialogProvider(dv3);
      //      dppppp3.ShowDialog();

            ITupleItem i = testTuple["dkv_1"];
            ISerializeStream iss = i.Serialize();
            byte[] rdata = iss.GetData();

            RawTupleItem ds = new RawTupleItem("a", DateTime.Now, rdata);
            ITupleItem it = ds.Restore();


            return;


            //PgSqlProvider prvclear = new PgSqlProvider("Server=127.0.0.1;User id=postgres;password=qwer;database=kgo"); prvclear.ClearAllData();

            ParamTuple config = ParamTuple.LoadFromXML("config.xml");
           // SunEnv s = new SunEnv(config);

           // Application.Run(new MainForm(s));
        //    return;

            //s.ImportToBase(true, null);
           // s.UpdateDates();

           // s.ViewMultiTuple(null, s.SimpleRecoveryWithCoeffRecoveryAndNewGamma(s.GetDates()[1], s.GetDates()[0]));
            //s.ViewMultiTuple(null, s.SimpleRecoveryWithCoeffRecovery(s.GetDates()[1], s.GetDates()[0]));
            //s.ViewMultiTuple(null, s.SimpleRecovery(s.GetDates()[1], s.GetDates()[0]) );


      //      DataTupleVisualizer dv3 = new DataTupleVisualizer();
    //        dv3.SetMultiTuple(s.GetAllData());
  //          DialogProvider dppppp3 = new DialogProvider(dv3);
//            dppppp3.ShowDialog();

            



            return;

            TestKgoRecovery();



            //TestDarwMuliFiles(@"../../../RBMK-Data/");
            

            //TestDarwMuliFiles(@"../../../RBMK-Data/kgo-24-04-2008/");
            


            TupleMaps.TupleParamMap test__s1 = new TupleMaps.TupleParamMap("spirit as spirited_away");
            TupleMaps.TupleParamMap test__s2 = new TupleMaps.TupleParamMap("tt.spirit as spirited_away");
            TupleMaps.TupleParamMap test__s3 = new TupleMaps.TupleParamMap("tt.spirit as mm.spirited_away");
            TupleMaps.TupleParamMap test__s4 = new TupleMaps.TupleParamMap("spirit as mm.spirited_away");

            TupleMaps test__MAIN = new TupleMaps("spirit as spirited_away,tt.nospirit as nospirited_away");
#if d
            #region AttributesTest
            /*
            Teest t = new Teest();
            MethodInfo mInfo = t.GetType().GetMethod("superMethod");
            foreach (Attribute attr in
                    Attribute.GetCustomAttributes(mInfo))
            {
                if (attr.GetType() != typeof(RulesAttribute))
                    continue;

                RulesAttribute mattr = (RulesAttribute)attr;
            }
            */

            AttributeSingleTupleRules z = new AttributeSingleTupleRules(
                "a contains (power, flow)");

            AttributeSingleTupleRules a = new AttributeSingleTupleRules(
                "a [] contains (power, flow) cast (power as cart(pvk), flow as array(int, 3))");

            AttributeSingleTupleRules b = new AttributeSingleTupleRules(
                "a [1] contains (power, flow) cast (power as cart(pvk), flow as array(int, 3))");

            AttributeSingleTupleRules c = new AttributeSingleTupleRules(
                "a [mouse] contains (power, flow) cast (power as cart(pvk), flow as array(int, 3))");

            AttributeSingleTupleRules d = new AttributeSingleTupleRules(
                "a contains (power, flow) cast (power as cart(pvk), flow as array(int, 3))");

            AttributeRules r = new AttributeRules(
                "a contains (power, flow) cast (power as cart(pvk), flow as array(int, 3));" +
                "b contains (power, flow) cast (power as cart(pvk), flow as array(int, 3))");

            AttributeRules q = new AttributeRules(
                "a contains (power, flow) cast (power as cart(pvk), flow as array(int, 3));" +
                "b contains (power, flow) cast (power as cart(pvk), flow as array(int, 3));" +
                "return contains (power, flow) cast (power as cart(pvk), flow as array(int, 3))"
            );

            AttributeRules qp = new AttributeRules(
                "a contains (power, flow) cast (power as cart(pvk), flow as array(int, 3));" +
                "a[] contains (power, flow) cast (power as cart(pvk), flow as array(int, 3));" +
                "return contains (power, flow) cast (power as cart(pvk), flow as array(int, 3));"
            );
            
            ITupleRules rrrr = qp.SingleInputRules;

            //AttributeRules qp2 = new AttributeRules("t contains (power) ; return contains (power-mod)");
            #endregion
#endif

            Hashtable tbl = new Hashtable();
            tbl.Add("numberOfRestarts", "1");
            tbl.Add("numberOfRestarts2", "меганастройка");

            #region DataCheck
            MultiIntFloat fl = new MultiIntFloat(0.1f);
            DataArray da = new DataArray("n", "h", DateTime.Now, typeof(int), 10, 1, 0);
            da[8,0] = 10;
            da[0,0] = 100;
            da[1, 0] = 255;
            da[2, 0] = 65535;
            da[3, 0] = -2;

            ISerializeStream sstream = da.Serialize();
            ///int[] vals = da.GetArrayInt();

            AwfulSerializer aser = new AwfulSerializer(10);
            aser.Put(10);
            //aser.Put(da.RawArray);
            aser.Put((short)-1);

            byte[] datas = aser.Data;
            

            AwfulDeserializer dser = new AwfulDeserializer(datas, 0);
            int val;
            short after;
            dser.Get(out val);
            Array outar = dser.GetArray(typeof(int), val, 0, 0);
            dser.Get(out after);
            #endregion

            //dser.Get(out val);
            //dser.Get(out val);
            //dser.Get(out val);
            StreamDeserializer dsser = new StreamDeserializer(sstream.GetData(), sstream.GetName(), sstream.GetTimeDate(), sstream.GetHumanName());
            DataArray dab = new DataArray(dsser);

            #region TesetCoord

            DataArray test_da1 = new DataArray("t", "t", DateTime.Now, typeof(short), 48, 48, 0);
            test_da1[47, 20] = (short)1000;
            ScaleIndex si = new ScaleIndex(0, 0.001);

            DataCartogram test_cda1 = DataCartogram.Create(test_da1, si);
            double test_valcda = test_cda1[new Coords(47, 20)];

            DataCartogram test_cda1_linear = test_cda1.Convert(DataCartogram.CartogramType.Linear);
            DataCartogram test_cda1_wlinear = test_cda1.Convert(DataCartogram.CartogramType.LinearWide);

            DataCartogram test_cda1_d = test_cda1.ToDouble();
            DataCartogram test_cda1_linear_d = test_cda1_linear.ToDouble();

            DataCartogram test_cda1_f = test_cda1.ToType(typeof(float), ScaleIndex.Default) ;

            AttributeTypeRules test_conv_rule = new AttributeTypeRules("t as Cart(linear, float)");
            AttributeTypeRules test_conv_rule2 = new AttributeTypeRules("t as Cart(native, float, scale)");

            DataCartogram test_rule = test_cda1.CastTo(test_conv_rule);
            DataCartogram test_rule2 = test_cda1.CastTo(test_conv_rule2);


            DataParamTable ddd = new DataParamTable("q", "ee", DateTime.Now, tbl);
            byte[] raw = ddd.Serialize().GetData();

            DataParamTable ddd_restored = new DataParamTable(new StreamDeserializer(raw, "q", DateTime.Now, "ee"));

            raw = test_rule.Serialize().GetData();
            DataCartogram test_rule_r = DataCartogram.Create(new StreamDeserializer(raw, "q", DateTime.Now, "ee"));

            raw = test_rule2.Serialize().GetData();
            DataCartogram test_rule2_r = DataCartogram.Create(new StreamDeserializer(raw, "q", DateTime.Now, "ee"));

            #endregion

#if MONO			
            RockMicroFile frockl = new RockMicroFile(@"/home/serg/FABULA_11/UNIT1/", false);
#else
            RockMicroFile frockl = new RockMicroFile(@"../../../RBMK-Data/kgo-24-04-2008/1B24-4-2008_10ч45м/", false);
            RockMicroFile frockl2 = new RockMicroFile(@"../../../RBMK-Data/kgo-24-04-2008/1B24-4-2008_10ч41м/", false);
            RockMicroFile frockl3 = new RockMicroFile(@"../../../RBMK-Data/kgo-24-04-2008/1B24-4-2008_10ч35м/", false);	
#endif
            IDataTuple eee = frockl.GetDataTuple();

            DataCartogram frockl_rule2 = (DataCartogram)eee["zagr_rockmicro"].CastTo(test_conv_rule2);
            DataCartogram frockl_rule2f = (DataCartogram)eee["flow"].CastTo(test_conv_rule2);
            DataCartogram frockl_rule2fs = (DataCartogram)eee["dkr_1"].CastTo(test_conv_rule2);

            double frockl_rule2_d = frockl_rule2[new Coords(0, 17)];
            double frockl_rule2_d1 = frockl_rule2[new Coords(0, 18)];
            double frockl_rule2_d2 = frockl_rule2[new Coords(0, 19)];
            double frockl_rule2_d21 = frockl_rule2.RawArrayFloat[19, 0];
            double frockl_rule2_d22 = frockl_rule2.RawArrayFloat[0, 19];

            DataCartogram frockl_rule2suz = (DataCartogram)eee["suz"].CastTo(test_conv_rule2);

            IDataTuple ttt = eee.CastTo(new AttributeSingleTupleRules(
                "a contains (zagr_rockmicro, power, flow) cast (power as Cart(native,double,scale), flow as Cart(native,double,scale))"));

            DataCartogram m = frockl_rule2.Clone("new", "ttt", DateTime.Now);

            DataCartogram someZagr = (DataCartogram)eee["zagr_rockmicro"];

            DataCartogram someFlow = (DataCartogram)eee["flow"];

            ParseLenPvk dLenPvk = new ParseLenPvk(@"../../../RBMK-Data/KNPP4.csv");

#if false
            DataTupleVisualizer dv = new DataTupleVisualizer();
            dv.SetMultiTuple(new IDataTuple[] { frockl.GetDataTuple(), frockl2.GetDataTuple(), frockl3.GetDataTuple() });

            DialogProvider dppppp = new DialogProvider(dv);

        //    dv.SetTuple(eee);
            dppppp.ShowDialog();
#endif
#if false
            CartView cvvv = new CartView();
            cvvv.SetCart((DataCartogram)eee["energovir"]);
           // cvvv.SetCart(dLenPvk);
            cvvv._nav.LeftClickCoord += new CoordsEventDelegate(nav_LeftClickCoord);
            DialogProvider dppppp = new DialogProvider(cvvv);
            dppppp.ShowDialog();
#endif

#if DOTNET_V11
#if ALTER_
			String path = @"C:\sundown\test2\bin\Debug-MS11";
#else
            String path = @"C:\sundown\test2\bin\Debug";
#endif
#else
#if MONO
            String path = @"/home/serg/sundown/test2/bin/Debug";			
#else
            String path = @"C:\sundown\test2\bin\Debug_NET2";
#endif
#endif

            #region PgSqlProvider Test

            PgSqlProvider prv = new PgSqlProvider("Server=127.0.0.1;User id=postgres;password=qwer;database=kgo");

            prv.PushData(eee);

            DateTime[] dts = prv.GetDates();
            string[] streams = prv.GetStreamNames();
            string[] allNames = prv.GetExistNames();

            IDataTuple trueData = prv.GetData(dts[0], streams[0]);

            prv.ClearAllData();

            #endregion

            DataComponents dc = new DataComponents(path);

            //Hashtable parametersConn = new Hashtable();
            ParamTuple parametersConn = new ParamTuple("params");
            parametersConn.Add("connctionString", "Server=127.0.0.1;User id=postgres;password=qwer;database=kgo");
            ISuperDataResource dataResource = dc.CreateSuperResource("PgSqlProvider", "main", parametersConn);

            IResource rcdata = dataResource.CreateDateSource("main_dates");
            ITuple rcdatatuple = rcdata.Receive();

            dataResource.Dispose();


            IDataComponent data = dc["Class1"];

			AlogComponents ac = new AlogComponents(path);

            IResource rsc = ac.CreateResource("processPowerValues", "processPowerValues");
            IResource rsc2 = ac.CreateResource("processPowerMiddleValues", "processPowerMiddleValues");


            DataTuple nt = new DataTuple(ttt);
            nt.StreamName = "t";
            rsc.Send(nt);
            ITuple return_data = rsc.Receive();

            DataTuple newDynTuple = new DataTuple(
                new AttributeSingleTupleRules("t contains (power) cast (power as Cart(linear, float, scale))"),
                nt );

            rsc2.Send(newDynTuple);
            ITuple return_data2 = rsc2.Receive();


            //DataTuple newAutoTuple = new DataTuple(rsc2.Rules.SingleInputRules.Rule, new DataTuple[] { nt });
            DataTuple newAutoTuple = new DataTuple(rsc2.Rules.SingleInputRules.Rule,  nt );
            rsc2.Send(newDynTuple);
            ITuple return_data3 = rsc2.Receive();


#if NOT_DEFINED
            AkgoSqlDb db = new AkgoSqlDb("DSN=azotdb;");
            AkgoSqlDb.AkgoSystemInfo akgosi = db[0];
            akgosi.UpdateActCache();
            DateTime testDate = akgosi.CachedDates[0];
            AkgoSqlDb.FiberRecord[] recs = akgosi.GetAllFiberAzotRecords(testDate);
#endif

            parametersConn.Add("akgoConnectionString", "DSN=azotdb;");
            parametersConn.Add("akgoDbSystem", "default");
            ISuperDataResource rc = dc.CreateSuperResource("AkgoSqlProvider", "AkgoSqlProvider", parametersConn);

            IResource akgoDates = rc.CreateDateSource("akgo_dates");
            IResource akgoConsts = rc.CreateContSource("akgo_consts");
            IResource akgoAzot = rc.CreateNamesSource("akgo_azot", "kgoprp_info", "kgoprp_azot");
            ITuple akgoTupleDates = akgoDates.Receive();
            DateTime[] akgoIds = (DataArray)akgoTupleDates.GetParam("multi_dates");

            ITuple akgoConstants = akgoConsts.Receive();

            //akgoAzot.Send(new ParamTuple("in", "date", akgoIds[0]));
            akgoAzot.Send(new ParamTuple("in", "date", akgoIds[0]));
            ITuple[] akgoAzotData = akgoAzot.ReceiveMulti();

            rc.Dispose();
            //AkgoSqlDb

            DataArray somePvkScheme = (DataArray)akgoConstants.GetParam("pvk_scheme");

            IResource convZagr = ac.CreateResource("zagrConvertToInternal", "zagrConvertToInternal");
            convZagr.Send(new DataTuple("zagr", someZagr.GetTimeDate(), someZagr ));
            IDataTuple zagrNative = (IDataTuple)convZagr.Receive();


            IDataTuple zagrTuple = new DataTuple("zagr", someZagr.GetTimeDate(), zagrNative["zagr"], somePvkScheme);
            //IDataTuple zagrTuple = new DataTuple("zagr", someZagr.GetTimeDate(), new String[] { "zagr", "pvk_scheme" }, someZagr, somePvkScheme);

            IDataTuple[] n = DataTuple.CreateMultiTuple("kgo",
                new string[] { "kgoprp_info", "kgoprp_azot" }, (IDataTuple[])akgoAzotData);
               // new IDataTuple[][] { (IDataTuple[])akgoAzotData });

            IResource nrc = ac.CreateResource("kgoDetectMaxPos", "kgoDetectMaxPos");

            nrc.Send(zagrTuple);
            nrc.SendMulti(n);

            someZagr.SetPvkInfo(new ArrayPvkInfo(somePvkScheme));
            DataCartogram Newc = someZagr.CastTo(new AttributeTypeRules("t as Cart(pvk)"));

            ITuple[] akgoAnswer = nrc.ReceiveMulti();


            IResource kgoCompactMax = ac.CreateResource("kgoCompactMax", "kgoCompactMax");
            kgoCompactMax.Send(new DataTuple(kgoCompactMax.Rules.InputRules("pvks").Rule, (DataTuple)akgoConstants));
            kgoCompactMax.SendMulti(DataTuple.CreateMultiTuple("maxes", new string[] { "kgoprp_info", "pvk_maxes" },
                (IDataTuple[])akgoAnswer, (IDataTuple[])akgoAzotData));

            IDataTuple maxesInDia = (IDataTuple)kgoCompactMax.Receive();
            DataCartogram cartMaxes = (DataCartogram)maxesInDia["pvk_maxes_cart"];


            //DataTupleVisualizer dv = new DataTupleVisualizer();
            //DialogProvider dppppp = new DialogProvider(dv);


            //dv.SetMultiTuple((IDataTuple[])akgoAzotData);
            //dv.SetTuple((IDataTuple)akgoAzotData[0]);
            //dv.SetTuple((IDataTuple)akgoAnswer[0]);
            //dv.SetTuple(new DataTuple("t", new DateTime(), somePvkScheme));
            //dppppp.ShowDialog();

#if false
            CartView cvvv_maxesInDia = new CartView();
            cvvv_maxesInDia.SetCart((DataCartogram)cartMaxes);
            DialogProvider dpppppQ = new DialogProvider(cvvv_maxesInDia);
            dpppppQ.ShowDialog();
#endif
#if false
            someFlow.SetPvkInfo(new ArrayPvkInfo(somePvkScheme));
            DataCartogram newFlowPvk = someFlow.CastTo(new AttributeTypeRules("t as Cart(pvk)"));            
            CartView cvvv = new CartView();
            cvvv.SetCart((DataCartogram)newFlowPvk);
            cvvv._nav.LeftClickCoord += new CoordsEventDelegate(nav_LeftClickCoord);
            DialogProvider dppppp = new DialogProvider(cvvv);
            dppppp.ShowDialog();
#endif

			//TestEnviroment te = new TestEnviroment(path);
			//te.CreateThread("default");
			//TestEnviroment.TestThreadEnviroment en = te.Def;
			
			
			//GuiComponents gc = new GuiComponents(path);

            //IGuiResource graph = gc.CreateResource("GraphComponent", "GraphComponent");
            /*
            IDataTuple[] converted = new IDataTuple[akgoAnswer.Length];
            for (int ggg = 0; ggg < akgoAnswer.Length; ggg++)
            {
                converted[ggg] = new DataTuple(graph.Rules.InputRules("datas").Rule,
                                    new TupleMaps("kgoprp_azot as data"), (DataTuple)akgoAzotData[ggg]);
            }
            */
            //graph.SendMulti(converted);
            //graph.Send(converted[0]);


            //IGuiResource res = gc.CreateResource("UserControl1", "UserControl1");
            //IGuiResource res2 = gc.CreateResource("UserControl1", "UserControl1");

            //IGuiResource res3 = gc.CreateResource("UserControl1", "UserControl1");
            //IGuiResource res_cont = gc.CreateResource("Form1", "Form1");
            //IGuiResource res_cont2 = te.GuiComponents.CreateResource("Form1");
           // res_cont.PlaceGuiResource(res);
           // res_cont.PlaceGuiResource(res2);
           // res_cont.PlaceGuiResource(res3);
            //res_cont.PlaceGuiResource(res_cont2);

            //res_cont.PlaceGuiResource(graph);

           // res.OutputChanged += new GuiResourceEvent(Class1.TestEvent);
           // res2.OutputChanged += new GuiResourceEvent(Class1.TestEvent);
            //res3.OutputChanged += new GuiResourceEvent(Class1.TestEvent);

           // graph.OutputChanged += new GuiResourceEvent(Class1.TestEvent);


    //        DialogProvider dlg = new DialogProvider(res_cont);
      //      dlg.ShowDialog();

            //DialogProvider dlg = new DialogProvider(res2);
            //dlg.ShowDialog();
            //Application.SetCompatibleTextRenderingDefault(false);

            //Form f = res_cont.BaseInstance as Form;
            //f.ShowDialog();

            #region Deprecated
            /*

	        Assembly a = Assembly.LoadFrom(@"C:\sundown\testlib\bin\Debug\testlib.dll");
	        Type[] mytypes = a.GetTypes();

            BindingFlags flags = (BindingFlags.Static | BindingFlags.Public );

	        foreach(Type t in mytypes)
	        {
	            MethodInfo[] mi = t.GetMethods(flags);
                if (mi.Length == 0)
                    continue;	            

                MethodInfo q = t.GetMethod("describeDataProvider", new Type[0]);
                Type interf = t.GetInterface("IAwesomeInterface");

                
                ConstructorInfo[] mi2 = t.GetConstructors();
                ParameterInfo[] tp2 = mi2[0].GetParameters();

                Object[] param = new Object[tp2.Length];

                for(int i = 0; i < tp2.Length; i++)
                {
                    Object ob = tbl[tp2[i].Name];
                    if (ob == null)
                        throw new ArgumentException("Not Found");
                    Type type = tp2[i].ParameterType;

                    param[i] = CastObject(ob, type);
                }

                Object obj = mi2[0].Invoke(param);


                foreach (MethodInfo m in mi)
                {
                    if (m.Name == "describeDataProvider")
                    {
                        bool rs = (m.ReturnType == typeof(void));

                        Console.WriteLine(m);

                        Object res = m.Invoke(null, null);
                    }
                }
            }
            
*/
            #endregion
	    }
	}

#if DEFFF
    // rockMicro  (gui) : env(?rockDefPath), in(),  out (zagr_micro,flow,power,energovir)
#endif
#if FALSE_DEFINE
    // this       (gui) : in(), out(zagr_micro,flow,power,energovir,pvk_maxes_cart)
    // fileSlRock (gui) : new{defpath, filedetail}, in(), out{filename}
    // rockMicro        : new{filename}, in(), out(zagr_micro,flow,power,energovir	)
    // zagrCovnert      : in(zagr_micro), out(zagr)
    // akgoSelect (gui) : new{akgoConnectionString, akgoDbSystem}, in(), out(kgoprp_info[], kgoprp_azot[], pvk_scheme[])
    // akgoMaxPos       : in($kgo(kgoprp_info[], kgoprp_azot[]), $zagr(zagr, pvk_scheme)) out(pvk_errorlevel[], pvk_maxes[])
    // akgoCheck  (gui) : in($zagr(zagr_micro), $kgo(pvk_errorlevel[], pvk_maxes[])) out(pvk_maxes_cart)

    zagrCovnert = gnew ZagrConvert;

    fileSlRock = gnew FileGuiSelect {defpath, filedetail} <- {'',''};
    rockMicro = dnew RockMicroSingleProvider {path} <- fileSlRock{filename} ;

    zagrCovnert(zagr_micro) <- rockMicro(zagr_micro);

    akgoSelect = gnew AkgoSelectGui {akgoConnectionString, akgoDbSystem} <- { env {akgoConnectionString}, 'default' };

    akgoMaxPos $kgo(kgoprp_info[], kgoprp_azot[]) <= akgoSelect(kgoprp_info[], kgoprp_azot[]), $zagr(pvk_scheme, zagr) <- (akgoSelect (pvk_scheme[0]), zagrCovnert(zagr));

    akgoCheck $zagr(zagr_micro) <- rockMicro(zagr_micro),  $kgo(zagr_micro) <- akgoMaxPos(pvk_errorlevel[], pvk_maxes[]);

    this(zagr_micro,flow,power,energovir,pvk_maxes_cart) <-
        (rockMicro(zagr_micro,flow,power,energovir), akgoCheck(pvk_maxes_cart));
#endif

#if NOT_DEF
    class NIL
    {
        type { 

        in  [  ]
        out [   ]
    }

#endif

}