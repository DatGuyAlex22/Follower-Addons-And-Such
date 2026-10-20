local TrailColorController = {}

TrailColorController.name = "FollowerAddons/TrailColorController"

TrailColorController.depth = -1000000
TrailColorController.texture = "objects/TrailColorController/icon"

TrailColorController.placements = {
	{
		name = "normal",
		data = {
			entities = "Strawberry",
			color = "FFFFFF",
			global = true
		}
	}
}

TrailColorController.fieldInformation = {
	color = {
		fieldType = "color"
	}
}

return TrailColorController