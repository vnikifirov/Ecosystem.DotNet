# Вспомогательные утилиты

def FuelCoords(tuples, env as BasicEnv) as CoordsConverter:  
	tup as IMultiDataTuple	for tup in tuples:		break		if tup == null:  		return null	zagrConvToInt = env.GetAlgorithm(env.GetGlobalParam("SunEnv_AlgoZagrConvert"))	cart = zagrConvToInt.CallIntelli(tup)[0]["zagr"] as DataCartogram	return FuelCoords(cart)		  def FuelCoords(cart as DataCartogram) as CoordsConverter:  	lst = List[of Coords]()	for c as Coords in cart.AllCoords:		if (cart.GetAnyValue(c, 0).ToInt32(null) == ChannelType.TVS):  
			lst.Add(c)				crds as (Coords) = array(Coords, lst.Count)	lst.CopyTo(crds, 0)		return CoordsConverter(TupleMetaData("fuelcoords", "fuelcoor", DateTime.Now, TupleMetaData.StreamAuto), CoordsConverter.SpecialFlag.Named, crds)