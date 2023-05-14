a as (double)
d = Item as DataArray
d.Normalize().ToArray(a)

accor = MathOp.Accor(a, a.Length)

#display accor

k = Math.Ceiling(0.4 * 115)
qq = array(double, a.Length)

for i in range(a.Length):
	sm = 0.0
	l = 0
	for j in range(-k, k+1):
		if (i+j < 0):
			continue
		elif (i+j >= a.Length):
			continue
		else:
			sm = sm + a[i + j] * (k + 1 - Math.Abs(j))
			l++
	qq[i] = l * sm / (k * k * k );


display qq
