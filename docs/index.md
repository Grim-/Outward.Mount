## Mount Mod 1.1.0

Hi! 

Welcome to the 1.1.0 update page for Emo's Mount Mod!




### Changes Summary 

- Bags are gone and replaced with access to the Player Stash.

The serialization and deserialization of a bag and it's contents has always been the biggest cause of problems within the mod itself, often what happens is the bag registered to the mount is not always correctly ignored by the ItemManager, causing either the bag to be deleted on a scene close, or sometimes be duplicated causing issues with the mounting and dismounting process leading to all sorts of wierd problems.




### FAQ 

- Why no horse? - These are all freely available assets made available by generous 3D Artists that I have used to create the available mounts, simply put I could not find a free Horse model that is fully rigged with the minimum required animations.

- Why dont you use the in-game Animals? - Exporting all the required Assets from the game files is actually massively time consuming and requires often the armature and rigging, weighting to be redone. I am looking into another method of obtaining the resources I need to make this happen eventually.

### Change Log

v.1.0.3 
- Added ModConfig Options for Enabling Food and Weight Requirements and whistle drop chances.
- Hopefully finally fixed the bag issue, the bag should now be fully stable. 
- Updated the Storage UI to allow for scrolling.
- Added Summon Active Mount and Dismiss Active Mount Skills learnable from the Stable Masters, these allow you to dismiss and summon your current active mount, you still need to visit the stables to change your active mount.
- Fixed an issue where dying while mounted caused the player to be stuck parented to the mount.
- Updated the cosmic skin shader, updated the holy wolf, air golem visuals.
- Added Raptor mount type and Lizard mount type, bringing the total to 5 mounts with different variations.

### Known Issues
- The Mount currently teleports to you while using combat teleports, this isn't intended and is a side effect of me trying to keep the mount with the player between area changes, it will hopefully be fixed.

### Potential Future Updates
 - Lanterns, pets that can act as cooking pots and other things. 

## Credits

- Iggy + Sinai for allowing me to use some of the code from his Alternate Start mod for the NPCs.

- Faeryn for their patches allowing mounts to be fed by right clicking the item in your inventory.

- AlienVsYourMom and Avrixel of the modding Discord for testing and providing feedback.

- [Dungeon Mason](https://assetstore.unity.com/packages/3d/characters/creatures/dragon-the-soul-eater-and-dragon-boar-77121#description) for the DragonBoar and the Golem!

- [Dzen Games](https://assetstore.unity.com/packages/3d/characters/animals/wolf-animated-45505#description) for the Wolf.
