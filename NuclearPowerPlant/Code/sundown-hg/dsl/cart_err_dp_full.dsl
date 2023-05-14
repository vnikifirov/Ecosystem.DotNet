#plugin-data-toolbar
#Строит картограмму дисперсий рассогласований по Dp
#Карт расс по Dp


c = Cartogram
mm as (double) = array(double, c.AllCoords.Length)

pos = -1
dates as (Timestamp, 1)
a as Coords
log c

for a in c.AllCoords:
	  z = DCVPluginErrFlowDp.sCollectData(Item, Tuples, a, pos, dates)
	  p = MathOp.Var(z)
	  mm[c.AllCoords[a]] = p

cc = DataCartogram(TupleMetaData("r-disp-dp", "Дисперсия рассогласований по Dp", DateTime.Now, TupleMetaData.StreamAuto), ScaleIndex.Default, c.AllCoords, mm)

display cc