q=0
def MkCoeff(d) as DataTuple:
	mk1 = (Enviroment as object).ComputeAdaptationCoefficient(d) as IMultiDataTuple
	k1 = (Enviroment as object).CombineCoeffsIntoCartogram(mk1, d)
	return k1 as DataTuple	

def MkIterAvg(i as int, w as DataCartogram, next as DataCartogram) as DataCartogram:
	if (i == 1):
		return next
	
	return (w * (i - 1) + next) / i

kavg_bet1 as DataCartogram
kavg_kpd as DataCartogram
kphi as DataCartogram

f1 as IMultiDataTuple
f2 as IMultiDataTuple

alpha = 0.2

l = ListMultiDataProvider()
c = (@c6135, @c4233)
k = 0
for i in Tuples:
	print k
	d = i as IDataTuple
	def mod(s as double):
		return s - k * 0.3
	
	if (k == 0):
		f1 = d; f2 = d
	else:
		f2 = d
	
	k++

	korig = MkCoeff(d)	
	dmod = mod_tup(d, "flow", mod, *c)
	kmod = MkCoeff(dmod)
	
	kbet1 = kmod["bet1_cart"] as DataCartogram
	kkpd = kmod["kpd_cart"] as DataCartogram
	
	kavg_bet1 = MkIterAvg(k, kavg_bet1, kbet1).Rename("bet1_avg")
	kavg_kpd = MkIterAvg(k, kavg_kpd, kkpd).Rename("kpd_avg")

	phi = ((kbet1 / kavg_bet1 - 1) * (kkpd / kavg_kpd - 1))//.Rename("phi")

	allCoords = (Enviroment as object).GetValidCoordsZrk(f2, f1) as (Coords)
	conv = CoordsConverter(TupleMetaData("recover", "recover", DateTime.Now, TupleMetaData.StreamAuto), CoordsConverter.SpecialFlag.Named, allCoords)

	if k > 1:
		phi = phi.ConvertToIndexedType(
                TupleMetaData("phi_cart", "PHI", phi.Date, TupleMetaData.StreamAuto), conv, phi.ElementType);
		kphi = (kphi * (1-alpha) + phi * alpha).ConvertToIndexedType(
                TupleMetaData("phi_cart_exp", "PHI_exp", phi.Date, TupleMetaData.StreamAuto), conv, phi.ElementType);
	else:
		kphi = phi.Rename(
                TupleMetaData("phi_cart_exp", "PHI_exp", phi.Date, TupleMetaData.StreamAuto));

	l.PushData( DataTuple.combine(dmod, kmod, korig.ReNameItems("org"), kavg_bet1, kavg_kpd, phi, kphi))

window l