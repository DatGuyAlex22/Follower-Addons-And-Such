local CustomRegisterAreaCompleteTrigger = {}

CustomRegisterAreaCompleteTrigger.name = "FollowerAddons/CustomRegisterAreaCompleteTrigger"

CustomRegisterAreaCompleteTrigger.placements = {
	{
		name = "collectStrawberries",
		data = {
			collectStrawberries = true,
			registerInSave = false,
			onlyOnce = true
		}
	},
	{
		name = "registerInSave",
		data = {
			collectStrawberries = false,
			registerInSave = true,
			onlyOnce = true
		}
	}
}

return CustomRegisterAreaCompleteTrigger