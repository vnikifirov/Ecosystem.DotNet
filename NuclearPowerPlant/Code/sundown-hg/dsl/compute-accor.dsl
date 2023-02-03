a as (double)
d = Item as DataArray
d.Normalize().ToArray(a)

accor = MathOp.Accor(a, a.Length)

display accor
