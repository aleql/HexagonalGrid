# Hexagonal Grid Demo

![](https://github.com/aleql/HexagonalGrid/Other/hex_main.png)

This is a demo that showcases functionalities on an hexagonal tile grid using a cubic coordinate system.

The use of this coordinate system allows for the optimized O(1) implementation of operations such as: Distance between two tiles, all neighbors of a tile and all tiles at a distance.

This grid features 12 terrain types, being two of these mountains that feature a different elevation that are considered non-navigable tiles. For this implementation, Unity's ScriptableObjects were used, creating one for each terrain type and their respective properties.

The following functionalities are available on this demo:
+ A path finding between two tiles using the A* algorithm.
![](https://github.com/aleql/HexagonalGrid/Other/pathfinding.gif)

+ Resizing of the grid radious.
![](https://github.com/aleql/HexagonalGrid/Other/resize.gif)

+ Change the terrain type of the selected tiles.
![](https://github.com/aleql/HexagonalGrid/Other/reroll.gif)

This demo allows for interaction, implemented through Unity's new input system. 
The following controls are available:
		Move Camera: Mouse movement
		Select Tile: Mouse left click
		Rotate Cameta: Mouse 3
		Camera Zoom: Mouse wheel

This demo is available [here!](https://aleql.github.io/HexagonalGridDemo/)

Resources:
[https://www.redblobgames.com/grids/hexagons/](https://www.redblobgames.com/grids/hexagons/)
[https://assetstore.unity.com/packages/2d/textures-materials/tiles/hexlands-low-poly-style-133586](https://assetstore.unity.com/packages/2d/textures-materials/tiles/hexlands-low-poly-style-133586)
