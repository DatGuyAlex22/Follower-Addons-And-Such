# Updated to v1.1.0
    Removed the rogue Theo Crystal in the demo map
    Added Trail Color Controllers (with a custom Loenn sprite by me!)
    Fixed some older dialog in the test map
    Removed a Loenn plugin that created a non-existent entity
# Updated to v1.2.0
    Added an option to make strawberries collect instantly
    Added Player Temple Eyes that follow players instead of Theo Crystals
    Added a default trail color for strawberry seeds
    Added a description for some settings
# Updated to v1.2.1
    Removed a setting under Experimental Options that was accidentally left in
# Updated to v1.3.0
    Added 'Stop Heart Collection' option
    Added 'Settings Affect Golden Strawberries'
    Added padding to some subheaders for a cleaner look
# Updated to v1.3.1
    Removed test map
    Fixed an issue with Stop Heart Collection not actually changing it's value
    Fixed an issue where Strawberry Blockfield Triggers wouldn't work when Strawberry Collection Speed was set to 'Instant'
# Updated to v1.3.2
    Fixed CelesteNet entities having flashing sprites and colours
    Fixed an issue with Stop Heart Collection not actually changing it's value for real
    Moved Prevent Crystal Heart Collection to Cosmetic Options
    Changed the name of Settings Affect Golden Strawberries to 'Leniency For Golden Strawberries' and added a description
# Updated to v1.3.3
    Changed 'Debug Move' to Debug Mode
    Crystal Hearts set to end the level will now set Settings.StrawberryCollectOverride to true (does not include vanilla)
# Updated to v1.3.4
    Follower and Holdable scales can now be floats
    Follower and Holdable scale range reduced to 0.0x - 10.0x
    Renamed 'resetdonotload' to 'wipedonotload'
    wipedonotload now actually works
    Holdables will now scale down when not held
    Followers and Holdables will now scale up/down slightly smoother
    The mod will now be disabled in Mod Options by default to be more compatible with custom maps
# Updated to v1.4.0
    Fixed an issue where CNet players always face left
    Added an option to stop strawberry seeds from popping
    Reworked Trail Color Controller to be easier to use
    Added Custom Depth Touch Switch (requires Tiny Adventure Dependancy)
# Updated to v1.5.0
    Changed the way the settings menu works for more customization in the future
    Removed dialogue for the old test map
# Updated to v1.6.0
    Added 'alwaysScale' option to Follower/Holdable size triggers
    Moved Instant berry collection to orig_Update for more consistent results
    Added Entity Outliner
    Added Follower Lag Buffer
        Removes any berries that have a follower index higher than the player's set limit
# Updated to v1.7.0
    Renamed 'Place Large Entities Further Back' to 'Place Resized...'
    The 'Place Resized Entities Further Back' option can now be toggled in-game
    Followers will now float to the player's spawn point on death
    Follower Lag Buffer can now be set to 0
    Added Disable Hair Gravity and Crystal Heart Blockfield Triggers
# Updated to v1.7.1
    Added descriptions to Mod Options
# Updated to v1.7.2
    Lost followers now curve instead of lerp
    