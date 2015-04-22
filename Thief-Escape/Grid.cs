/*
 * Zachary T. Vig
 * Jamie Gleason
 * Keegon Cabinaw
 * GROUP PHIV
 * CIT195 Group Project
 * Code-stop date: 04/22/2015
 * 
 * grid class .cs file
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Thief_Escape
{

    [Serializable]

    public class Grid
    {
        #region [ Enums ]

        //Direction Enum for simple switching
        public enum Direction
        {
            NORTH,
            SOUTH,
            EAST,
            WEST
        }


        //Map list Enum for grid creation
        public enum MapFiles
        {
			Test1,
			Test2,
            map1,
			map2,
			map3,
            End

        }


        #endregion


        #region [ Fields ]

        // The Grid, Declared
        private Cell[,] _map;

        //The Grid's size
        private int _mapSize;

        //The Grid's file
        private MapFiles _file;

        // The Grid's starting cell
        private int _startingY;
        private int _startingX;


        #endregion


        #region [ Properties ]


        public int[] StartingCell
        {
            get
            {
                int[] coords = { _startingX, _startingY };
                return coords;
            }
        }

        public int MapSize
        {
            get { return _mapSize; }
        }

        public MapFiles MapFile
        {
            get { return _file; }
        }



        #endregion


        #region [ Constructors ]

        public Grid()
        {

        }

        public Grid(MapFiles file, string playerName)
        {
            //  Store the map file
            _file = file;

            //  Create the target path
            string target = @"SaveGames\" + playerName + @"\" + file.ToString() + @".txt";

            //  Read the file to a string array
            string[] line = File.ReadAllLines(target);
            string[][] fileLines = line.Select(l => l.Split(new[] { ',' })).ToArray();

            //  Read the mapsize at line 1
            _mapSize = int.Parse(fileLines[0][0].ToString());

            //  Read the starting coords at line 2
            _startingX = int.Parse(fileLines[1][0].ToString());
            _startingY = int.Parse(fileLines[1][1].ToString());

            //Instantiate the Grid
            _map = new Cell[MapSize, MapSize];

            //For loop to instantiate the array with cells
            for (int x = 0; x < _mapSize; x++)
            {
                for (int y = 0; y < _mapSize; y++)
                {
                    _map[x, y] = new Cell();
                }
            }

            //  For loop to iterate from line 3 of the file to the end, create cells 
            for (int i = 2; i < line.Count(); i++)
            {
                //  Read x and y coords
                int x = int.Parse(fileLines[i][0]);
                int y = int.Parse(fileLines[i][1]);

                //  Figure out what type the line references
                Cell.Archetypes type;
                if (Enum.TryParse<Cell.Archetypes>(fileLines[i][2].ToString(), out type))
                {
                    //  Switch on archetype
                    switch (type)
                    {
                        case Cell.Archetypes.WALL:
                            //  Create the cell
                            _map[x, y].Archetype = type;

                            //  If there is something in the Item spot in the line
                            if (fileLines[i][3].ToString() != null & fileLines[i][3].ToString() != "")
                            {
                                //  Read the item type and assign it
                                Cell.Contents cont = (Cell.Contents)Enum.Parse(typeof(Cell.Contents), fileLines[i][3].ToString());
                                _map[x, y].Content = cont;
                            }

                            break;

                        case Cell.Archetypes.FLOOR:
                            //  Create the cell
                            _map[x, y].Archetype = type;

                            //  If there is something in the Item spot in the line
                            if (fileLines[i][3].ToString() != null & fileLines[i][3].ToString() != "")
                            {
                                //  Read the item type and assign it
                                Cell.Contents cont = (Cell.Contents)Enum.Parse(typeof(Cell.Contents), fileLines[i][3].ToString());
                                _map[x, y].Content = cont;
                            }

                            break;

                        case Cell.Archetypes.DOOR:
                            //  Create the Cell
                            _map[x, y].Archetype = type;

                            //  Set the LOCKED, UNLOCKED, or EXIT state
                            Cell.Modifiers mod = (Cell.Modifiers)Enum.Parse(typeof(Cell.Modifiers), fileLines[i][3].ToString());
                            _map[x, y].Modifier = mod;

                            break;

                        case Cell.Archetypes.STAIR:
                            //  Create the Cell
                            _map[x, y].Archetype = type;

                            //  Set the destination map
                            Grid.MapFiles map = (Grid.MapFiles)Enum.Parse(typeof(Grid.MapFiles), fileLines[i][3].ToString());
                            _map[x, y].Destination = map;

                            //  Set the destination coords
                            int[] coords = { int.Parse(fileLines[i][4]), int.Parse(fileLines[i][5]) };
                            _map[x, y].DestinationCoords = coords;

                            break;
                    }
                }
                else   //   Unable to read the enumeration
                {
                    throw new ArgumentException("Error occured during file reading, unable to process cell Archetype");
                }
            }
        }


        #endregion


        #region [ Methods ]


        //  Returns the Archetype of a cell
        public Cell.Archetypes CheckType(int x, int y)
        {
            Cell.Archetypes result = _map[x, y].Archetype;
            return result;
        }


        //  Checks for Items in a given cell
        public Cell.Contents CheckForItem(int x, int y)
        {
            Cell.Contents result = _map[x, y].Content;
            return result;
        }


        //  Removes Item's in a given cell, and passes back the item in the cell.
        public Cell.Contents RemoveItem(int x, int y)
        {
            Cell.Contents result = _map[x, y].Content;
            _map[x, y].Content = Cell.Contents.NULL;
            return result;
        }


        //  Checks if a door is unlocked
        public Cell.Modifiers CheckDoorModifier(int x, int y)
        {
            Cell.Modifiers result = _map[x, y].Modifier;
            return result;
        }


        //  Toggles Lock status on a door, and passes back new status
        public Cell.Modifiers ToggleDoorModifier(int x, int y)
        {
            if (_map[x, y].Modifier == Cell.Modifiers.LOCKED)
                _map[x, y].Modifier = Cell.Modifiers.UNLOCKED;
            else if (_map[x, y].Modifier == Cell.Modifiers.UNLOCKED)
                _map[x, y].Modifier = Cell.Modifiers.LOCKED;
            Cell.Modifiers result = _map[x, y].Modifier;
            return result;
        }

        //Returns the cell's destination map
        public Grid.MapFiles Destination(int x, int y)
        {
            Grid.MapFiles result;
            result = _map[x, y].Destination;
            return result;
        }

        //Returns the cell's destination coordinates
        public int[] DestinationCoords(int x, int y)
        {
            int[] result;
            result = _map[x, y].DestinationCoords;
            return result;
        }


        #endregion
    }
}
