Mount Mod 1.1.0 Change Log (Aka the changes I remember)


 ### Mod Information / Mechanics

- The acquisition of the base mod mounts stays the same, they are rare random world drops. 

- Newly added mounts from ninedots, can be acquired in various ways, some are bought, some are found and some are hatched from eggs.

- Dismount/Revert Form are both now bound to whatever key you use for interacting.

- Bag is gone, replaced by letting the player access their stash this is a much more stable method, this has the benefit of allowing you to access the same storage from any mount, there will be a config option to enable/disable access to the player stash.

- Added a small collection of components that you can add to the XML of a specific mount species, the mod itself does not make much use of these, they are provided to enable users to have more control over their mounts apperance and behaviours without having to create a new species from scratch.

 Examples such as the ability to sprint while mounted (SprintComp, GlideComp), or allowing creatures to detect proximity to items and enemies and alert you visually(DetectionComp).

There are also two mount components to force a specific species to spawn in a certain color, one for the default tinting and another for the emission (glowing bits).


### Mounts

- Added Pearl Bird, Black Pearl Bird, Alpha Coral Horn, Coral Horn Doe, Manticore, Alpha Tuanosaur, Tuanosaur, Beast Golem, Beast Golem Alternate(Boss model), Obsidian Elemental, Beetle from the base game as mounts. 

Special thanks goes to @Proboina for making this possible by bringing up the idea and then asking ninedots for the prefabs I would need! 
Ofcourse big thanks to ninedots, Gheyoom brought this up with their team and the lead designer Devo was happy to share their work!

This request started with asking for a single Coral Horn prefab and we ended up with 11(!) prefabs from ninedots they went above and beyond I'd just like to acknowledge that.


#### Mount Components
 - SprintComp & GlideComp
  These just add the ability to sprint to the mounts, the speed bonus can be modified as are most things in this mod.

```xml
		<MountCompProp CompName="SprintComp" xsi:type="SprintCompProp">
				<SprintModifier>2</SprintModifier>
		</MountCompProp>
```

```xml
		<MountCompProp CompName="GlideComp" xsi:type="GlideCompProp">
				<SprintModifier>2</SprintModifier>
		</MountCompProp>
```


- ColorableComp

```xml
		<MountCompProp CompName="ColorableComp" xsi:type="ColorableCompProp">
				<TintColor>#ff00ff</TintColor>
   	<EmissionColor>#ff00ff</EmissionColor>
		</MountCompProp>
```

- DetectionComp
```xml
			<MountCompProp CompName="DetectionComp" xsi:type="DetectionCompProp">
					<DetectionRadius>50</DetectionRadius>
					<BaseIntensity>2</BaseIntensity>
					<MinIntensity>0</MinIntensity>
					<MaxIntensity>25</MaxIntensity>
					<DetectionInterval>0.5</DetectionInterval>
			</MountCompProp>
```


-EmissionBlendComp

```xml
		<MountCompProp CompName="EmissionBlendComp" xsi:type="EmissionBlendCompProp">
			<StartColor>#ff00ff</StartColor>
   <EndColor>#ff00ff</EndColor>
   <BlendTime>10</BlendTime>
		</MountCompProp>
```


#### Coloring

- All of the Outward Base Game mount models can now be re-tinted, this does not look so great on some as others so this is disabled by default (but you can force any color you want on any of them more on that later).

- There are 9 different varities of 'color berries' these will be bought from the stable masters in any city for X silver (the berry that resets to default color will be free)

Special thanks to @Schnabeldoktor another fellow modder who graciously did the icons for these berries, something I am utterly incapable at.


#### Mount 'Transformations'
- Added transformation active skills for all nine base game mounts, instead of whistling and having your mount accompany you, you can transform into the form of that mount while out of combat when you revert this form is destroyed, as a downside this means you lose access to the stash from your mount, but you no longer need to worry about feeding your mount.


I have only implemented the XML for these skills for the Outward mount models but you can edit them to use any of the mounts in this modin this mod, this is easily done with SideLoader or simply editing the XML file.

#### Quests

- Certain creatures (Pearl Bird, Manticore, Tuanosaur) can now drop eggs and infact thats the only way to get certain varities of mounts.
These eggs will give you a quest to wait 12 hours then the egg hatches, you cannot hatch more than one species of egg at a time, this only applies to certain species not all of them. 

*If you have a Pearl Bird Egg quest you won't get another until that one is complete, **if it is a Pearl Bird egg then there is a chance it will spawn with a different color (Yellow, Blue, Red, Green, Black, and Gold)** and some of you might even be ancient enough to get the reference.*

- Alpha Coral Horn, Coral Horn Doe, Manticore, Alpha Tuanosaur, Beast Golem, Beast Golem Alternate(Boss model), Obsidian Elemental can all spawn with random colors for their emission (the parts of the model that glow).
