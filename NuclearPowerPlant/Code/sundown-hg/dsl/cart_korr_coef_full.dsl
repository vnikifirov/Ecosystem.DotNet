#plugin-data-toolbar
#Строит картограмму корреляционных моментов коэффициентов dP и az
#Korr(dP,az)

import file from "utils.boo"

cnv = FuelCoords(Tuples, Enviroment)
mm as (double) = array(double, cnv.Length)

for a as Coords in cnv:
	x = DataCartogramVisualizerPlugins.CollectDataFromCoord("bet1_cart", Tuples, a)
	y = DataCartogramVisualizerPlugins.CollectDataFromCoord("kpd_cart", Tuples, a)
	q = Math.Sqrt(MathOp.Var(x) * MathOp.Var(y))
	p = MathOp.Korr(x,y) / q
	if q == 0:
		p = 0
	mm[cnv[a]] = p

cc = DataCartogram(TupleMetaData("korr-az-dp", "Корреляционный моментов коэффициентов dP и az", DateTime.Now, TupleMetaData.StreamAuto), ScaleIndex.Default, cnv, mm)

display cc