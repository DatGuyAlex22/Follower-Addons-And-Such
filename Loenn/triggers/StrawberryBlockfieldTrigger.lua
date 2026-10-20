local StrawberryBlockfieldTrigger = {}

StrawberryBlockfieldTrigger.name = "FollowerAddons/StrawberryBlockfieldTrigger"
StrawberryBlockfieldTrigger.triggerText = "Strawberry Blockfield Trigger"
    
StrawberryBlockfieldTrigger.placements = {
	{
		name = "blockfield_safe",
		data = {
			keepOnDeath = true,
			triggerOn = true,
			resetOnLeave = false
		}
	},
	{
		name = "blockfield_unsafe",
		data = {
		keepOnDeath = false,
		triggerOn = true,
		resetOnLeave = false
		}
	}
}

return StrawberryBlockfieldTrigger