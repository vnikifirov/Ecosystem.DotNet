#plugin-data-toolbar
#Строит картограмму дисперсий рассогласований по 3 методикам
#Карт расс по 3мет

import file from "utils.boo"

x = FuelCoords(Tuples, Enviroment)
mm as (double) = array(double, x.Length)

pos = -1
dates as (Timestamp, 1)

for a as Coords in x:
	  z = DCVPluginTripleFlow.sCollectData(Item, Tuples, a, pos, dates)
	  p = MathOp.Var(z)
	  mm[x[a]] = p

cc = DataCartogram(TupleMetaData("r-disp-m", "Дисперсия рассогласований по 3методика", DateTime.Now, TupleMetaData.StreamAuto), ScaleIndex.Default, x, mm)

display cc