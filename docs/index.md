## Mount Mod 1.1.0
Hi! 

Welcome to the 1.1.0 update page for Emo's Mount Mod!

### Changes Summary 

- Added [Mounts](https://github.com/Grim-/Outward.Mount/blob/main/docs/Mounts.md) (**WARNING SPOILER HEAVY PAGE**) made available by ninedots these can be acquired in various ways.

    All the added ninedots mounts are color tintable by default, due to the base textures available for these mounts the tinting works better for some species (PearlBird) than the others, all the ninedot mounts also have customizable emission (Such as the glowing chest of the Alpha Coral Horn or tentacles of the SandShark) which works perfectly fine.

- Added Mounts components system allowing players to customize certain aspects of the mount.

- Added two stable masters to the final two game areas (Harmattan and New Sirocco)

- All 4 original Stable Masters now sell a mount (either egg or whistle) for 300 silver, maybe they even sell something else after certain conditions are met? 

    *You should probably check around the town for people needing help?* or visit [Mounts](https://github.com/Grim-/Outward.Mount/blob/main/docs/Mounts.md)(**WARNING SPOILER HEAVY**)


- Mounts now act a little bit more alive, this is again limited by the animations I have available for all models, but they should while idle feel a little more alive.

- Added config options for Mount Leash Distance, Leash Radius, ColorBerry cost, FoodLostTraveling, TravelDistanceThreshold, EncumberanceModifier, DisableNonNinedots, EnableFoodNeed, EnableWeightLimit, WeightLimitOverride.
    
    Any of these settings will override for all mount species, if you just want to make an edit to a single species, this can be done in the xml file for that Species located in **'ModFolder/MountSpecies'** please visit [Example Mount Definition](https://github.com/Grim-/Outward.Mount/edit/main/docs/ExampleMountDefinition.md) for more information.

- Icons!
    Thanks to Schnabeldoktor &  Libre comme l'air of the modding discord for all the egg, skill icons and item icons! <3

- Dismount/Revert Form are both now bound to whatever key you use for interacting by default. 

    Default F on PC.

- Bags are gone and replaced with access to the Player Stash.

The serialization and deserialization of a bag and it's contents has always been the biggest cause of problems within the mod itself, often what happens is the bag registered to the mount is not always correctly ignored by the ItemManager, causing either the bag to be deleted on a scene close, or sometimes be duplicated causing issues with the mounting and dismounting process leading to all sorts of wierd problems.

## Mounts

Added Pearl Bird, Black Pearl Bird, Alpha Coral Horn, Coral Horn Doe, Manticore, Alpha Tuanosaur, Tuanosaur, Beast Golem, Beast Golem Alternate(Boss model), Obsidian Elemental, Beetle, Sand Shark from the base game as collectible mounts. 

**Special thanks goes to @Proboina for making this possible by bringing up the idea and then asking ninedots for the prefabs** 

**Ofcourse big thanks to ninedots, Gheyoom brought this up with their team and their lead designer Devo was happy to share their work!**

This request started with asking for a *single* Coral Horn prefab and we ended up with **14(!)** prefabs from ninedots they went above and beyond I'd just like to acknowledge that and thank them.


## Mounts Acquisition
Please visit the [Mounts](https://github.com/Grim-/Outward.Mount/blob/main/Mounts.md) page for more information.



## Mount Components
These are components I created that you can add to a mount by editing the MountSpecies definition under the <MountComponents> </MountComponents> section these are rarely used by the mod itself, some are overpowered in the context of Outward, others people simply would not like them detracting from a certain RP element or story and they are most something I provided as a test of data-orientated design and to give users some features requested without making them mandatory for everyone.

I am open to adding more of these at a later date, suggestions welcome.


You can find the full list with examples [here](https://github.com/Grim-/Outward.Mount/blob/main/docs/MountComponents.md)

## FAQ 

- Why no horse? - These are all freely available assets made available by generous 3D Artists that I have used to create the available mounts, simply put I could not find a free Horse model that is fully rigged with the minimum required animations.

- __Why dont you use the in-game Animals?__ - It does now!

Exporting all the required Assets from the game files is actually massively time consuming and requires often the armature and rigging, weighting to be redone. I am looking into another method of obtaining the resources I need to make this happen eventually.

## Change Log

v.1.0.3 
- Added ModConfig Options for Enabling Food and Weight Requirements and whistle drop chances.
- Hopefully finally fixed the bag issue, the bag should now be fully stable. 
- Updated the Storage UI to allow for scrolling.
- Added Summon Active Mount and Dismiss Active Mount Skills learnable from the Stable Masters, these allow you to dismiss and summon your current active mount, you still need to visit the stables to change your active mount.
- Fixed an issue where dying while mounted caused the player to be stuck parented to the mount.
- Updated the cosmic skin shader, updated the holy wolf, air golem visuals.
- Added Raptor mount type and Lizard mount type, bringing the total to 5 mounts with different variations.

## Known Issues
- The Mount currently teleports to you while using combat teleports, this isn't intended and is a side effect of me trying to keep the mount with the player between area changes, it will hopefully be fixed.

### Potential Future Updates
 - Lanterns, pets that can act as cooking pots and other things. 

## Credits

- Iggy + Sinai for allowing me to use some of the code from his Alternate Start mod for the NPCs.

- Faeryn for their patches allowing mounts to be fed by right clicking the item in your inventory.

- AlienVsYourMom and Avrixel of the modding Discord for testing and providing feedback.

- Schnabeldoktor &  Libre comme l'air of the modding discord for all the egg, skill icons and item icons! No more Push Kick icon!

- [Dungeon Mason](https://assetstore.unity.com/packages/3d/characters/creatures/dragon-the-soul-eater-and-dragon-boar-77121#description) for the DragonBoar and the Golem!

- [Dzen Games](https://assetstore.unity.com/packages/3d/characters/animals/wolf-animated-45505#description) for the Wolf.

- ChatGPT for coming up with all the names for the species :D
