import Algorithms

g_min = 5.0
g_max = 50.0
w_min = 0.1
w_max = 3.1

step = 300

t0 = 270.0
p0 = 70.0
lpvk = 25.0

a = matrix(double, step, step)

dp as double
gm = array(double, 120)
om = array(double, 120)

aa = 0.0

for g in range(step):
	print g
	for w in range(step):
		n = w_min + (w_max - w_min) * w / step
		EvaluteAzot.azgone(t0, p0, lpvk, dp, g_min + (g_max - g_min)*g / step, n, n, aa, gm, om)
		a[g,w] = aa



print "FINISH"

#display a

d = DataGrid()
d.Columns.Add("G / W", double)
for w in range(step):
	n = w_min + (w_max - w_min) * w / step
	d.Columns.Add(n.ToString() , double)

d.HeadColumns = 1

for g in range(step):
	ss = array(object, step+1)
	gg = g_min + (g_max - g_min)*g / step
	ss[0] = gg

	for w in range(step):
		ss[w+1] = a[g,w]
	
	d.Rows.Add(DataGrid.DataRow(ss))

TupleVis.SetDataGrid(d, null, null, "default")
