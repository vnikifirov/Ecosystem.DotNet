using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;
using corelib;

namespace corelib
{

    class DTVSunEnvAdd : ActionDataTupleVisualizerUI
    {
        public DTVSunEnvAdd()
        {
            _name = "AddData";
            _humaneName = "Добавить в базу...";
            _descr = "Добавить внешние данные";
            _action = Actions.ToolBarButton;

            _handler = new EventHandler(onClick);

            _image = DefImages.ImageAdd;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            SunEnv env = (SunEnv)ui.GetEnviroment();

            try
            {
                ImportToBase(env, true);

                env.UpdateDates();
                ((DataTupleVisualizer)ui).SetDataProvider(new ListMultiDataProvider(env.GetAllData(env.DefProvider)));

            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "В ходе добавления данных произошла ошибка");
            }
        }


        const string azotStream = "azot";
        const string mainStream = "main";

        const string importSkalaProvider = "SunEnv_ImportSkalaProvider";
        const string importAzotProvider = "SunEnv_ImportAzotProvider";
        const string importSkalaStreamParamName = "SunEnv_ImportSkalaStream";
        const string importAzotStreamParamName = "SunEnv_ImportAzotStream";

        const string dbProvider = "SunEnv_DataStorageProvider";

        const string zagrConverterParamName = "SunEnv_AlgoZagrConvert";

        const string kgoDetectAlgo = "kgoDetectMaxPos";


        static public IMultiDataTuple GetSkalaData(BasicEnv env)
        {
            IDataResource rcSkala = env.CreateData(importSkalaProvider);

            ////////////////////////////////////////////
            IMultiDataTuple tup = rcSkala.GetMultiProvider().GetData(
                rcSkala.GetMultiProvider().GetDates()[0],
                env.GetGlobalParam(importSkalaStreamParamName));

            env.CloseData(importSkalaProvider);

            return tup;
        }

        static public void ImportToBase(GuiEnv env, bool confirm)
        {
            IDataResource dataResource = env.OpenData(dbProvider);

            IMultiDataTuple idataSkala = GetSkalaData(env);

            IDataResource rcAzot = env.CreateData(importAzotProvider);

            ////////////////////////////////////////////
            IMultiDataTuple dataAzot = rcAzot.GetMultiProvider().GetData(
                rcAzot.GetMultiProvider().GetDates()[0],
                env.GetGlobalParam(importAzotStreamParamName));
            env.CloseData(importAzotProvider);


            

            DataParamTable t;
            Double badness;
            DataCartogram dc = ParseAndCompactKgoI(env, idataSkala, dataAzot, out t, out badness);

            DataTuple azotComact = new DataTuple(azotStream, idataSkala.GetTimeDate(), dc);
            DataTuple dataSkala = new DataTuple(idataSkala[0], mainStream);

            if (confirm)
            {
                DataTuple tupelsS1 = new DataTuple(dataSkala, "Скала");
                DataTuple tupelsS2 = new DataTuple(azotComact, "Активность компактно");

                ListMultiDataProvider l = new ListMultiDataProvider();
                l.PushData(dataAzot);
                l.PushData(tupelsS1);
                l.PushData(tupelsS2);
                if (t != null)
                {
                    l.PushData(new DataTuple("Подробности разбора", azotComact.GetTimeDate(), t));
                }

                ShowAddition sd = new ShowAddition(env, l,
                    String.Format("Данные по азотной активности за {0} и срезы за {1}",
                        dataAzot[0].GetTimeDate(), dataSkala.GetTimeDate()), badness);
                if (sd.ShowDialog() != DialogResult.OK)
                    return;

                if (l.GetDates("zrk").Length > 0)
                {
                    // ХАК! Объединение с информацией по зрк
                    DateTime dt = l.GetDates("zrk")[0];
                    IDataTuple tn = l.GetData(dt, "zrk")[0];
                    ITupleItem zrk = tn[0];

                    zrk = zrk.Rename(new TupleMetaData(zrk.Name, zrk.HumaneName, dataSkala.GetTimeDate(), mainStream));
                    tn = new DataTuple(mainStream, dataSkala.GetTimeDate(), zrk);

                    // Объединение с dataSkala
                    string[] names = new string[dataSkala.ItemsCount + tn.ItemsCount];
                    dataSkala.CopyNamesTo(names);
                    tn.CopyNamesTo(names, dataSkala.ItemsCount);
                    DataTuple combined = DataTuple.Combine(names, dataSkala, tn);

                    dataSkala = new DataTuple(combined, mainStream, dataSkala.GetTimeDate());
                }
            }

            // Store it

            dataResource.GetMultiProvider().PushData(azotComact);
            dataResource.GetMultiProvider().PushData(dataSkala);
        }

        static public IMultiDataTuple ParseKgo(BasicEnv env, IMultiDataTuple dataSkala, IMultiDataTuple dataAzot)
        {
            return env.GetAlgorithm(kgoDetectAlgo).CallIntelli(
                env.GetAlgorithm(env.GetGlobalParam(zagrConverterParamName)).CallIntelli(dataSkala), dataAzot);
        }

        static public DataCartogram ParseAndCompactKgo(BasicEnv env, TupleMetaData info, IMultiDataTuple dataSkala, IMultiDataTuple dataAzot, out DataParamTable t, out Double overall)
        {
            IMultiDataTuple akgoAnswer = ParseKgo(env, dataSkala, dataAzot);
            overall = 0;
            DataParamTableItem[] items = new DataParamTableItem[akgoAnswer.Count * 2];
            for (int i = 0; i < akgoAnswer.Count; i++)
            {
                DataParamTable tbl = (DataParamTable)akgoAnswer[i]["pvk_errorinfo"];
                AnyValue v = tbl["badness"];
                AnyValue r = tbl["report"];
                items[2 * i] = new DataParamTableItem(String.Format("banessFiber{0}", tbl["fiberNum"]), v);
                items[2 * i + 1] = new DataParamTableItem(String.Format("reportFiber{0}", tbl["fiberNum"]), r);
                overall += v.ToDouble(null);
            }
            t = new DataParamTable(new TupleMetaData("prp_parseinfo", "Подробности разбора", DateTime.Now, TupleMetaData.StreamAuto), items);
            //Сделать проверку на праввильность рабора азотной активность
            return CompactIntoPvk(env, info, "pvk_maxes", akgoAnswer);
        }

        static public DataCartogram CompactIntoPvk(BasicEnv env, TupleMetaData info, string item, IMultiDataTuple dataAzot)
        {
            IMultiTupleItem kpd = dataAzot[item];
            DataCartogram coeffDP = DataCartogram.CreateFromParts(info,
                env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, info), kpd);
            return coeffDP;
        }

        static public DataCartogram ParseAndCompactKgoI(BasicEnv env, IMultiDataTuple dataSkala, IMultiDataTuple dataAzot, out DataParamTable t, out Double v)
        {
            v = 0;
            t = null;
            TupleMetaData info = new TupleMetaData("pvk_maxes_cart", "pvk_maxes_cart", DateTime.Now, TupleMetaData.StreamAuto);

            //Check type of record
            foreach (ITupleItem item in dataAzot[0])
            {
                string st = item.GetName();
                if (st == "kgoprp_azot")
                    return ParseAndCompactKgo(env, info, dataSkala, dataAzot, out t, out v);
                else if (st == "pvk_maxes")
                    return CompactIntoPvk(env, info, "pvk_maxes", dataAzot);
            }
            throw new Exception("Unknown azot information");
        }



    }


    #region Extra Forms

    class ShowAddition : Form
    {
        TableLayoutPanel _panel = new TableLayoutPanel();
        TableLayoutPanel _panel2 = new TableLayoutPanel();
        DataTupleVisualizer _dv;
        Button _btOk = new Button();
        Button _btCancel = new Button();
        Label _info = new Label();
        Label _warn = new Label();

        IDataTuple _zrk;

        void UpdateZrk()
        {
            DataTuple t = new DataTuple(_zrk, "zrk");
            _dv.GetMultiDataProvider().PushData(t);
        }

        public ShowAddition(GuiEnv env, IMultiDataProvider prov, string text, double pLevel)
        {
            SuspendLayout();
            Size = new System.Drawing.Size(700, 400);
            Text = "Будут связаны и добавлены данные";

            _warn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _warn.Font = new System.Drawing.Font(_warn.Font.FontFamily.Name, 14);
            _warn.Text = "Внимательно просмотрите добавляемые данные и нажмите \"Добавить\"";
            _warn.Dock = DockStyle.Fill;

            _dv = new DataTupleVisualizer(env);
            _dv.Dock = DockStyle.Fill;
            _dv.SetDataProvider(prov);
            _dv.RegisterAction(new DTVShowAddZrk(this));

            _info.Text = text;
            _info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _info.Dock = DockStyle.Fill;

            _panel.ColumnCount = 1;
            _panel.RowCount = 3;
            _panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            _panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            _panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            _panel2.RowCount = 1;
            _panel2.ColumnCount = 3;
            _panel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _panel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            _panel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            _panel2.Dock = DockStyle.Fill;

            _panel.Controls.Add(_warn, 0, 0);
            _panel.Controls.Add(_dv, 0, 1);
            _panel.Controls.Add(_panel2, 0, 2);
            _panel2.Controls.Add(_info, 0, 0);
            _panel2.Controls.Add(_btOk, 1, 0);
            _panel2.Controls.Add(_btCancel, 2, 0);


            _btOk.Text = "Добавить";
            _btCancel.Text = "Отмена";
            _btOk.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _btCancel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _btOk.DialogResult = DialogResult.OK;
            _btCancel.DialogResult = DialogResult.Cancel;

            _panel.Dock = DockStyle.Fill;

            Controls.Add(_panel);
            CancelButton = _btCancel;
            AcceptButton = _btCancel;
            ResumeLayout();

            double wlevel = Convert.ToDouble(env.GetGlobalParam("SunEnv_ParseWarningLevel"));
            double flevel = Convert.ToDouble(env.GetGlobalParam("SunEnv_ParseFailLevel"));

            if (pLevel >= flevel)
            {
                _panel.RowStyles[0].Height = 60;

                _warn.Text = String.Format("Не удалось распознать нитки: пороговое значение критерия алгоритма {0} при текущем {1}\r\nУбедитесь что были указаны верные данные. Добавление в базу невозможно.", flevel, pLevel);
                _warn.BackColor = System.Drawing.Color.Red;

                _btOk.Enabled = false;
            }
            else if (pLevel > wlevel)
            {
                _panel.RowStyles[0].Height = 60;

                _warn.Text = String.Format("Проверте правильнось разбора нитки: пердупреждающее значение критерия алгоритма {0} при текущем {1}\r\nУбедитесь что были указаны верные данные.", wlevel, pLevel);
                _warn.BackColor = System.Drawing.Color.Yellow;
            }

            _dv.OnStream += new OnStreamHandler(_dv_OnStream);

            foreach (string s in _dv.GetMultiDataProvider().GetStreamNames())
            {
                if (s == "prp")
                    _dv_OnStream(_dv, s);
            }
        }

        void _dv_OnStream(IDataTupleVisualizerUI sender, string stream)
        {
            if ((stream == "Подробности разбора") ||
                (stream == "Активность компактно") ||
                (stream == "zrk"))
            {
                DateTime dt = _dv.GetMultiDataProvider().GetDates(stream)[0];
                sender.SetActiveTupleItem(_dv.GetMultiDataProvider().GetData(dt, stream)[0][0], stream);
            }
            else if (stream == "prp")
            {
                DateTime dt = _dv.GetMultiDataProvider().GetDates(stream)[0];
                sender.SetActiveTupleItems(
                    _dv.GetMultiDataProvider().GetData(dt, stream)["kgoprp_azot"], stream);
            }
        }

        class DTVShowAddZrk : ActionDataTupleVisualizerUI
        {
            ShowAddition _parent;

            public DTVShowAddZrk(ShowAddition parent)
            {
                _name = "AddZrk";
                _humaneName = "Подсоединить файл ЗРК...";
                _descr = "Добавить к скальным срезам данные по ЗРК";
                _action = Actions.ToolBarButton;

                _handler = new EventHandler(onClick);

                _image = DefImages.ImageAdd;

                _parent = parent;
            }

            public void onClick(object sender, EventArgs e)
            {
                ADTVEventArgs args = (ADTVEventArgs)e;
                IDataTupleVisualizerUI ui = args._ui;
                SunEnv env = (SunEnv)ui.GetEnviroment();

                _parent._zrk = null;

                try
                {
                    using (IDataResource src = env.CreateData("OvlInfoProvider"))
                    {
                        _parent._zrk = src.GetConstData()[0];
                        _parent.UpdateZrk();
                    }

                }
                catch (ActionCanceledException)
                {

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "В ходе добавления данных произошла ошибка");
                }
            }
        }
    }

    #endregion
}
