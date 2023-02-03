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
