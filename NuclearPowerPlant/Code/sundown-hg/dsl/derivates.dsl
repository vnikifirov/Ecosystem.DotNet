# skip
# library workaround
nothing = null

def CalculateDerivates(data as IDataTuple, pvkInfo, l):
	a = Enviroment.GetAlgorithm("calculateNitroModel")
	m = Enviroment.GetAlgorithm("calculateMiddlePower")
	decompact = Enviroment.GetAlgorithm("kgoDeCompactMax")
	compact = Enviroment.GetAlgorithm("kgoCompactMax")
	
	#ret = a.CallMultiIntelli(Data, m.CallIntelli(Data), l)
	#nitro = DuckTuple.GetMultiItem(ret, "nitro")
	
	akgo = data["pvk_maxes_cart"]
	rbmk_params = data["rbmk_params"]
	power = data["power"]as DataCartogram
	flow = data["flow"] as DataCartogram
	
	dpower = (power * 1.001).Rename("power") as DataCartogram;
	dflow = (flow * 1.001).Rename("flow") as DataCartogram;
	
	nitro = DuckTuple.GetMultiItem(a.CallMultiIntelli(data, m.CallIntelli(data), l), "nitro")
	dn_nitro = DuckTuple.GetMultiItem(a.CallMultiIntelli(dpower, flow, rbmk_params, m.CallIntelli(dpower), l), "nitro")
	dg_nitro = DuckTuple.GetMultiItem(a.CallMultiIntelli(power, dflow, rbmk_params, m.CallIntelli(power), l), "nitro")
	
	akgo_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(akgo), "pvk_maxes")
	akgo_info = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(akgo), "kgoprp_info")
	#display dp_nitro
	
	p = array(DataParamTable, 16)
	for i in range(0,16):
		temp = nitro[akgo_info[i]["fiberNum"]].Sum() / akgo_nitro[i].Sum()
		p[i] = DataParamTable("kgoprp_info", "Адаптационные коэффициенты активности", data.GetTimeDate(), "kappa", temp, "fiberNum", i)
		log "Нитка ${i} коэффициент ${temp}"
	
	nitro_cart = compact.CallIntelli(TupleMaps("nitro as pvk_maxes"), nitro, p, pvkInfo.PvkArray)["pvk_maxes_cart"] as DataCartogram
	dn_nitro_cart= compact.CallIntelli(TupleMaps("nitro as pvk_maxes"), dn_nitro, p, pvkInfo.PvkArray)["pvk_maxes_cart"] as DataCartogram
	dg_nitro_cart= compact.CallIntelli(TupleMaps("nitro as pvk_maxes"), dg_nitro, p, pvkInfo.PvkArray)["pvk_maxes_cart"] as DataCartogram
	
	div_n = (dn_nitro_cart - nitro_cart) / (dpower - power + 0.000000000001)
	div_g = (dg_nitro_cart - nitro_cart) / (dflow - flow + 0.000000000001)
	return (DataTuple(data.GetStreamName(), data.GetTimeDate(), div_n.Rename("dnitro_dn"), div_g.Rename("dnitro_dg")), p)


def RecoveryDerivates(data as IDuckTuple, odata as IDuckTuple, pvkInfo, l, derivates as IDuckTuple, params as (DataParamTable)):
	decompact = Enviroment.GetAlgorithm("kgoDeCompactMax")
	compact = Enviroment.GetAlgorithm("kgoCompactMax")
	
	#ret = a.CallMultiIntelli(Data, m.CallIntelli(Data), l)
	#nitro = DuckTuple.GetMultiItem(ret, "nitro")
	
	akgo = data["pvk_maxes_cart"]
	power = data["power"].ToDouble()
	flow = data["flow"]

	oakgo = odata["pvk_maxes_cart"]
	opower = odata["power"].ToDouble()
	oflow = odata["flow"]
	
	div_n = derivates["dnitro_dn"]
	div_g = derivates["dnitro_dg"]
	
	akgo_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(akgo), "pvk_maxes")	
	power_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(power.Rename("pvk_maxes_cart")), "pvk_maxes")	
	flow_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(flow.Rename("pvk_maxes_cart")), "pvk_maxes")	

	oakgo_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(oakgo), "pvk_maxes")	
	opower_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(opower.Rename("pvk_maxes_cart")), "pvk_maxes")	
	oflow_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(oflow.Rename("pvk_maxes_cart")), "pvk_maxes")	
	
	div_n_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(div_n.Rename("pvk_maxes_cart")), "pvk_maxes")	
	div_g_nitro = DuckTuple.GetMultiItem(decompact.CallMultiIntelli(div_g.Rename("pvk_maxes_cart")), "pvk_maxes")	

	flow_nitro_div = array(DataArray, 16)
	
	for i in range(0,16):
		flow_nitro_div[i] = (oflow_nitro[i] + ((akgo_nitro[i] - oakgo_nitro[i]) * params[i]["kappa"] - div_n_nitro[i] * (power_nitro[i] - opower_nitro[i])) / (div_g_nitro[i])).Normalize().Rename("pvk_maxes")
	
	flow_nitro_cart = compact.CallIntelli( flow_nitro_div, params, pvkInfo.PvkArray)["pvk_maxes_cart"] as DataCartogram
	
	return flow_nitro_cart.Rename("flow_nitro")


a = 0
odata = null
cdata = null
for t in Tuples:
	odata = t if (a == 0)
	cdata = t if (a == 1)
	a++

log odata.GetTimeDate()
log cdata.GetTimeDate()
	
ll = Enviroment.CreateData(Enviroment.GetGlobalParam("SunEnv_PvkLengthProvider")).CreateContSource("pvklen").Receive() as IDataTuple
ppvkInfo = (odata as DataTuple).GetPvkInfo()
ll.SetPvkInfo(ppvkInfo)

ret = CalculateDerivates(odata, ppvkInfo, ll)

flow_ret = RecoveryDerivates(cdata, odata, ppvkInfo, ll, ret[0], ret[1])

#display ret[0]["dnitro_dn"]
#display flow_ret

dt = DataTuple("[default]", System.DateTime.Now, flow_ret, ret[0]["dnitro_dn"], ret[0]["dnitro_dg"])
MultiData.PushData(dt)

