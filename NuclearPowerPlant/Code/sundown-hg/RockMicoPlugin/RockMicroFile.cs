using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.IO;

using corelib;

namespace RockMicoPlugin
{
#if !DOTNET_V11
    using DataArrayInt = DataArray<int>;
    using DataArrayShort = DataArray<short>;
    using DataArrayByte = DataArray<byte>;
    using DataArrayFloat = DataArray<float>;
    using DataArrayDouble = DataArray<double>;
    using DataArraySensored = DataArray<Sensored>;
    using DataArrayCoords = DataArray<Coords>;
    using DataArrayFiberCoords = DataArray<FiberCoords>;
    using DataArrayMultiIntFloat = DataArray<MultiIntFloat>;

    using DataCartogramIndexedInt = DataCartogramIndexed<int>;
    using DataCartogramIndexedShort = DataCartogramIndexed<short>;
    using DataCartogramIndexedByte = DataCartogramIndexed<byte>;
    using DataCartogramIndexedFloat = DataCartogramIndexed<float>;
    using DataCartogramIndexedDouble = DataCartogramIndexed<double>;
    using DataCartogramIndexedSensored = DataCartogramIndexed<Sensored>;

    using DataCartogramNativeInt = DataCartogramNative<int>;
    using DataCartogramNativeShort = DataCartogramNative<short>;
    using DataCartogramNativeByte = DataCartogramNative<byte>;
    using DataCartogramNativeFloat = DataCartogramNative<float>;
    using DataCartogramNativeDouble = DataCartogramNative<double>;
    using DataCartogramNativeSensored = DataCartogramNative<Sensored>;
#endif

    public class RockMicroFile
    {
        protected readonly CoordsConverter _liner1884;

        RockMicroFile(CoordsConverter liner1884)
        {
            _liner1884 = liner1884;
        }

        void InitZagr(string filename, String name, String humanhelp, out DataCartogramNativeByte c)
        {
            byte[] tmp = new byte[2304];
            using (FileStream fs = File.OpenRead(_path + filename))
                fs.Read(tmp, 0, 2304);
            
            StreamDeserializer d = new StreamDeserializer(tmp, 0);
            DataArrayByte array = new DataArrayByte(new TupleMetaData(name, humanhelp, _dateTime, TupleMetaData.StreamAuto),
                d, 48, 48, 0);
            c = new DataCartogramNativeByte(array.Info,ScaleIndex.Default, array);
        }

        #region DEPRECATED MUST BE REWRITTEN

        private void SetDateFromFile(bool readDateFromPath)
        {
            if (!readDateFromPath)
            {
                //Skip it, use timestamp from file
                _dateTime = File.GetLastWriteTime(_path + "TPK.IZM");
            }
            else
            {
                char[] chars = { '/' };
                String[] pt = _path.Split(chars);
                int parts = pt.Length;

                String timePart = (parts > 2) ? pt[parts - 1] : null;
                String datePart = (parts > 2) ? pt[parts - 2] : null;

                //TODO
                //FIXME
            }
        }

        private void ReadDateTime(bool readDateFromPath)
        {
            if (File.Exists(_path + "DATEEPOL.PRZ"))
            {
                try
                {
                    int year, mounth, day;
                    int hour, minute, sec;
                    byte[] dataArray = new byte[8];
                    FileStream fs = File.OpenRead(_path + "DATEEPOL.PRZ");
                    fs.Read(dataArray, 0, 8);
                    year = (dataArray[4] - '0') * 10 + (dataArray[5] - '0');
                    year += ((dataArray[6] - '0') * 10 + (dataArray[7] - '0')) * 100;
                    mounth = (dataArray[2] - '0') * 10 + (dataArray[3] - '0');
                    day = (dataArray[0] - '0') * 10 + (dataArray[1] - '0');
                    fs.Close();

                    fs = File.OpenRead(_path + "WREMEPOL.PRZ");
                    fs.Read(dataArray, 0, 8);
                    hour = (dataArray[0] - '0') * 10 + (dataArray[1] - '0');
                    minute = (dataArray[2] - '0') * 10 + (dataArray[3] - '0');
                    sec = (dataArray[4] - '0') * 10 + (dataArray[5] - '0');
                    fs.Close();

                    if (year < 30)
                    {
                        year += 2000;
                    }
                    _dateTime = new DateTime(year, mounth, day, hour, minute, sec);

                }
                catch (FileNotFoundException)
                {
                    SetDateFromFile(readDateFromPath);
                }
            }
            else
                SetDateFromFile(readDateFromPath);
        }

        private VXentry GetVX(String vxstr)
        {
            VXentry[] vxe;
            switch (vxstr.Length)
            {
                case 3: vxe = vxList; break;
                case 5: vxe = vxPersList; break;
                case 7: vxe = vxUnconvList; break;
                default: throw new Exception("Incorrect vxstr");
            }
            int idx = GetComplVX(vxe, vxstr, 0);
            if (idx < 0)
                throw new Exception("Can't find VX in array");

            return vxe[idx];
        }

        private int GetComplVX(VXentry[] vxe, String vxstr, int start)
        {
            int len = vxstr.Length;
            char[] warr = vxstr.ToCharArray(0, len);

            for (int i = start; i < vxe.Length; i++)
            {
                if ((vxe[i].vx0 == warr[0]) &&
                   (vxe[i].vx1 == warr[1]) &&
                   (vxe[i].vx2 == warr[2]) &&
                   ((len < 4) || (vxe[i].vx3 == warr[3])) &&
                   ((len < 5) || (vxe[i].vx4 == warr[4])) &&
                   ((len < 6) || (vxe[i].vx5 == warr[5])) &&
                   ((len < 7) || (vxe[i].vx6 == warr[6])))
                {
                    return i;
                }
            }
            return -1;
        }

        private ScaleIndex GetScaleIndex(VXentry vxe)
        {
            int range = 0;
            switch (vxe.format)
            {
                case 0: range = 511; break;
                case 1: range = 127; break;
                case 2: range = 1023; break;
                default: throw new Exception("Incorrect range format");
            }

            double dMas = 1.0;
            double dMin = 0.0;
            if (vxe.nmas < masList.Length)
            {
                MASentry mase = masList[vxe.nmas];
                dMas = ((double)mase.mas_max - (double)mase.mas_min) / range / (double)Math.Pow(10, mase.mas);
                dMin = (double)mase.mas_min;
            }
            return new ScaleIndex(dMin, dMas);
        }

        int ReadFlow(String vx, String file, String name, String humanhelp, out DataCartogramNativeDouble c)
        {
            VXentry vxe = GetVX(vx);

            byte[] fullData = GetFileData(file);
            if ((fullData == null) || (fullData.Length == 0) || (fullData.Length < vxe.offset_p))
                throw new ArgumentException(String.Format("Incorrect file {1}: {0}", _path, file));

            if (vxe.tn != 2304)
                throw new ArgumentException(String.Format("Incorrect flow"));

            StreamDeserializer d = new StreamDeserializer(fullData, vxe.offset_p);
            Sensored[,] tmp = (Sensored[,])d.GetArray(typeof(Sensored), 48, 48, 0);

            double[,] a = new double[48, 48];
            foreach (Coords crd in _liner1884)
            {
                Sensored val = ((Sensored[,])tmp)[crd.Y, crd.X];
                double res;
                if (_zagr.GetItem(crd,0) < 0x21)
                    res = Math.Round(val.Value * 500.0 / 511.0) / 10.0;
                else
                    res = Math.Round(val.Value * 80.0 / 511.0) / 10.0;
                a[crd.Y, crd.X] = res;
            }

            c = new DataCartogramNativeDouble(new TupleMetaData(name, humanhelp, _dateTime, TupleMetaData.StreamAuto), ScaleIndex.Default, a);
            return 0;
        }

        Coords[] GetIndexedCoords(VXentry vxe)
        {
            byte[] bdData = GetFileData("BDPUB.INI");
            if ((bdData == null) || (bdData.Length == 0))
                throw new ArgumentException(String.Format("Incorrect file BDPUB.INI: {0}", _path));

            int nChn = vxe.ntk;
            int nChnTable = vxe.offset_k;

            StreamDeserializer coords = new StreamDeserializer(bdData, nChnTable);
            short[] tmp = (short[])coords.GetArray(typeof(short), nChn, 0, 0);
            Coords[] crds = new Coords[nChn];
            for (int i = 0; i < nChn; i++)
                if (tmp[i] != -1)
                    crds[i] = new Coords((byte)((tmp[i] & 0xff) - 8), (byte)((tmp[i] >> 8) - 8));
                else
                    crds[i] = Coords.incorrect;

            return crds;
        }

        int ReadCartogram(String vx, String file, String name, String humanhelp, out DataCartogram c)
        {
            VXentry vxe = GetVX(vx);
            ScaleIndex sc = GetScaleIndex(vxe);

            byte[] fullData = GetFileData(file);
            if ((fullData == null) || (fullData.Length == 0) || (fullData.Length < vxe.offset_p))
                throw new ArgumentException(String.Format("Incorrect file {1}: {0}", _path, file));

            DataArraySensored array;
            if (vxe.tn == 2304)
            {
                StreamDeserializer d = new StreamDeserializer(fullData, vxe.offset_p);

                array = new DataArraySensored(
                    new TupleMetaData(name, humanhelp, _dateTime, TupleMetaData.StreamAuto), d, 48, 48, 0);

                c = new DataCartogramNativeSensored(array.Info, sc, array);
            }
            else
            {
                Coords[] crds = GetIndexedCoords(vxe);
                CoordsConverter crd = new CoordsConverter(
                    new TupleMetaData(vx, vx, _dateTime, TupleMetaData.StreamAuto), CoordsConverter.SpecialFlag.Named, crds);

                StreamDeserializer d = new StreamDeserializer(fullData, vxe.offset_p);
                int layers = vxe.tn / vxe.ntk;

                if (layers == 1)
                {
                    array = new DataArraySensored(new TupleMetaData(name, humanhelp, _dateTime, TupleMetaData.StreamAuto),
                        d, vxe.ntk, 0, 0);
                }
                else 
                {
                    array = new DataArraySensored(new TupleMetaData(name, humanhelp, _dateTime, TupleMetaData.StreamAuto),
                        d, vxe.ntk, layers, 0);
                }

                c = new DataCartogramIndexedSensored(array.Info, sc, crd, array);
            }
            return 0;
        }

        int ReadSingleDouble(String vx, String file, out double storage)
        {
            VXentry vxe = GetVX(vx);
            ScaleIndex sc = GetScaleIndex(vxe);

            byte[] fullData = GetFileData(file);
            Sensored sens = new Sensored((short)(fullData[vxe.offset_p] | fullData[vxe.offset_p + 1] << 8));
            storage = sc.Scale(sens);

            return 0;
        }
        #endregion

        private void InitVXlist()
        {
            byte[] tmp = new byte[44];

            FileStream fs = File.OpenRead(_path + "KATKR.INI");
            int items = (int)fs.Length / 44;
            if (((fs.Length % 44) != 0) || (fs.Length == 0))
                throw new Exception("Error reading VX");

            vxList = new VXentry[items];

            for (int i = 0; i < items; i++)
            {
                fs.Read(tmp, 0, 44);

                vxList[i].vx0 = (char)tmp[0];
                vxList[i].vx1 = (char)tmp[1];
                vxList[i].vx2 = (char)tmp[2];

                vxList[i].tn = tmp[3] + (tmp[4] << 8);
                vxList[i].nmas = tmp[5];
                vxList[i].nraz = tmp[6];
                vxList[i].tk = tmp[7];
                vxList[i].offset_p = tmp[8] + (tmp[9] << 8);//  + (tmp[10] << 16) +  (tmp[11] << 24);
                vxList[i].offset_u = tmp[12] + (tmp[13] << 8);//  + (tmp[14] << 16) +  (tmp[15] << 24);
                vxList[i].offset_d = tmp[16] + (tmp[17] << 8);//  + (tmp[18] << 16) +  (tmp[19] << 24);
                vxList[i].n_alg = tmp[20];
                vxList[i].ntk = tmp[21] + (tmp[22] << 8);
                vxList[i].offset_k = tmp[23] + (tmp[24] << 8);//  + (tmp[25] << 16) +  (tmp[26] << 24);
                vxList[i].u_alg = tmp[27];
                vxList[i].format = tmp[28];

                vxList[i].vx3 = vxList[i].vx4 = vxList[i].vx5 = vxList[i].vx6 = (char)0;
                vxList[i].tm = -1;
            }

            fs.Close();

            //Init non-standard VX
            fs = File.OpenRead(_path + "KATEXC.INI");
            try
            {
                items = (int)fs.Length / 24;
                if (((fs.Length % 24) != 0) || (fs.Length == 0))
                    throw new Exception("Error reading non-standard VX");

                vxUnconvList = new VXentry[items];

                for (int i = 0; i < items; i++)
                {
                    fs.Read(tmp, 0, 24);

                    vxUnconvList[i].vx0 = (char)tmp[0];
                    vxUnconvList[i].vx1 = (char)tmp[1];
                    vxUnconvList[i].vx2 = (char)tmp[2];
                    vxUnconvList[i].vx3 = (char)tmp[3];
                    vxUnconvList[i].vx4 = (char)tmp[4];
                    vxUnconvList[i].vx5 = (char)tmp[5];
                    vxUnconvList[i].vx6 = (char)tmp[6];

                    vxUnconvList[i].tn = tmp[7] + (tmp[8] << 8);
                    vxUnconvList[i].nmas = tmp[9];
                    vxUnconvList[i].nraz = tmp[10];
                    vxUnconvList[i].tk = tmp[11];
                    vxUnconvList[i].offset_p = tmp[12] + (tmp[13] << 8);//  + (tmp[14] << 16) +  (tmp[15] << 24);
                    vxUnconvList[i].offset_u = tmp[16] + (tmp[17] << 8);//  + (tmp[18] << 16) +  (tmp[19] << 24);
                    vxUnconvList[i].offset_d = tmp[20] + (tmp[21] << 8);//  + (tmp[22] << 16) +  (tmp[23] << 24);

                    //Unsupported data
                    vxUnconvList[i].tm = -1;
                    vxUnconvList[i].n_alg = -1;
                    vxUnconvList[i].ntk = -1;
                    vxUnconvList[i].offset_k = -1;
                    vxUnconvList[i].u_alg = -1;
                    vxUnconvList[i].format = 0;
                }
            }
            finally
            {
                fs.Close();
            }

            //Init personal VX
            fs = File.OpenRead(_path + "KATINR.INI");
            try
            {
                items = (int)fs.Length / 22;
                if (((fs.Length % 22) != 0) || (fs.Length == 0))
                    throw new Exception("Error reading non-standard VX");

                vxPersList = new VXentry[items];

                for (int i = 0; i < items; i++)
                {
                    fs.Read(tmp, 0, 22);

                    vxPersList[i].vx0 = (char)tmp[0];
                    vxPersList[i].vx1 = (char)tmp[1];
                    vxPersList[i].vx2 = (char)tmp[2];
                    vxPersList[i].vx3 = (char)tmp[3];
                    vxPersList[i].vx4 = (char)tmp[4];
                    vxPersList[i].tm = tmp[5];
                    vxPersList[i].tn = tmp[6];
                    vxPersList[i].nmas = tmp[7];
                    vxPersList[i].nraz = tmp[8];
                    vxPersList[i].tk = tmp[9];
                    vxPersList[i].offset_p = tmp[10] + (tmp[11] << 8);//  + (tmp[12] << 16) +  (tmp[13] << 24);
                    vxPersList[i].offset_u = tmp[14] + (tmp[15] << 8);//  + (tmp[16] << 16) +  (tmp[17] << 24);
                    vxPersList[i].offset_d = tmp[18] + (tmp[19] << 8);//  + (tmp[20] << 16) +  (tmp[21] << 24);

                    //Unsupported data
                    vxPersList[i].vx5 = vxPersList[i].vx6 = (char)0;
                    vxPersList[i].n_alg = -1;
                    vxPersList[i].ntk = -1;
                    vxPersList[i].offset_k = -1;
                    vxPersList[i].u_alg = -1;
                    vxPersList[i].format = 0;
                }
            }
            finally
            {
                fs.Close();
            }
        }

        private void InitMASlist()
        {
            byte[] tmp = new byte[5];
            FileStream fs = File.OpenRead(_path + "TSCALE.INI");
            int items = (int)fs.Length / 5;
            if (((fs.Length % 5) != 0) || (fs.Length == 0))
                throw new Exception("Error reading MASlist");

            masList = new MASentry[items];

            for (int i = 0; i < items; i++)
            {
                fs.Read(tmp, 0, 5);
                masList[i].mas_min = (short)(tmp[0] + (tmp[1] << 8));
                masList[i].mas_max = (short)(tmp[2] + (tmp[3] << 8));
                masList[i].mas = tmp[4];
            }

            fs.Close();
        }

        byte[] GetFileData(string file)
        {
            if (!_cachedFiles.ContainsKey(file))
            {
                FileStream fs = File.OpenRead(_path + file);
                byte[] data = new byte[(int)fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                fs.Close();

                _cachedFiles.Add(file, data);
            }
            return (byte[])_cachedFiles[file];
        }

        void InitRestartOfSkalaINTE(String filename)
        {
            byte[] fullData = GetFileData(filename);

            StreamDeserializer d = new StreamDeserializer(fullData, 0x4720);
            DataArrayDouble array = new DataArrayDouble(new TupleMetaData("energovir", "Энерговырвботка", _dateTime, TupleMetaData.StreamAuto),
                d, 1884, 0, 0);

            _energovir = new DataCartogramIndexedDouble(array.Info, ScaleIndex.Default, _liner1884, array);
        }

        void InitZAPK(String filename)
        {
            byte[] fullData = GetFileData(filename);

            StreamDeserializer d = new StreamDeserializer(fullData, 0);
            DataArrayShort array = new DataArrayShort(new TupleMetaData("zapk", "Запас до кризиса", _dateTime, TupleMetaData.StreamAuto),
                d, 48, 48, 0);

            _zapk = new DataCartogramNativeShort(array.Info, new ScaleIndex(0, 0.01), array);

        }

        void InitRestartOfSkalaNPHR(String filename)
        {
            byte[] fullData = GetFileData(filename);

            StreamDeserializer d = new StreamDeserializer(fullData, 0x0010);
            DataArrayShort array = new DataArrayShort(new TupleMetaData("fizras", "Физрасчет", _dateTime, TupleMetaData.StreamAuto),
                            d, 1884, 0, 0);

            ScaleIndex scale = new ScaleIndex(0, 0.01);
            _fizras = new DataCartogramIndexedShort(array.Info, scale, _liner1884, array);
        }

        void FillParamHashtable(string name, string helpName, out DataParamTable tbl)
        {
            double totalHeatPower;
            double pLeftNK, pLeftBS1, pLeftBS2;
            double pRightNK, pRightBS1, pRightBS2;
            double tLeftVK1, tLeftVK2;
            double tRightVK1, tRightVK2;

            double kmpzTotal, kmpzLeft, kmpzRight;
            double flowSuz, tInSuz, tOutSuz;

            ReadSingleDouble("p0N0000", "TPEUSTE.IZM", out totalHeatPower);
			try { ReadSingleDouble("p1P3411", "TPEUSTE.IZM", out pLeftNK); } 
			catch { pLeftNK = -1; }
			try { ReadSingleDouble("p2P3411", "TPEUSTE.IZM", out pRightNK); }
			catch { pRightNK = -1; }

			try { ReadSingleDouble("p1P2111", "TPEUSTE.IZM", out pLeftBS1); }
			catch { pLeftBS1 = -1; }
			try { ReadSingleDouble("p1P2121", "TPEUSTE.IZM", out pLeftBS2); }
			catch { pLeftBS2 = -1; }
			try { ReadSingleDouble("p2P2111", "TPEUSTE.IZM", out pRightBS1); }
			catch { pRightBS1 = -1; }
			try { ReadSingleDouble("p2P2121", "TPEUSTE.IZM", out pRightBS2); }
			catch { pRightBS2 = -1; }

			try { ReadSingleDouble("p1T3111", "TPEUSTE.IZM", out tLeftVK1); }
			catch { tLeftVK1 = -1; }
			try { ReadSingleDouble("p1T3112", "TPEUSTE.IZM", out tLeftVK2); }
			catch { tLeftVK2 = -1; }
			try { ReadSingleDouble("p1T3111", "TPEUSTE.IZM", out tRightVK1); }
			catch { tRightVK1 = -1; }
			try { ReadSingleDouble("p1T3112", "TPEUSTE.IZM", out tRightVK2);  }
			catch { tRightVK2 = -1; }

            ReadSingleDouble("K0G0000", "TPEUSTE.IZM", out kmpzTotal);
            ReadSingleDouble("K1G0000", "TPEUSTE.IZM", out kmpzLeft);
            ReadSingleDouble("K2G0000", "TPEUSTE.IZM", out kmpzRight);

			try { ReadSingleDouble("A0G3111", "TPEUSTE.IZM", out flowSuz); }
			catch { flowSuz = -1; }
			try { ReadSingleDouble("A0T3311", "TPEUSTE.IZM", out tInSuz); }
			catch { tInSuz = -1; }
			try { ReadSingleDouble("A0T3111", "TPEUSTE.IZM", out tOutSuz); }
			catch { tOutSuz = -1; }

            DataParamTableItem[] hs = {
                new DataParamTableItem("totalHeatPower", totalHeatPower),
                new DataParamTableItem("pLeftNK", pLeftNK),
                new DataParamTableItem("pRightNK", pRightNK),

                new DataParamTableItem("pLeftBS1", pLeftBS1),
                new DataParamTableItem("pLeftBS2", pLeftBS2),
                new DataParamTableItem("pRightBS1", pRightBS1),
                new DataParamTableItem("pRightBS2", pRightBS2),                

                new DataParamTableItem("tLeftVK1", tLeftVK1),
                new DataParamTableItem("tLeftVK2", tLeftVK2),
                new DataParamTableItem("tRightVK1", tRightVK1),
                new DataParamTableItem("tRightVK2", tRightVK2),

                new DataParamTableItem("fKmpzTotal", kmpzTotal),
                new DataParamTableItem("fKmpzLeft", kmpzLeft),
                new DataParamTableItem("fKmpzRight", kmpzRight),

                new DataParamTableItem("flowSuz", flowSuz),
                new DataParamTableItem("tInSuz", tInSuz),
                new DataParamTableItem("tOutSuz", tOutSuz)
                                      };

            tbl = new DataParamTable(new TupleMetaData(name, helpName, _dateTime, TupleMetaData.StreamAuto), hs);
        }

        protected void Init(string filepath, bool readDateFromPath)
        {
            if (filepath[filepath.Length - 1] != '/')
                filepath = filepath + "/";

            _path = filepath;

            ReadDateTime(readDateFromPath);

            InitVXlist();
            InitMASlist();

            try
            {
                //New format
                InitZagr("ZAGR0", "zagr_rockmicro", "Загрузка, СКАЛА-Микро", out _zagr);
            }
            catch (FileNotFoundException)
            {
                //Old format
                InitZagr("ZAGR.PRZ", "zagr_rockmicro", "Загрузка, СКАЛА-Микро", out _zagr);
            }

            ReadCartogram("K0N", "TPK.IZM", "power", "Мощность", out _power);
            ReadFlow("K0G", "TPK.IZM", "flow", "Расход", out _flow);
            ReadCartogram("K0S", "TPK.IZM", "suz", "СУЗ", out _suz);

            InitRestartOfSkalaINTE("INTE0"); //_energovir "energovir", "Энерговырвботка"
            InitRestartOfSkalaNPHR("NPHR0"); //_fizras "fizras", "Физрасчет"

            ReadCartogram("I0N", "TPK.IZM", "dkr_1", "ДКЭР1", out _dkr1); 
            ReadCartogram("J0N", "TPK.IZM", "dkr_2", "ДКЭР2", out _dkr2);

            ReadCartogram("I0K", "TPK.IZM", "dkr_corr_1", "ДКЭР1 скоррекстированный", out _dkr1fixed);
            ReadCartogram("J0K", "TPK.IZM", "dkr_corr_2", "ДКЭР2 скоррекстированный", out _dkr2fixed);

            ReadCartogram("I1N", "TPK.IZM", "dkv_1", "ДКЭВ1", out _dkv1);
            ReadCartogram("J1N", "TPK.IZM", "dkv_2", "ДКЭВ2", out _dkv2);

            FillParamHashtable("rbmk_params", "Параметры систем", out _parameters);

            InitZAPK("ZAPK.PRZ");

        }

        public DataTuple GetDataTuple()
        {
            DataTuple t = new DataTuple(defaultStreamName, _dateTime,
                _zagr, _power, _flow, _suz, _energovir, _fizras, _dkr1, _dkr2, _dkr1fixed, _dkr2fixed, _parameters, _dkv1, _dkv2,
                _zapk);
            return t;
        }

        public RockMicroFile(CoordsConverter liner1884, string filepath, bool readDateFromPath)
            : this (liner1884)
        {
            Init(filepath, readDateFromPath);
        }
        
        public RockMicroFile(CoordsConverter liner1884, string filepath)
            : this (liner1884)
        {
            Init(filepath, false);
        }

        struct VXentry
        {
            public Char vx0, vx1, vx2;
            public int tn;
            public int nmas;
            public int nraz;
            public int tk;
            public int offset_p;
            public int offset_u;
            public int offset_d;
            public int n_alg;     //Only in VX_3
            public int ntk;       //Only in VX_3
            public int offset_k;  //Only in VX_3
            public int u_alg;     //Only in VX_3
            public int format;    //Only in VX_3
            public Char vx3, vx4; //Only in VX_5 & VX_7
            public int tm;        //Only in VX_5
            public Char vx5, vx6; //Only in VX_7

			public override string ToString()
			{
				return String.Format("{0}{1}{2}{3}{4}{5}{6}",
					vx0,vx1,vx2,vx3,vx4,vx5,vx6);
			}

        }

        struct MASentry
        {
            public int mas;
            public int mas_min;
            public int mas_max;
        }

        public const string sourceNames = "zagr_rockmicro,power,flow,suz,energovir,fizras,dkr_1,dkr_2,dkr_corr_1,dkr_corr_2,rbmk_params";
        public const string defaultStreamName = "rock_micro";

        protected DataCartogramNativeByte _zagr;
        protected DataCartogram _power;
        protected DataCartogramNativeDouble _flow;
        protected DataCartogram _suz;

        protected DataCartogram _dkr1;
        protected DataCartogram _dkr2;

        protected DataCartogram _dkr1fixed;
        protected DataCartogram _dkr2fixed;

        protected DataCartogram _dkv1;
        protected DataCartogram _dkv2;

        protected DataCartogram _energovir;
        protected DataCartogram _fizras;

        protected DataParamTable _parameters;

        protected DataCartogramNativeShort _zapk;

#if DOTNET_V11
        private Hashtable _cachedFiles = new Hashtable();
#else
        private Dictionary<string, byte[]> _cachedFiles = new Dictionary<string, byte[]>();
#endif

        private String _path;
        private DateTime _dateTime;

        // VX and scales
        private VXentry[] vxList;
        private VXentry[] vxPersList;
        private VXentry[] vxUnconvList;
        private MASentry[] masList;
    }

    [AttributeDataComponent("Чтение среза состояния СКАЛА-Микро", SourceNames = RockMicroFile.sourceNames, ComponentFileFilter = "Данные базы СКАЛА-МИКРО KATKR.INI|KATKR.INI", ComponentFileNameArgument = "rockMicroSingleDirectoryPath")]
    public class RockMicroSingleProvider : RockMicroFile, ISingleDataProvider
    {
        public static readonly string[] splitedSourceNames = sourceNames.Split(',');
        const string initFile = "KATKR.INI";

        public RockMicroSingleProvider(IEnviromentEx enviromentObject, string rockMicroSingleDirectoryPath)
            : base(enviromentObject.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()),
                   ModifyPath(rockMicroSingleDirectoryPath))
        {
        }

        #region ISingleDataProvider Members

        public IMultiDataTuple GetData()
        {
            return GetDataTuple();
        }

        public void PushData(IMultiDataTuple data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDataProvider Members

        public string[] GetExistNames()
        {
            return splitedSourceNames;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        static string ModifyPath(string path)
        {
            if (path.ToUpper().EndsWith(initFile))
            {
                return path.Substring(0, path.Length - initFile.Length);
            }
            return path;
        }
    }


    [AttributeDataComponent("Чтение множества прописок базы СКАЛА-Микро", MultiTuple = false, SourceNames = RockMicroFile.sourceNames)]
    public class RockMicroMultiProvider : SingleSearchToMultiProvider
    {
        public RockMicroMultiProvider(IEnviromentEx enviromentObject, string rockMicroMultiDirectoryPath)
            : base(enviromentObject, new DataComponents.DataComponent(typeof(RockMicroSingleProvider)), rockMicroMultiDirectoryPath)
        {

        }
    }

    public class RockMicroAlgo
    {
        [AttributeAlgoComponent("Преобразование загрезки")]
        [AttributeRules("zagr is (zagr_rockmicro as double[native]) ;" +
                 " return is (zagr as Cart(native) to zagr_rockmicro )")]
        public static void zagrConvertToInternal(double[,] zagr_rockmicro)
        {
            for (int y = 0; y < 48; y++)
                for (int x = 0; x < 48; x++)
                {
                    double ad = zagr_rockmicro[y, x];
                    ChannelType ch = ChannelType.UNKNOWN;

                    if (ad <= 0x20)
                        ch = ChannelType.TVS;
                    else if ((ad >= 0x50) && (ad <= 0x60))
                        ch = ChannelType.DP;
                    else if (ad == 0x47)
                        ch = ChannelType.WATER;
                    else
                        //throw new Exception("Unknown type");
                        ch = ChannelType.UNKNOWN;

                    zagr_rockmicro[y, x] = (double)ch;
                }
        }
    }
}
