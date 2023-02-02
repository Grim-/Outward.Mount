
Mount Mod 1.1.0 Change Log (Aka the changes I remember)


 ### Mod Information / Mechanics

- The acquisition of the base mod mounts stays the same, they are rare random world drops. 

- Newly added mounts from ninedots, can be acquired in various ways some are bought, some are found and some are hatched from eggs and some are crafted.

- Dismount/Revert Form are both now bound to whatever key you use for interacting by default.

- Bag is gone!
This has been replaced by letting the player access their stash this is a much more stable method, this has the benefit of allowing you to access the same storage from any mount, there will be a config option to enable/disable access to the player stash.

- Added a small collection of components that you can add to the XML of a specific mount species, the mod itself does not make much use of these, they are provided to enable users to have more control over their mounts apperance and behaviours without having to create a new species from scratch.

 Examples such as the ability to sprint while mounted (SprintComp, GlideComp), or allowing creatures to detect proximity to items and enemies and alert you visually(DetectionComp).

There are also two mount components to force a specific species to spawn in a certain color, one for the default tinting and another for the emission (glowing bits).


### Mounts

Added Pearl Bird, Black Pearl Bird, Alpha Coral Horn, Coral Horn Doe, Manticore, Alpha Tuanosaur, Tuanosaur, Beast Golem, Beast Golem Alternate(Boss model), Obsidian Elemental, Beetle, Sand Shark from the base game as collectible mounts. 

**Special thanks goes to @Proboina for making this possible by bringing up the idea and then asking ninedots for the prefabs I would need!** 

**Ofcourse big thanks to ninedots, Gheyoom brought this up with their team and their lead designer Devo was happy to share their work!**

This request started with asking for a *single* Coral Horn prefab and we ended up with **12(!)** prefabs from ninedots they went above and beyond I'd just like to acknowledge that and thank them.


####  Acquisition
### Chersonese

> - **Pearl Bird**  
> Bought from the Stable Master
> 
> - **Pearl Bird Color variations**  
> drop as eggs from Pearl Birds and Jewel Birds.
> 
> - **Black Pearl Bird** 
Bought from the Stable Master after completing (SomeCierzoImportantQuest)




### Berg
> - **Coral Horn Doe**  
> Bought from the Stable Master
> 
> - **Alpha Coral Horn**  
> Bought from the Stable Master after completing (SomeBergImportantQuest)
> 
> - **Manticore**  
> drop as eggs from manticores and royal manticores.

### Hallowed Marsh
> - **Alpha Tuanosaur**  
> Bought from the Stable Master after completing (SomeBergImportantQuest) or rarely as eggs.
> 
> - **Tuanosaur**  
> Bought from the Stable Master

### Harmattan
> - **Beast Golem**  
> Bought from the Stable Master
> 
> - **Beast Golem Alternate(Boss model)**  
> Crafted from 4 'Pristine Beast Golem Parts' recipe learnt from Stable Master.

### Levant
> - **Beetle**  
> Bought from the Stable Master
> 
> - **Obsidian Elemental**  
> Can be summoned into existenance from an Obsidian core(Crafting recipe + 3x Fire Stone), rarely drops from Obsidian elementals.
> 
> - **Sand Shark**  
> Bought from the Stable Master after completing (SomeLevantImportantQuest)

### Mount Components

 SprintComp
> 
>  These just adds the ability to sprint to the mounts, the speed bonus
> can be modified as are most things in this mod.

```xml
<MountCompProp CompName="SprintComp" xsi:type="SprintCompProp">
<SprintModifier>2</SprintModifier>
</MountCompProp>
```

 GlideComp
> 
> This just adds the ability to sprint to the mounts, the speed bonus
> can be modified as are most things in this mod, it will also lift a
> little off the ground.

```xml
<MountCompProp CompName="GlideComp" xsi:type="GlideCompProp">
<SprintModifier>2</SprintModifier>
</MountCompProp>
```


 ColorableComp 
> 
> Forces a species to spawn in, as a certain color, the
> color is in hexadecimal format.

```xml
<MountCompProp CompName="ColorableComp" xsi:type="ColorableCompProp">
<TintColor>#ff00ff</TintColor>
<EmissionColor>#ff00ff</EmissionColor>
</MountCompProp>
```

 DetectionComp 
> 
> Enables the mount to periodically detect Items,
> Gatherables and Enemies around you, this is visually represented by
> the glowing parts of the mount, yellow is for loot, green for
> gatherables and red for enemies, the stronger the glow the closer they
> are, the glow will double in strength if you are facing towards the
> nearest detected target.

```xml
<MountCompProp CompName="DetectionComp" xsi:type="DetectionCompProp">
<DetectionRadius>50</DetectionRadius>
<BaseIntensity>2</BaseIntensity>
<MinIntensity>0</MinIntensity>
<MaxIntensity>25</MaxIntensity>
<DetectionInterval>0.5</DetectionInterval>
</MountCompProp>
```


 EmissionBlendComp 
 >Blends between the two specified colors over the
> specified time, just to look pretty.

```xml
<MountCompProp CompName="EmissionBlendComp" xsi:type="EmissionBlendCompProp">
<StartColor>#ff00ff</StartColor>
<EndColor>#ff00ff</EndColor>
<BlendTime>10</BlendTime>
</MountCompProp>
```


#### Coloring

- All of the Outward Base Game mount models can now be re-tinted.
*This does not look so great on some as others so this is disabled by default (but you can force any color you want on any of them with the components).*

- There are 9 different varities of **'color berries'** these will be bought from the stable masters in any city for X silver (the berry that resets to default color will be free)

**Special thanks to @Schnabeldoktor another fellow modder who graciously did the icons for these berries, something I am utterly incapable at.**


#### Mount 'Transformations'

 Added transformation active skills for all nine base game mounts, instead of whistling and having your mount accompany you, you can transform into the form of that mount while out of combat when you revert (using the **interact** key) this form is destroyed.
 
A natural downside to this means you lose access to the stash made available by your mount normally, but you no longer need to worry about feeding your mount, the form still acts like a mounted mount in every other regard.

'mmo' style mounts were requested more than once, this is my compromise.

**Another special thanks to @Libre comme l'air of the modding discord for creating the skill icons, and the new Summon Active Mount and Dismiss Mount skill icons.**


I have only implemented the XML for these skills for the Outward mount models.
But you can edit them to use any of the mounts in this mod, this is easily done with SideLoader or simply editing the XML file, be sure to specify a New_ItemID in the XML if you do not wish to overwrite a current one. 

#### Quests

##### Build a Beast
You can build your own beast golem by finding the 'Pristine Beast Golem Part's scattered around harmattan.

##### Final Fantasy Aurai
Certain creatures (Pearl Bird, Manticore, Tuanosaur) can now drop eggs and infact thats the only way to get certain varities of mounts.

*If you have a Pearl Bird Egg quest you won't get another until that one is complete.

 **Pearl Bird eggs have a chance it will spawn with a different color (Yellow, Blue, Red, Green, Black, and Gold)**

These eggs will give you a quest to wait 12 hours then the egg hatches you cannot hatch more than one species of egg at a time.

A small bug present currently is the quest timer for the eggs hatching is reset every time you load a save, I am working on this but it is rather minor.

##### Note :

- Alpha Coral Horn, Coral Horn Doe, Manticore, Alpha Tuanosaur, Beast Golem, Beast Golem Alternate(Boss model), Obsidian Elemental and Sand Sharks can all spawn with random colors for their emission (the parts of the model that glow) this change be forcibly changed using the ColorableComp!
