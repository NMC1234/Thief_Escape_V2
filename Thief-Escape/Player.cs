using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thief_Escape
{

    [Serializable]

    public class Player
    {
        #region [ Fields ]


        //Player's name
        string _name;

		//Game Clock
		int _gameClock;

        //Player's coords
        int _x;
        int _y;

        //Players current map
        Grid.MapFiles _currentMap;


        #endregion

        #region [ Properties ]


        //Gets or sets the player's name.
        public string Name 
        {
            get { return _name; }
            
            set { _name = value.Trim(); }
        }

		//Gets or sets the Player's Game Clock
		public int GameClock
		{
			get { return _gameClock; }

			set { 
				if(value >= 0)
					_gameClock = value; 
			}
		}

        //Gets or sets the player's X coordinate
        public int XCoord 
        {
            get { return _x; }

            set
            {
                //Restrict coord values to >= 0
                if (value >= 0)
                    _x = value;
            }
        }


        //Gets or sets the player's Y coordinate
        public int YCoord
        {
            get { return _y; }

            set
            {
                //Restrict coord values to >= 0
                if (value >= 0)
                    _y = value;
            }
        }


        //Gets an Array containing the X,Y coordinate pair
        public int[] Coordinates 
        { 
            get
            {
                int[] coords = {_x,_y};
                return coords;
            }
        }


        //Gets or sets the players current map
        public Grid.MapFiles CurrentMap 
        {
            get { return _currentMap; }
            set
            {
                _currentMap = value;
            }
        }


        #endregion

        #region [ Constructors ]

		//Default constructor needed for serialization
		public Player( )
		{

		}

		//Constructor with player name propertie
		public Player(string sentname)
		{
			_name = sentname;
		}

		//Constructor with both a name property and an x,y for the current cell
		public Player(string sentName, int x, int y)
		{
			_name = sentName;
			XCoord = x;
			YCoord = y;
		}

		//Constructor with a name property, GameClock, and an x,y for the current cell
		public Player(string sentName, int clock, int x, int y)
		{
			_name = sentName;
			_gameClock = clock;
			XCoord = x;
			YCoord = y;
		}
        #endregion

        #region [ Methods ]


        //  Sets the players location - separate coords
        public void SetLocation(int x, int y)
        {
            XCoord = x;
            YCoord = y;
        }


        //  Sets the players location - coord array
        public void SetLocation(int[] coords)
        {
            XCoord = coords[0];
            YCoord = coords[1];
        }


        #endregion
    }
}
