Example of a EMM Mount Species Definition.

Below is a template you can use to create your own MountSpecies, you can use the visuals of any of the existing species and change which ever parts you like.
Remember to update the WhistleItemID to a new ID unless you intend to replace the original species.


```xml
<MountSpecies xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
<WhistleItemID>-26300</WhistleItemID>
<TargetItemID>4300130</TargetItemID>
<GenerateRandomTint>false</GenerateRandomTint>
<MountColors>
	<ColorChance Chance="70" Color="#ffffff" />
	<ColorChance Chance="8" Color="#0000ff" />
	<ColorChance Chance="5" Color="#00ff00" />
	<ColorChance Chance="2" Color="#ff0000" />
	<ColorChance Chance="2" Color="#ff00ff" />
	<ColorChance Chance="2" Color="#00ffff" />
	<ColorChance Chance="2" Color="#ffff00" />
	<ColorChance Chance="1" Color="#800000" />
	<ColorChance Chance="1" Color="#008000" />
	<ColorChance Chance="1" Color="#000080" />
	<ColorChance Chance="1" Color="#008080" />
	<ColorChance Chance="1" Color="#800080" />
	<ColorChance Chance="1" Color="#808000" />
</MountColors>
<SpeciesName>PearlBird</SpeciesName>
<SLPackName>mount</SLPackName>
<AssetBundleName>mount_original</AssetBundleName>
<PrefabName>Mount_PearlBird</PrefabName>
<Names>
	<string>Chocobo</string>
</Names>
<CameraOffset>
	<x>0</x>
	<y>0</y>
	<z>-4</z>
</CameraOffset>
<MoveSpeed>12</MoveSpeed>
<RotateSpeed>90</RotateSpeed>
<Acceleration>5</Acceleration>
<MaximumCarryWeight>70</MaximumCarryWeight>
<CarryWeightEncumberenceLimit>0.8</CarryWeightEncumberenceLimit>
<EncumberenceSpeedModifier>0.5</EncumberenceSpeedModifier>
<FoodTags>
	<string>Meat</string>
</FoodTags>
<FavouriteFoods>
	<MountFoodData>
		<ItemID>4000260</ItemID>
		<FoodValue>2</FoodValue>
	</MountFoodData>
</FavouriteFoods>
<HatedFoods>
</HatedFoods>
<MaximumFood>200</MaximumFood>
<FoodTakenPerTick>10</FoodTakenPerTick>
<HungerTickTime>200</HungerTickTime>
<MountComponents>
</MountComponents>
</MountSpecies>
```
