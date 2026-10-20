local EntityOutliner = {}

EntityOutliner.name = "FollowerAddons/EntityOutliner"

EntityOutliner.depth = -1000000
EntityOutliner.texture = "objects/EntityOutliner/icon"

EntityOutliner.placements = {
	{
		name = "normal",
		data = {
			entities = "Strawberry",
			color = "FFFFFF",
			global = true,
			offset = 1
		}
	}
}

EntityOutliner.fieldInformation = {
	color = {
		fieldType = "color"
	}
}

return EntityOutliner