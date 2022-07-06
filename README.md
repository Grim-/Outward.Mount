# Emo's Outward Mount Mod v1.0.3

![image](https://user-images.githubusercontent.com/3288858/175751166-ab3d6da0-a3a6-4aa6-880d-aa6197d8a627.png)


##### For any bug reports please visit the mount-mod channel of the [Outward Modding Discord](https://discord.gg/zKyfGmy7TR). 


**Currently I consider this 'early access' as there quite a few moving parts, I hope to have caught the majority of them but I can't promise a bug-free experience so you might want to back up your saves before installing it, I have found nothing game breaking so far...**

The base mod comes with 5 types of mounts each with a few variations. 

There are 4 NPCs stationed in the 4 base game cities that allow you to store and retrieve mounts incase you decide to change later, currently there is no cost inccured to do this, but this might change later.

I'll leave it to people to find them in-game, currently they are world drops that have a small chance of dropping from everything you can loot.

The numbers will be adjusted later.

## You must also set the three required keybinds(Follow/Wait, GoTo, Dismount) in the keybind options.


### Getting a mount

First you must acquire a whistle in order to summon a mount for the first time these are currently world drops with a small chance, after that it will follow you wherever you go with the caveat it can't follow you into places where there is no "NavMesh" for it to use these are usually places you would find no moving NPCs, one example at the start is the player house in Ciezro. 

You can still mount and ride them pretty much anywhere you would expect the player to be able to go.

### Feeding

The mounts require feeding they lose a little bit of food passively over time and a little for X distance travelled while mounted, most of these settings can be changed in the XML files found in "MountSpecies" folder. 

Everything currently eats either Vegetables or Meat. 

The value on the UI for the food is the amount it will restore. With Favourite foods restoring more and making your mount happy.


## Stable Masters
Stable Masters found in the four major cities will store your active mount should you wish to change it, you can also retrieve a stored mount from here and learn some skills to better manage your mount.


#### Current Food Prefences

- Wolf/Dragonboar/Lizard/Raptor - Meat items - Loves Jewel Bird Meat
- Golems - Vegetables - Loves MarshMelons.


### Utility
Mounts also come with a bag, mounts like the player have a carry limit, which is a combination of the things in the mounts bag and your own inventory. 
Generally mounts can carry more than the player can over longer distances quicker, with the draw back they need feeding more. 

The mount should also fully save including all its bag contents, please let me know if you run across any bugs.


## Notes 
There is no mounted combat, the mounts were designed to help with moving between areas in Outward without fast travel.

I have set the mod up so others can add mounts later on, I have yet to document this but it is possible.


## FAQ 

- Why no horse? - These are all freely available assets made available by generous 3D Artists that I have used to create the available mounts, simply put I could not find a free Horse model that is fully rigged with the minimum required animations.

- Why dont you use the in-game Animals? - Exporting all the required Assets from the game files is actually massively time consuming and requires often the armature and rigging, weighting to be redone. I am looking into another method of obtaining the resources I need to make this happen eventually.

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
- You can 'steal' the mounts bag by pressing "take all" with no items in the inventory, aslong as this bag is not destroyed or sold it should reattach itself to the mount on reload, otherwise a new one will be created.
- The Mount currently teleports to you while using combat teleports, this isn't intended and is a side effect of me trying to keep the mount with the player between area changes, it will hopefully be fixed.


## Remaining 
- Spice up the mount statistics and favourite foods.

## Potential Future Updates
 - Lanterns, pets that can act as cooking pots and other things. 
 - Update the UI.
 - Hopefully find a nice mounted seating animation.


## Credits

- Iggy + Sinai for allowing me to use some of the code from his Alternate Start mod for the NPCs.

- Faeryn for their patches allowing mounts to be fed by right clicking the item in your inventory.

- AlienVsYourMom and Avrixel of the modding Discord for testing and providing feedback.

- [Dungeon Mason](https://assetstore.unity.com/packages/3d/characters/creatures/dragon-the-soul-eater-and-dragon-boar-77121#description) for the DragonBoar and the Golem!

- [Dzen Games](https://assetstore.unity.com/packages/3d/characters/animals/wolf-animated-45505#description) for the Wolf.

