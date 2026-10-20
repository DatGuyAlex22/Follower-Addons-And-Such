local drawableSprite = require("structs.drawable_sprite")

local CustomDepthTouchSwitch = {}

CustomDepthTouchSwitch.name = "FollowerAddons/CustomDepthTouchSwitch"

CustomDepthTouchSwitch.placements = {
	{
		name = "normal",
		data = {
			depth = 2000
		}
	}
}

function CustomDepthTouchSwitch.depth(room, entity)
    return entity.depth
end

local containerTexture = "objects/touchswitch/container"
local iconTexture = "objects/touchswitch/icon00"

function CustomDepthTouchSwitch.sprite(room, entity)
    local containerSprite = drawableSprite.fromTexture(containerTexture, entity)
    local iconSprite = drawableSprite.fromTexture(iconTexture, entity)

    return {containerSprite, iconSprite}
end

return CustomDepthTouchSwitch