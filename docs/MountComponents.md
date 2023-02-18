
Please visit [ExampleMountDefinition](https://github.com/Grim-/Outward.Mount/edit/main/docs/ExampleMountDefinition.md) if you are un-sure where to place the XML you will find the example contains various MountComponents near the bottom of the XML.


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

 GrantStatusInRangeComp 
> Grants the Specified StatusEffect to it's Owner while in-range.

```xml
	<MountCompProp CompName="GrantStatusInRangeComp" StatusName="Environment Resistance" xsi:type="GrantStatusInRangeCompProp" Radius="15" />
```


 LightableComp 
> Creates a Light at the specified position on relative to the mount's local space, you can also specify the color, range and intensity of the light.

```xml
		<MountCompProp CompName="LightableComp" xsi:type="LightableCompProp">
			<Position>
				<x>0</x>
				<y>1</y>
				<z>0</z>
			</Position>
			<Color>
				<r>0</r>
				<g>0.1</g>
				<b>0.2</b>
				<a>1</a>
			</Color>
			<Intensity>10</Intensity>
			<Range>15</Range>
		</MountCompProp>
```
