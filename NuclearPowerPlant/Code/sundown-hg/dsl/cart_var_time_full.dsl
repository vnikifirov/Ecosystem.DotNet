#plugin-data-toolbar
#Строит картограмму дисперсий рассогласований по параметру
#Карт дисп

c = Cartogram
mm as (double) = array(double, c.AllCoords.Length)

pos = -1
dates as (Timestamp, 1)
a as Coords
log c

for a in c.AllCoords:
	  z = DataCartogramVisualizerPlugins.CollectDataFromCoord(Item, Tuples, a, pos, dates)
	  p = MathOp.Var(z)
	  mm[c.AllCoords[a]] = p

cc = DataCartogram(TupleMetaData("r-disp-m", "Дисперсия рассогласований по 3методика", DateTime.Now, TupleMetaData.StreamAuto), ScaleIndex.Default, c.AllCoords, mm)

display cc