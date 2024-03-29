Example of a Mount Species Definition.

Below is a template you can use to create your own MountSpecies, you can use the visuals of any of the existing species and change which ever parts you like.
Remember to update the WhistleItemID to a new ID unless you intend to replace the original species.

These .xml files must be placed in the Mod/MountSpecies folder.


```xml
<MountSpecies xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	<WhistleItemID>-999993</WhistleItemID>
	<TargetItemID>4300130</TargetItemID>
	<IsWhistleWorldDrop>false</IsWhistleWorldDrop>
	<!-- Species Info !-->
	<SpeciesName>EliteSandShark</SpeciesName>
	<SLPackName>mount</SLPackName>
	<AssetBundleName>mount_original</AssetBundleName>
	<PrefabName>Mount_EliteShark</PrefabName>
	<!-- A list of names picked at random when spawned in for the first time !-->
	<Names>
		<string>Lunarclaw</string>
		<string>Shadowpaw</string>
		<string>Starfur</string>
		<string>Thunderjaw</string>
		<string>Iceheart</string>
		<string>Moonshade</string>
		<string>Stormfur</string>
		<string>Wildfire</string>
		<string>Riverwind</string>
		<string>Mountainfang</string>
	</Names>
	<CameraOffset>
		<x>0</x>
		<y>0</y>
		<z>-2</z>
	</CameraOffset>
	<!-- Speed !-->
	<MoveSpeed>6.5</MoveSpeed>
	<SprintModifier>1.5</SprintModifier>
	<RotateSpeed>240</RotateSpeed>
	<Acceleration>5</Acceleration>
	<!-- Carry Weight !-->
	<MaximumCarryWeight>250</MaximumCarryWeight>
	<CarryWeightEncumberenceLimit>0.8</CarryWeightEncumberenceLimit>
	<EncumberenceSpeedModifier>0.5</EncumberenceSpeedModifier>
	<!-- Food !-->
	<RequiresFood>true</RequiresFood>
	<MaximumFood>200</MaximumFood>
	<!-- The time between food being taken from your mount !-->
	<PassiveFoodLossTickTime>200</PassiveFoodLossTickTime>
	<!-- The amount of food being taken from your mount !-->
	<PassiveFoodLoss>10</PassiveFoodLoss>
		<!-- The amount of distance your mount can travel befor needing to use some food !-->
	<TravelDistanceThreshold>150</TravelDistanceThreshold>
		<!-- The amount of food being taken from your mount !-->
	<FoodLostPerTravelDistance>20</FoodLostPerTravelDistance>
	<FoodTags>
		<string>Meat</string>
	</FoodTags>
	<!-- A List of MountFoodData of mounts favourite foods !-->
	<FavouriteFoods>
			<!-- A List item of MountFoodData !-->
		<MountFoodData>
			<!-- Game ItemID (Jewel Bird Meat) !-->
			<ItemID>4000260</ItemID>
			<!-- A modifier on the food value of the item !-->
			<FoodValue>2</FoodValue>
		</MountFoodData>
	</FavouriteFoods>
	<HatedFoods>
	</HatedFoods>
	<!-- Color !-->
	<!-- Should this species generate a random Tint color from the MountColors list when spawned? !-->
	<GenerateRandomTint>false</GenerateRandomTint>
	<!-- Should this species generate a random Emission color from the MountEmissionColors list when spawned? !-->
	<GenerateRandomEmission>false</GenerateRandomEmission>
	<!-- Components !-->
	<MountComponents>
		<MountCompProp CompName="EmissionBlendComp" xsi:type="EmissionBlendCompProp">
			<StartColor>
			<r>7.2</r>
			<g>3.01</g>
			<b>1</b>
			<a>1</a>
			</StartColor>
			<EndColor>
			<r>3.01</r>
			<g>1</g>
			<b>9.01</b>
			<a>1</a>
			</EndColor>
			<BlendTime>10</BlendTime>
			<ReverseOnComplete>true</ReverseOnComplete>
		</MountCompProp>
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
		<MountCompProp CompName="GrantStatusInRangeComp" StatusName="Environment Heat Resistance" xsi:type="GrantStatusInRangeCompProp" Radius="15" />
	</MountComponents>
</MountSpecies>

```
