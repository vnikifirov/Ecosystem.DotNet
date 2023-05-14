#plugin-data-toolbar
#Выделяет ячейки с соотношением мощность расход менее 0.04
#W/G < 0.04

SelectedCoords = null
for c in Cartogram.AllCoords:
	if Data["power"][c, 0] / Data["flow"][c, 0] < 0.04:
		select c
