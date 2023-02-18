## Mount Mod 1.1.0
Hi! 

Welcome to the 1.1.0 update page for Emo's Mount Mod!

This update has been a long time coming but due to family health issues I had to postpone the mod for almost 7 months and since then I have only been able to work on it in small amounts due to having very limited free time.

Although changes come at a slower pace than I'd like this is due to the wide scope of the mod while the intent is seemingly simple on the surface 'add a faster way to travel around Aurai without teleporting' all the accompanying systems such as custom quests, dialogue, NPCs, UI, Skills, Items, The Mount themselves are slightly more complex in most cases than SideLoader allows (with very good reason).

If you have ever had to programmatically write a node-graph (or any kind of graph) without an accompanying UI you will understand this is not a simple task, the documentation for NodeCanvas (the package used by 9dots in this case) simply states its an advanced topic and doesn't elobrate. \o/


**My gripes and excuses for the pathetic update rate out of the way here are (most of) the changes I have made. (or Just skip to here!)**



### Changes Summary 

- Added [Mounts](https://github.com/Grim-/Outward.Mount/blob/main/docs/Mounts.md) (**WARNING SPOILER HEAVY PAGE**) made available by **9dots** these can be acquired in various ways.

    All the added **9dots** mounts are color tintable by default due to the base textures available for these mounts the tinting works better for some species (PearlBird) than the others.

    All the **9dot** mounts also have customizable emission (Such as the glowing chest of the Alpha Coral Horn or tentacles of the SandShark).

- Added Mounts components system allowing players to customize certain aspects of the mount.

- Added Eggs for certain species where applicable otherwise whistles still remain.

- All Mounts can now sprint.

- Added two stable masters to the final two game areas (Harmattan and New Sirocco)

- All 4 original Stable Masters now sell a mount (either egg or whistle) for 300 silver, maybe they even sell something else after certain conditions are met? 

    *You should probably check around the town for people needing help?* or visit [Mounts](https://github.com/Grim-/Outward.Mount/blob/main/docs/Mounts.md)(**WARNING SPOILER HEAVY**)

- Removed Mount/Dismount custom keybind

- Mounts now act a little bit more alive this is again limited by the animations I have available for all models, but they should while idle feel a little more alive hopefully.

- Added config options for Mount Leash Distance, Leash Radius, ColorBerry cost, FoodLostTraveling, TravelDistanceThreshold, EncumberanceModifier, DisableNonNinedots, EnableFoodNeed, EnableWeightLimit, WeightLimitOverride.
    
    Any of these settings will override for all mount species, if you just want to make an edit to a single species, this can be done in the xml file for that Species located in **'ModFolder/MountSpecies'** please visit [Example Mount Definition](https://github.com/Grim-/Outward.Mount/edit/main/docs/ExampleMountDefinition.md) for more information.

- The 9dots mounts have both walking and running animations and so use a better animator controller to take advantage of this.

- Icons!
    Thanks to Schnabeldoktor &  Libre comme l'air of the modding discord for all the egg, skill icons and item icons! <3

-  Removed Mount&Dismount custom keybind & Mount&Dismount/Revert Form are both now bound to whatever key you use for interacting by default. 

    Default F on PC.

- Bags are gone and replaced with access to the Player Stash.

    The serialization and deserialization of a bag and it's contents has always been the biggest cause of problems within the mod itself often what happens is the bag registered to the mount is not always correctly ignored by the ItemManager, causing either the bag to be deleted on a scene close, or sometimes be duplicated causing issues with the mounting and dismounting process leading to **all sorts** of wierd problems, such as missing hit boxes, losing the ability to interact and other wierd otherwise unexplainable issues.

## Mounts

This request started with asking for a *single* Coral Horn prefab and we ended up with **14(!)** prefabs from ninedots they went above and beyond I'd just like to acknowledge that and thank them.

Added Pearl Bird, Black Pearl Bird, Alpha Coral Horn, Coral Horn Doe, Manticore, Alpha Tuanosaur, Tuanosaur, Beast Golem, Beast Golem Alternate(Boss model), Obsidian Elemental, Beetle, Sand Shark from the base game as collectible mounts. 

**Special thanks goes to @Proboina for making this possible by bringing up the idea and then asking ninedots for the prefabs** 

**Ofcourse big thanks to 9dots, Gheyoom brought this up with their team and their lead designer Devo was happy to share their work!**


### Mounts Acquisition

As a general summary you can buy a basic mount for 300 silver from Stable Masters in each of the four base game cities (this is done to not lock out anyone without DLC).

You can also find PearlBird eggs as drops from PearlBirds and their nests with nests having double the chance of dropping an egg.

Each of these Stable Masters also another mount that becomes available after completing certain tasks for other NPC'S in the same city.

There are also better mounts that cannot be found anywhere except through crafting the recipes are acquired through defeating certain enemies or finding certain items.

The reagents for these recipes are available at your nearest Alchemy vendor these are quite expensive but fear not they can also drop from the creature/place that provides the recipe.



Please visit the [Mounts](https://github.com/Grim-/Outward.Mount/blob/main/Mounts.md) page for more information, this is spoiler heavy.



## Eggs?
1.1.0 introduces Eggs into the mod.

Eggs can either drop from certain creatures upon defeat or found in their nests.

Eggs are only available where it would make sense.

These Eggs when acquired and used from the inventory which will start a quest where you must simply keep the egg present in your inventory for 12 hours at which time it will hatch in the case of pearlbirds you might get an egg with a slight color variation and if you are very lucky you might even find the fabled *QuickSilver PearlBird.*

Certain PearlBird variants have ....darker origins and must be 'created'.

## Mount Components
These are components I created that you can add to a mount by editing the MountSpecies definition under the ```<MountComponents> </MountComponents>``` section these are rarely used by the mod itself.

Some are overpowered in the context of Outward others people simply would not like them detracting from a certain RP element, think them silly or any reason really the choice is yours.

They are mostly something I provided as a test of data-orientated design within Unity itself and to give users some features requested without making them mandatory for everyone.

I am open to adding more of these at a later date, suggestions welcome.


You can find the full list with examples [here](https://github.com/Grim-/Outward.Mount/blob/main/docs/MountComponents.md)

## FAQ 

- Why no horse? 

These are all freely available assets made available by generous 3D Artists that I have used to create the available mounts, simply put I could not find a free Horse model that is fully rigged with the minimum required animations.

- __Why dont you use the in-game Animals?__ - It does now (14!)!

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

- [Exp111](https://exp111.github.io/OutwardNodeCanvasViewer/#quests/7011000_Neutral_RealIntro_Quest.json) of the modding discord for his OutwardNodeCanvasViewer which while it came along late into the mods development but hugely increased my understanding of the games graph system, and therefore speed in getting the mod out -  thank you!

- [Dungeon Mason](https://assetstore.unity.com/packages/3d/characters/creatures/dragon-the-soul-eater-and-dragon-boar-77121#description) for the DragonBoar and the Golem!

- [Dzen Games](https://assetstore.unity.com/packages/3d/characters/animals/wolf-animated-45505#description) for the Wolf.
