World implementation guide

Overview
The World prefab is used to generate the map
It has four scripts, World, Terrain handler, Item Spawner and Noise handler.

Configuration

	Terrain handler
	Determines the size and shape of the world.

	View distance
	-The distance from the player prefab in which chunks of the world will be viewable.

	World width/height:
	-The size (in chunks) of the world to be created

	Chunk size:
	-The size (in meters) of each chunk to be generated

	Chunk prefab:
	-The chunk to be used in world generation

	Noise handler:
	Configure noise to alter the shape of the terrain

	Noise
	-A list of noise layers
	-Frequency: The frequency of perlin noise for this layer
	-Intensity: A fixed 0-1 multiplier for a given noise layer

	Terrain threshold
	-The minimum noise value to generate terrain at a given point
	
	Edge thickness
	-Defines space around the edge of the map in which terrain will generate
	
	Seed
	-A seed to generate a specific map. Alters the terrain

	Item spawner:
	A script to spawn items around the map

	Items
	-A list of item objects to generate
	-Each item has attributes for the prefab to spawn, how many, and where on the map to spawn it.


Usage
Drag the World prefab into your scene
Configure according to your desired world setup
Use the world script attached to the prefab to destroy terrain
Call DestroyAtPoint to destroy all terrain within the specified radius, at the specified point.