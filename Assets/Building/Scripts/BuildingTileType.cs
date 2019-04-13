using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Building {
    [System.Serializable]
    public class BuildingTileType {
        public Dictionary<string, WallType> typeOfWall =
            new Dictionary<string, WallType>() {

            { "north", WallType.none },
            { "west", WallType.none },
            { "east", WallType.none },
            { "south", WallType.none }

        };
    }
}