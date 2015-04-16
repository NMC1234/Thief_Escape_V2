using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using System.Xml.Serialization;
using System.IO;

namespace Thief_Escape
{
    public partial class FrmGame : Form
    {
        #region [ Globals ]


        Player player;
        List<Item> Inventory;
        Item key, kitten;
        Grid cellGrid;
        string name;
        int gameClock;

        //Exit bool is used to prevent unwanted shutdowns 
        //  when going to the main menu.
        bool exit = true;


        #region [ Colors ]

        //  Define the colors for the cells
        Color wallColor = Color.DimGray;
        Color floorColor = Color.White;
        Color doorUnlockedColor = Color.Chocolate;
        Color doorLockedColor = Color.SaddleBrown;
        Color stairColor = Color.Yellow;
        Color kittenColor = Color.Purple;
        Color keyColor = Color.SpringGreen;

        // Define fog color
        Color fogColor = Color.Black;

        #endregion

        #endregion


		#region [ Fields/Properties ]
		//Strings used in saving a user's game
		private string saveTarget { get; set; }
		private string saveDirectory { get; set; }
		#endregion


		#region [ Constructors ]

		//Default constructor
        public FrmGame()
        {
            InitializeComponent();

            //Defaut constructor creates name of "User"
            name = "User";
        }


        // Overloaded FormGame constructor to pass in name from name form
        public FrmGame(string playername)
        {
            InitializeComponent();

            //Change the player name
            name = playername;
        }

        #endregion


        #region [ Form Events ]


        //  Form Closed
        private void FrmGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (exit)
                Application.Exit();

        }

        //  Arrow-Key Press handler
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //  Switch on Key press -- This can be used for other keys, currently only handles arrow keys.
            switch (keyData)
            {
                //Interact Button
                case Keys.Q:
                    btnInteract.PerformClick();
                    break;
                case Keys.NumPad7:
                    btnInteract.PerformClick();
                    break;
                case Keys.Space:
                    btnInteract.PerformClick();
                    break;

                //  Move west
                case Keys.Left:
                    btnMoveWest.PerformClick();
                    break;
                case Keys.A:
                    btnMoveWest.PerformClick();
                    break;
                case Keys.NumPad4:
                    btnMoveWest.PerformClick();
                    break;

                //  Move east
                case Keys.Right:
                    btnMoveEast.PerformClick();
                    break;
                case Keys.D:
                    btnMoveEast.PerformClick();
                    break;
                case Keys.NumPad6:
                    btnMoveEast.PerformClick();
                    break;

                //  Move north
                case Keys.Up:
                    btnMoveNorth.PerformClick();
                    break;
                case Keys.W:
                    btnMoveNorth.PerformClick();
                    break;
                case Keys.NumPad8:
                    btnMoveNorth.PerformClick();
                    break;

                //  Move south
                case Keys.Down:
                    btnMoveSouth.PerformClick();
                    break;
                case Keys.S:
                    btnMoveSouth.PerformClick();
                    break;
                case Keys.NumPad2:
                    btnMoveSouth.PerformClick();
                    break;

                //Use a Key
                //case Keys.Q:
                //	btnUseKey.PerformClick( );
                //	break;
                //case Keys.K:
                //	btnUseKey.PerformClick( );
                //	break;
                //case Keys.NumPad7:
                //	btnUseKey.PerformClick( );
                //	break;

                //Pickup A Key
                //case Keys.E:
                //	btnPickupKey.PerformClick( );
                //	break;
                //case Keys.P:
                //	btnPickupKey.PerformClick( );
                //	break;
                //case Keys.NumPad9:
                //	btnPickupKey.PerformClick( );
                //	break;

                //Pickup A Kitten
                //case Keys.R:
                //	btnPickupKitten.PerformClick( );
                //	break;
                //case Keys.O:
                //	btnPickupKitten.PerformClick( );
                //	break;
                //case Keys.NumPad3:
                //	btnPickupKitten.PerformClick( );
                //	break;

                //Use Stairs
                //case Keys.F:
                //	btnUseStairs.PerformClick();
                //	break;
                //case Keys.L:
                //	btnUseStairs.PerformClick( );
                //	break;
                //case Keys.NumPad1:
                //	btnUseStairs.PerformClick( );
                //	break;


                //  Do nothing
                default:
                    break;

            }
            //  No idea what this does, compiler error if removed.
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //  Form Loaded
        private void FrmGame_Load(object sender, EventArgs e)
        {
            //  Create the Player
            player = new Player(name);

            //  Create Items
            key = new Item(Item.ItemType.KEY);
            kitten = new Item(Item.ItemType.KITTEN);

            //  Create the Inventory
            Inventory = new List<Item>();

            //  Update Inventory Dialog
            UpdateInventory();

            //  Create the Grid
            cellGrid = new Grid(Grid.MapFiles.Test1, player.Name);

            //  Place the player
            player.SetLocation(cellGrid.StartingCell);
            player.CurrentMap = Grid.MapFiles.Test1;

            //  Make everything black
            InitialFog();

            //  Clear the fog around the player
            ViewArea();

            //  Check Valid Movements
            CheckValidMovements(player.XCoord, player.YCoord);

            //Check Valid Special Interactions
            CheckSpecialActions();

            //  Initial Prompt
            InitalPrompt();

            //Start Timmer
            tmrGameClock.Enabled = true;

        }

        //starting dialog
        public void InitalPrompt()
        {
            lstDialog.Items.Add(string.Format("Hello {0}, welcome to the game", name));
            lstDialog.Items.Add("Help Robbie get out of the house with all his treasures!");
            lstDialog.Items.Add("");
        }

        //Game Clock Tick Event
        private void tmrGameClock_Tick(object sender, EventArgs e)
        {
            gameClock++;
			player.GameClock = gameClock;
            lblGameClock.Text = Convert.ToString(gameClock);
        }

        #endregion


        #region [ Menu Button Clicks ]

        //  Load Button
        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            //message box to go along with loading of new game...
            DialogResult newDialog = MessageBox.Show("Your game is being loaded.", "LOADING...",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
        }


        //  Save Button
        private void btnSaveGame_Click(object sender, EventArgs e)
        {
            //message box to go along with saving of new game...
            DialogResult newDialog = MessageBox.Show("Are you sure you want to save your game?", "SAVING...",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (newDialog == DialogResult.OK)
            {
                SaveGame();
            }
        }


        //  Menu Button
        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            //Change the exit bool to false;
            exit = false;
            //Load the menu form
            FrmMain frm = new FrmMain();
            frm.Show();

            //Close this form
            this.Close();
        }


        #endregion


        #region [ Movement ]

        #region [ Movement Button Clicks ]

        //  North Button
        private void btnMoveNorth_Click(object sender, EventArgs e)
        {
            //  Move the player North ( y - 1 )
            PlayerMovement(Grid.Direction.NORTH);

        }

        //  East Button
        private void btnMoveEast_Click(object sender, EventArgs e)
        {
            //  Move the player East ( x + 1 )
            PlayerMovement(Grid.Direction.EAST);

        }

        // South Button
        private void btnMoveSouth_Click(object sender, EventArgs e)
        {
            //  Move the player South ( y + 1 )
            PlayerMovement(Grid.Direction.SOUTH);
        }

        //  West Button
        private void btnMoveWest_Click(object sender, EventArgs e)
        {
            //  Move the player West ( x - 1 )
            PlayerMovement(Grid.Direction.WEST);
        }

        #endregion

        #region [ Movement Methods ]

        //Enables or dissables buttons based on the cells surrounding the player
        public void CheckValidMovements(int x, int y)
        {

            //Check the cell north of the player. y - 1
            //  If the cell is a wall, disable movement
            if (cellGrid.CheckType(x, y - 1) == Cell.Archetypes.WALL)
            {
                btnMoveNorth.Enabled = false;
            }
            //  If the cell is a Door and is LOCKED, disable movement
            else if (cellGrid.CheckType(x, y - 1) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x, y - 1) == Cell.Modifiers.LOCKED)
            {
                btnMoveNorth.Enabled = false;
            }
            //  Everything else can be moved into
            else
            {
                btnMoveNorth.Enabled = true;
            }

            //Check the cell south of the player. y + 1
            //  If the cell is a wall, disable movement
            if (cellGrid.CheckType(x, y + 1) == Cell.Archetypes.WALL)
            {
                btnMoveSouth.Enabled = false;
            }
            //  If the cell is a Door and is LOCKED, disable movement
            else if (cellGrid.CheckType(x, y + 1) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x, y + 1) == Cell.Modifiers.LOCKED)
            {
                btnMoveSouth.Enabled = false;
            }
            //  Everything else can be moved into
            else
            {
                btnMoveSouth.Enabled = true;
            }
            //Check the cell west of the player. x - 1
            //  If the cell is a wall, disable movement
            if (cellGrid.CheckType(x - 1, y) == Cell.Archetypes.WALL)
            {
                btnMoveWest.Enabled = false;
            }
            //  If the cell is a Door and is LOCKED, disable movement
            else if (cellGrid.CheckType(x - 1, y) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x - 1, y) == Cell.Modifiers.LOCKED)
            {
                btnMoveWest.Enabled = false;
            }
            //  Everything else can be moved into
            else
            {
                btnMoveWest.Enabled = true;
            }

            //Check the cell east of the player. x + 1
            //  If the cell is a wall, disable movement
            if (cellGrid.CheckType(x + 1, y) == Cell.Archetypes.WALL)
            {
                btnMoveEast.Enabled = false;
            }
            //  If the cell is a Door and is LOCKED, disable movement
            else if (cellGrid.CheckType(x + 1, y) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x + 1, y) == Cell.Modifiers.LOCKED)
            {
                btnMoveEast.Enabled = false;
            }
            //  Everything else can be moved into
            else
            {
                btnMoveEast.Enabled = true;
            }
        }

        //Checks for any Special Action Available
        internal int CheckSpecialActions()
        {
            //check if there is a key or a kitten in current cell
            int[] keyDetails = CheckForNearbyKey();
            int[] kittenDetails = CheckForNearbyKitten();

            /* 
             * 0 = Nothing
             * 1 = Pickup Key
             * 2 = Use Key
             * 3 = Pickup Kitten
             * 4 = Both Key and Kitten
             * 5 = Use Stairs
             */
            int interactionType = 0;
            bool interactionFound = false;

            //Checks if special actions concerns an item
            #region [ Special Item Action ]
            if ((keyDetails[0] != 0) | (kittenDetails[0] != 0))
            {

                //Get Item Type and Enable or Disable Pickup button
                if ((keyDetails[0] != 0) & (kittenDetails[0] != 0))
                {
                    //Set interactionType to a Key and a Kitten Pickup
                    interactionType = 4;
                    interactionFound = true;
                }
                else if (keyDetails[0] != 0)
                {
                    //Set interactionType to a Key Pickup
                    interactionType = 1;
                    interactionFound = true;
                }
                else if (kittenDetails[0] != 0)
                {
                    //Set interactionType to a Kitten Pickup
                    interactionType = 3;
                    interactionFound = true;
                }
            }
            #endregion

            //Checks if special actions concerns Unlocking a Door
            #region [ Special Action Door ]
            //Checks if a Door Should be Opened
            //First checks if the player has a key in its inventory
            else if ((!interactionFound) && (Inventory.Contains<Item>(key)))
            {
                //First checks if the player has a key in its inventory

                //Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
                int x = player.XCoord - 1;
                int y = player.YCoord - 1;


                //Goes through each "column" of the search area
                for (int ix = 0; ix < 3; ix++)
                {
                    //Goes through each "row" of the column
                    for (int iy = 0; iy < 3; iy++)
                    {
                        //If the cell is a door, check if its locked.
                        if (cellGrid.CheckType((x + ix), (y + iy)) == Cell.Archetypes.DOOR)
                        {
                            //  Search found a locked door.
                            if (cellGrid.CheckDoorModifier((x + ix), (y + iy)) == Cell.Modifiers.LOCKED)
                            {
                                //Set interactionType to a Key being Used
                                interactionType = 2;
                                interactionFound = true;
                            }
                        }
                    }
                }

            }
            #endregion

            //Checks if special actions concerns Stairs
            #region [ Special Action Stairs ]
            //checks if the player is standing on stairs
            else if ((!interactionFound) && (cellGrid.CheckType(player.XCoord, player.YCoord) == Cell.Archetypes.STAIR))
            {
                //Set interactionType to use Stairs
                interactionType = 5;
                interactionFound = true;
            }
            #endregion

            //If Interaction found returns the coresponding int type
            if (interactionFound)
            {
                btnInteract.BackColor = Color.OliveDrab;
                btnInteract.Enabled = true;
                return interactionType;
            }
            else
            {
                //if no interaction found then returns 0
                btnInteract.BackColor = Color.Firebrick;
                btnInteract.Enabled = false;
                return 0;
            }

        }

        //  Handles all of the player movements - passed in the direction.
        internal void PlayerMovement(Grid.Direction direction)
        {
            //Move the player
            switch (direction)
            {
                case Grid.Direction.NORTH:
                    player.YCoord--;
                    break;
                case Grid.Direction.SOUTH:
                    player.YCoord++;
                    break;
                case Grid.Direction.EAST:
                    player.XCoord++;
                    break;
                case Grid.Direction.WEST:
                    player.XCoord--;
                    break;
            }
            //Clear the fog
            ViewArea();

            //Update the fog
            UpdateFog(direction);

            //Re-validate valid movements.
            CheckValidMovements(player.XCoord, player.YCoord);

            //Check Valid Special Interactions
            CheckSpecialActions();

            //Check Valid Special Interactions
            CheckSpecialActions();
        }

        #endregion

        #endregion


        #region [ Actions & Methods ]

        #region [ Action Button Clicks ]

        //Interact Button
        private void btnInteract_Click(object sender, EventArgs e)
        {

            /*
             * Switch between Item Special Actions
             * 0 = Nothing
             * 1 = Pickup Key
             * 2 = Use Key
             * 3 = Pickup Kitten
             * 4 = Both Key and Kitten
             * 5 = Use Stairs
             */
            switch (CheckSpecialActions())
            {
                //Nothing
                case 0:
                    MessageBox.Show("There is nothing to Interact With", "Nothing Around!");
                    break;

                //Pickup Key
                case 1:
                    PickupKey();
                    break;

                //Use Key
                case 2:
                    UseKey();
                    break;

                //Pickup Kitten
                case 3:
                    PickupKitten();
                    break;

                //Pickup both Key and Kitten
                case 4:
                    PickupKey();
                    PickupKitten();
                    break;

                //Use Stairs
                case 5:
                    UseStairs();
                    break;

                //Default
                default:
                    MessageBox.Show("There is nothing to Interact With", "Nothing Around!");
                    break;
            }
        }


        // removed Use/Pick-up Key, kitten, and Use Stairs Buttons
        //private void btnPickupKey_Click(object sender, EventArgs e)
        //{
        //	int[ ] keyDetails = CheckForNearbyKey( );
        //	if(keyDetails[0] != 0)
        //	{
        //		//  Remove key from grid.
        //		cellGrid.RemoveItem(keyDetails[1], keyDetails[2]);

        //		//  Refresh grid.
        //		ViewArea( );

        //		//  Add a key to the inventory
        //		Inventory.Add(key);
        //		UpdateInventory( );

        //		//  Tell user they have picked up a key.
        //		lstDialog.Items.Add("I have found a Key, I can use this to open doors.");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}
        //	else
        //	{
        //		//  Tell user there is no key nearby.
        //		lstDialog.Items.Add("There is no key nearby!");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}

        //	//  Update
        //	UpdateInventory( );
        //}



        //private void btnPickupKitten_Click(object sender, EventArgs e)
        //{
        //	int[] kittenDetails = CheckForNearbyKitten();

        //	if (kittenDetails[0] != 0)
        //	{
        //		//  Remove kitten from grid.
        //		cellGrid.RemoveItem(kittenDetails[1], kittenDetails[2]);

        //		//  Refresh grid.
        //		ViewArea();

        //		//  Add a kitten to the inventory
        //		Inventory.Add(kitten);
        //		UpdateInventory();

        //		//  Tell user they have picked up a kitten.
        //		lstDialog.Items.Add("I found a kitten!");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}
        //	else
        //	{
        //		//  Tell user there is no kitten nearby.
        //		lstDialog.Items.Add("There is no kitten nearby!");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}

        //	//  Update
        //	UpdateInventory();
        //}

        // Use Key Button
        //private void btnUseKey_Click(object sender, EventArgs e)
        //{
        //	bool foundDoor = false;

        //	//First checks if the player has a key in its inventory
        //	if (Inventory.Contains<Item>(key))
        //	{
        //		//Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
        //		int x = player.XCoord - 1;
        //		int y = player.YCoord - 1;


        //		//Goes through each "column" of the search area
        //		for (int ix = 0; ix < 3; ix++)
        //		{
        //			//Goes through each "row" of the column
        //			for (int iy = 0; iy < 3; iy++)
        //			{
        //				//If the cell is a door, check if its locked.
        //				if (cellGrid.CheckType((x + ix), (y + iy)) == Cell.Archetypes.DOOR)
        //				{
        //					foundDoor = true;

        //					//  Search found a locked door.
        //					if (cellGrid.CheckDoorModifier((x + ix), (y + iy)) == Cell.Modifiers.LOCKED)
        //					{
        //						Inventory.Remove(key);
        //						cellGrid.ToggleDoorModifier((x + ix), (y + iy));

        //						//Door is now unlocked
        //						lstDialog.Items.Add("This door is now unlocked.");
        //						lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //						lstDialog.SelectedIndex = -1;
        //					}

        //					else
        //					{
        //						//Door is already unlocked
        //						lstDialog.Items.Add("This door is already unlocked.");
        //						lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //						lstDialog.SelectedIndex = -1;
        //					}


        //				}
        //			}
        //		}
        //		//  Tell the user there is no door if the search didn't find one.
        //		if (!foundDoor)
        //		{
        //			lstDialog.Items.Add("I don't see a door near me.");
        //			lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //			lstDialog.SelectedIndex = -1;
        //		}
        //	}
        //	else if (!(Inventory.Contains(key)))
        //	{
        //		//  Player does not have key.
        //		lstDialog.Items.Add("I don't have a key to use.");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}

        //	//Refresh the map
        //	ViewArea();

        //	//Revalidate movements
        //	CheckValidMovements(player.XCoord, player.YCoord);

        //	//Update Inventory
        //	UpdateInventory();
        //}


        //  Use Stairs Button


        //private void btnUseStairs_Click(object sender, EventArgs e)
        //{
        //	//Check to see if the player is currently standing on stairs.
        //	if (cellGrid.CheckType(player.XCoord,player.YCoord) == Cell.Archetypes.STAIR)
        //	{
        //		// Get the stairs destination
        //		Grid.MapFiles destination = cellGrid.Destination(player.XCoord, player.YCoord);
        //		int[] coords = cellGrid.DestinationCoords(player.XCoord, player.YCoord);

        //		//Fog everything
        //		InitialFog();

        //		//  Recreate the grid
        //		cellGrid = new Grid(destination);

        //		//Place the player
        //		player.XCoord = coords[0];
        //		player.YCoord = coords[1];

        //		// Refresh the view
        //		ViewArea();

        //		//Check new valid movements
        //		CheckValidMovements(player.XCoord,player.YCoord);

        //		//Tell the user they have used stairs.
        //		//  Tell user there not on the stairs
        //		lstDialog.Items.Add("Those where some tall stairs.");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}

        //	else
        //	{
        //		//  Tell user there not on the stairs
        //		lstDialog.Items.Add("I have to be on the stairs to use them.");
        //		lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
        //		lstDialog.SelectedIndex = -1;
        //	}
        //}
        #endregion

        #region [ Action Methods ]

        #region [ Special Action Methods ]
        //Pickup Kitten Method
        private void PickupKitten()
        {
            int[] kittenDetails = CheckForNearbyKitten();

            if (kittenDetails[0] != 0)
            {
                //  Remove kitten from grid.
                cellGrid.RemoveItem(kittenDetails[1], kittenDetails[2]);

                //  Refresh grid.
                ViewArea();

                //  Add a kitten to the inventory
                Inventory.Add(kitten);
                UpdateInventory();

                //  Tell user they have picked up a kitten.
                lstDialog.Items.Add("I found a kitten!");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }
            else
            {
                //  Tell user there is no kitten nearby.
                lstDialog.Items.Add("There is no kitten nearby!");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }

            //  Update
            UpdateInventory();
            CheckSpecialActions();
        }

        //Pickup Key Method
        private void PickupKey()
        {
            int[] keyDetails = CheckForNearbyKey();
            if (keyDetails[0] != 0)
            {
                //  Remove key from grid.
                cellGrid.RemoveItem(keyDetails[1], keyDetails[2]);

                //  Refresh grid.
                ViewArea();

                //  Add a key to the inventory
                Inventory.Add(key);
                UpdateInventory();

                //  Tell user they have picked up a key.
                lstDialog.Items.Add("I have found a Key, I can use this to open doors.");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }
            else
            {
                //  Tell user there is no key nearby.
                lstDialog.Items.Add("There is no key nearby!");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }

            //  Update
            UpdateInventory();
            CheckSpecialActions();
        }

        //Uses a Key to Unlock a Door Method
        private void UseKey()
        {
            bool foundDoor = false;

            //First checks if the player has a key in its inventory
            if (Inventory.Contains<Item>(key))
            {
                //Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
                int x = player.XCoord - 1;
                int y = player.YCoord - 1;


                //Goes through each "column" of the search area
                for (int ix = 0; ix < 3; ix++)
                {
                    //Goes through each "row" of the column
                    for (int iy = 0; iy < 3; iy++)
                    {
                        //If the cell is a door, check if its locked.
                        if (cellGrid.CheckType((x + ix), (y + iy)) == Cell.Archetypes.DOOR)
                        {
                            foundDoor = true;

                            //  Search found a locked door.
                            if (cellGrid.CheckDoorModifier((x + ix), (y + iy)) == Cell.Modifiers.LOCKED)
                            {
                                Inventory.Remove(key);
                                cellGrid.ToggleDoorModifier((x + ix), (y + iy));

                                //Door is now unlocked
                                lstDialog.Items.Add("This door is now unlocked.");
                                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                                lstDialog.SelectedIndex = -1;
                            }

                            else
                            {
                                //Door is already unlocked
                                lstDialog.Items.Add("This door is already unlocked.");
                                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                                lstDialog.SelectedIndex = -1;
                            }


                        }
                    }
                }
                //  Tell the user there is no door if the search didn't find one.
                if (!foundDoor)
                {
                    lstDialog.Items.Add("I don't see a door near me.");
                    lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                    lstDialog.SelectedIndex = -1;
                }
            }
            else if (!(Inventory.Contains(key)))
            {
                //  Player does not have key.
                lstDialog.Items.Add("I don't have a key to use.");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }

            //Refresh the map
            ViewArea();

            //Revalidate movements
            CheckValidMovements(player.XCoord, player.YCoord);

            //Check Valid Special Interactions
            CheckSpecialActions();

            //Update Inventory
            UpdateInventory();
            CheckSpecialActions();
        }

        //Method to use Stairs
        private void UseStairs()
        {
            //Check to see if the player is currently standing on stairs.
            if (cellGrid.CheckType(player.XCoord, player.YCoord) == Cell.Archetypes.STAIR)
            {
                // Get the stairs destination
                Grid.MapFiles destination = cellGrid.Destination(player.XCoord, player.YCoord);
                int[] coords = cellGrid.DestinationCoords(player.XCoord, player.YCoord);

                //Fog everything
                InitialFog();

                //  Recreate the grid
                cellGrid = new Grid(destination, player.Name);

                //Place the player
                player.XCoord = coords[0];
                player.YCoord = coords[1];

                // Refresh the view
                ViewArea();

                //Check new valid movements
                CheckValidMovements(player.XCoord, player.YCoord);

                //Check Valid Special Interactions
                CheckSpecialActions();

                //Tell the user they have used stairs.
                //  Tell user there not on the stairs
                lstDialog.Items.Add("Those where some tall stairs.");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }

            else
            {
                //  Tell user there not on the stairs
                lstDialog.Items.Add("I have to be on the stairs to use them.");
                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                lstDialog.SelectedIndex = -1;
            }
        }
        #endregion

        private int[] CheckForNearbyKey()
        {
            //The array defined as (bool,x-coord,y-coord). Bool is 0-false 1-true, with default of false.
            int[] result = { 0, 0, 0 };

            //Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
            int x = player.XCoord - 1;
            int y = player.YCoord - 1;

            //Goes through each "column" of the search area
            for (int ix = 0; ix < 3; ix++)
            {
                //Goes through each "row" of the column
                for (int iy = 0; iy < 3; iy++)
                {
                    //If the cell has a key, return true.
                    if (cellGrid.CheckForItem((x + ix), (y + iy)) == Cell.Contents.KEY)
                    {
                        //Bool true
                        result[0] = 1;
                        //Key's x-coord
                        result[1] = (x + ix);
                        //Key's y-coord
                        result[2] = (y + iy);
                    }
                }
            }

            return result;

        }

        private int[] CheckForNearbyKitten()
        {
            //The array defined as (bool,x-coord,y-coord). Bool is 0-false 1-true, with default of false.
            int[] result = { 0, 0, 0 };

            //Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
            int x = player.XCoord - 1;
            int y = player.YCoord - 1;

            //Goes through each "column" of the search area
            for (int ix = 0; ix < 3; ix++)
            {
                //Goes through each "row" of the column
                for (int iy = 0; iy < 3; iy++)
                {
                    //If the cell has a kitten, return true.
                    if (cellGrid.CheckForItem((x + ix), (y + iy)) == Cell.Contents.KITTEN)
                    {
                        //Bool true
                        result[0] = 1;
                        //Kitten's x-coord
                        result[1] = (x + ix);
                        //Kitten's y-coord
                        result[2] = (y + iy);
                    }
                }
            }

            return result;

        }

        private void UpdateInventory()
        {
            //  Clear the current inventory dialog
            lstInventory.Items.Clear();

            // If the inventory is not empty
            if (Inventory.Count != 0)
            {
                //  Return the string of each item in the inventory 
                foreach (Item inv in Inventory)
                {
                    lstInventory.Items.Add(inv.ToString());
                    lstInventory.Items.Add(" ");

                }
            }
            else
            {
                //Dialog message for empty inventory.
                lstInventory.Items.Add("I've got some mighty fine lint in my pocket.");
            }
        }

        #endregion

        #endregion


        #region [ Image Map ]


        //Clears the fog and refreshes colors in a 5x5 square, centered on the players location
        public void ViewArea()
        {

            //  Get lower and upper Grid Bounds of the Clear, with validations to limit values.
            #region [ Bounds ]
            //      Northern Bound ( y - 2 )
            int lowerY = player.YCoord - 2;
            if (lowerY < 0)
                lowerY = 0;

            //      Southern Bound ( y + 2 )
            int upperY = player.YCoord + 2;
            if (upperY > cellGrid.MapSize - 1)
                upperY = cellGrid.MapSize - 1;

            //      Eastern Bound ( x + 2 )
            int upperX = player.XCoord + 2;
            if (upperX > cellGrid.MapSize - 1)
                upperX = cellGrid.MapSize - 1;

            //      Western Bound ( x - 2 )
            int lowerX = player.XCoord - 2;
            if (lowerX < 0)
                lowerX = 0;
            #endregion

            //  NOTE :: The Grid and the Map have flipped X and Y values, as well as the Map being indented X+1 and Y+1 from the Grid.

            //  For-loop through each 'column' of the Grid ( 'row' of the Map), starting at the lowerX bound and stopping at the upperX bound
            for (int x = lowerX; x <= upperX; x++)
            {
                //  For-loop through each 'row' of the Grid ( 'column' of the Map), starting at the lowerY bound and stopping at the upperY bound
                for (int y = lowerY; y <= upperY; y++)
                {
                    //Clear the face from the cell
                    grdconMap[y + 1, x + 1].Text = "";

                    //  Get the Archetype of the cell
                    Cell.Archetypes type = cellGrid.CheckType(x, y);

                    //  Switch on the Archetype - Get details as needed to continue switching.
                    switch (type)
                    {
                        // Walls - can contain Items
                        #region  [ Wall Switching ]


                        case Cell.Archetypes.WALL:
                            {
                                //Get contents
                                Cell.Contents cont = cellGrid.CheckForItem(x, y);

                                //  Switch on Contents
                                switch (cont)
                                {
                                    case Cell.Contents.NULL:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = wallColor;
                                        }
                                        break;
                                    case Cell.Contents.KEY:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = keyColor;
                                        }
                                        break;
                                    case Cell.Contents.KITTEN:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = kittenColor;
                                        }
                                        break;
                                }

                            }
                            break;


                        #endregion


                        //  Floors - can contain Items
                        #region [ Floor Switching ]
                        case Cell.Archetypes.FLOOR:
                            {
                                //Get contents
                                Cell.Contents cont = cellGrid.CheckForItem(x, y);

                                //Switch on Contents
                                switch (cont)
                                {
                                    case Cell.Contents.NULL:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = floorColor;
                                        }
                                        break;
                                    case Cell.Contents.KEY:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = keyColor;
                                        }
                                        break;
                                    case Cell.Contents.KITTEN:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = kittenColor;
                                        }
                                        break;
                                }

                            }
                            break;
                        #endregion


                        //  Doors - have a modifier
                        #region [ Door Switching ]
                        case Cell.Archetypes.DOOR:
                            {
                                //Get Modifier
                                Cell.Modifiers mod = cellGrid.CheckDoorModifier(x, y);

                                //Switch on Modifier
                                switch (mod)
                                {
                                    case Cell.Modifiers.LOCKED:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = doorLockedColor;
                                        }
                                        break;
                                    case Cell.Modifiers.UNLOCKED:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = doorUnlockedColor;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        #endregion


                        //  Stairs - have no other values
                        case Cell.Archetypes.STAIR:
                            {
                                grdconMap[y + 1, x + 1].BackColor = stairColor;
                            }
                            break;

                    }
                }
            }


            //  Set the player cell text
            grdconMap[player.YCoord + 1, player.XCoord + 1].Text = "☺";

        }


        //Simply sets everything in the map to black
        public void InitialFog()
        {
            //Get the max range of the cellGrid
            int maxRange = cellGrid.MapSize + 1;


            //For loop to process each cell in the cellGrid and set the color appropriately.
            for (int x = 1; x < maxRange; x++)
            {
                for (int y = 1; y < maxRange; y++)
                {
                    grdconMap[y, x].BackColor = fogColor;
                }
            }
        }


        //Return the fog to the area no longer revealed. - CALL AFTER THE PLAYER HAS BEEN MOVED
        internal void UpdateFog(Grid.Direction direction)
        {
            //Get the players new position
            int x = player.XCoord;
            int y = player.YCoord;

            switch (direction)
            {
                #region [ North ]
                case Grid.Direction.NORTH:
                    {
                        //Gets the lower and upper X bounds for the fog, validating for grid index.
                        //  lower X
                        int lowerX = player.XCoord - 2;
                        if (lowerX < 0)
                            lowerX = 0;
                        //  upper X
                        int upperX = player.XCoord + 2;
                        if (upperX > cellGrid.MapSize - 1)
                            upperX = cellGrid.MapSize - 1;

                        //Gets the Y coord for the line, breaking if coord is out of index
                        int YLine = player.YCoord + 3;

                        if (YLine < 0 | YLine > (cellGrid.MapSize - 1))
                        {
                            break;
                        }

                        //For-loops through each cell, turning it black.
                        for (int i = lowerX; i <= upperX; i++)
                        {
                            grdconMap[YLine + 1, i + 1].BackColor = fogColor;
                        }
                    }
                    break;
                #endregion


                #region [ South ]
                case Grid.Direction.SOUTH:
                    {
                        //Gets the lower and upper X bounds for the fog, validating for grid index.
                        //  lower X
                        int lowerX = player.XCoord - 2;
                        if (lowerX < 0)
                            lowerX = 0;
                        //  upper X
                        int upperX = player.XCoord + 2;
                        if (upperX > cellGrid.MapSize - 1)
                            upperX = cellGrid.MapSize - 1;

                        //Gets the Y coord for the line, breaking if coord is out of index
                        int YLine = player.YCoord - 3;

                        if (YLine < 0 | YLine > (cellGrid.MapSize - 1))
                        {
                            break;
                        }

                        //For-loops through each cell, turning it black.
                        for (int i = lowerX; i <= upperX; i++)
                        {
                            grdconMap[YLine + 1, i + 1].BackColor = fogColor;
                        }
                    }
                    break;
                #endregion


                #region [ East ]
                case Grid.Direction.EAST:
                    {
                        //Gets the lower and upper Y bounds for the fog, validating for grid index.
                        //  lower Y
                        int lowerY = player.YCoord - 2;
                        if (lowerY < 0)
                            lowerY = 0;
                        //  upper Y
                        int upperY = player.YCoord + 2;
                        if (upperY > cellGrid.MapSize - 1)
                            upperY = cellGrid.MapSize - 1;

                        //Gets the X coord for the line, breaking if coord is out of index
                        int XLine = player.XCoord - 3;

                        if (XLine < 0 | XLine > (cellGrid.MapSize - 1))
                        {
                            break;
                        }

                        //For-loops through each cell, turning it black.
                        for (int i = lowerY; i <= upperY; i++)
                        {
                            grdconMap[i + 1, XLine + 1].BackColor = fogColor;
                        }
                    }
                    break;
                #endregion


                #region [ West ]
                case Grid.Direction.WEST:
                    {
                        //Gets the lower and upper Y bounds for the fog, validating for grid index.
                        //  lower Y
                        int lowerY = player.YCoord - 2;
                        if (lowerY < 0)
                            lowerY = 0;
                        //  upper Y
                        int upperY = player.YCoord + 2;
                        if (upperY > cellGrid.MapSize - 1)
                            upperY = cellGrid.MapSize - 1;

                        //Gets the X coord for the line, breaking if coord is out of index
                        int XLine = player.XCoord + 3;

                        if (XLine < 0 | XLine > (cellGrid.MapSize - 1))
                        {
                            break;
                        }

                        //For-loops through each cell, turning it black.
                        for (int i = lowerY; i <= upperY; i++)
                        {
                            grdconMap[i + 1, XLine + 1].BackColor = fogColor;
                        }
                    }
                    break;
                #endregion
            }
        }

        #endregion


        #region [ File Interaction Methods ]

        //  Saves the GAME to the SaveGames Directory - for when the player saves the game before quiting
		//Must currently go through SaveGame to get to SaveRoom!
        public void SaveGame()
        {
			//Temp used vars for save path creation
			string savePath;
			bool saveCancel = false;

			//Default values/settings are assigned
			saveTarget = ".txt";
			saveDirectory = Directory.GetCurrentDirectory( );
			saveFD.InitialDirectory = saveDirectory;
			saveFD.FileName = "SaveGames";
			saveFD.Title = "Select Where You Want To Save The Game.  Default Save Folder is 'SaveGames'.";
			saveFD.AddExtension = false;
			saveFD.CheckFileExists = false;
			saveFD.CheckPathExists = false;
			saveFD.CreatePrompt = false;
			saveFD.OverwritePrompt = false;

			//Shows the file dialog window to let user select where to save game
			if(saveFD.ShowDialog( ) == DialogResult.OK)
			{
				//Selected path is set upon accept
				saveDirectory = saveFD.FileName;
				saveCancel = false;
			}
			else
			{
				//if no path was given then default is used
				saveDirectory = Directory.GetCurrentDirectory( );
				saveCancel = true;
			}

			//checks if save was cancelled
			if(!saveCancel)
			{

				//  First saves the current room - Assumes all other rooms have already been saved
				SaveRoom( );

				//  Creates the saveTarget string for the player.txt file
				saveTarget = ".txt";
				saveTarget = player.Name + saveTarget;

				//  Determine if the player's save folder exists
				savePath = string.Format(saveDirectory + "\\" + player.Name + "\\");
				if(!System.IO.Directory.Exists(savePath)) //  If the folder doesn't exist create it
				{
					System.IO.Directory.CreateDirectory(savePath);
				}

				//  Create list to hold strings
				List<String> fileStrings = new List<string>( );

				//  Store the player's Name - Not sure if this is necessary because its the name of the text file
				fileStrings.Add(player.Name);

				//  Store the player's current coordinates
				fileStrings.Add(string.Format(player.XCoord.ToString( ) + "," + player.YCoord.ToString( ) + ","));

				//  Store the player's current room
				fileStrings.Add(string.Format(player.CurrentMap.ToString( )));

				//  Add the saveTarget file to the end of the directory string
				savePath = savePath + saveTarget;


				//  save filestrings to file.
				using(System.IO.StreamWriter file = new System.IO.StreamWriter(savePath))
				{
					foreach(string line in fileStrings)
					{
						file.WriteLine(line);
					}
				}

				MessageBox.Show("Your game has been saved!", "SUCCESS!");
			}
			else
			{
				MessageBox.Show("Cancelled Save Operation", "CANCELLED");
			}
        }

        //  Saves the ROOM to the SaveGames Directory - for when the player transitions between rooms
        public void SaveRoom()
        {
			string savePath;
            //  Determine which room is currently open
            saveTarget = cellGrid.File.ToString() + saveTarget;

            //  Determine if the player's save folder exists
			savePath = string.Format(saveDirectory + "\\" + player.Name + "\\");
			//  If the folder doesn't exist create it
			if(!System.IO.Directory.Exists(savePath)) 
            {
				System.IO.Directory.CreateDirectory(savePath);
            }

            //  Create list to hold strings
            List<String> fileStrings = new List<string>();

            //  Store the mapsize
            string size = cellGrid.MapSize.ToString();
            fileStrings.Add(size);

            //  Store the starting cell
            string start = string.Format(cellGrid.StartingCell[0] + "," + cellGrid.StartingCell[1] + ",start,");
            fileStrings.Add(start);

            //  Iterate through the grid
            for (int y = 0; y < cellGrid.MapSize; y++)
            {
                for (int x = 0; x < cellGrid.MapSize; x++)
                {
                    //  First add the coords in x,y, format
                    string input = string.Format(x + "," + y + ",");


                    //  Add the cell type to the string
                    input = string.Format(input + cellGrid.CheckType(x, y).ToString() + ",");

                    switch (cellGrid.CheckType(x, y))
                    {
                        case Cell.Archetypes.WALL:

                            //  If the cell contains an item, add it to the string
                            if (cellGrid.CheckForItem(x, y) != Cell.Contents.NULL)
                            {
                                input = string.Format(input + cellGrid.CheckForItem(x, y).ToString() + ",");
                            }

                            break;

                        case Cell.Archetypes.FLOOR:

                            //  If the cell contains an item, add it to the string
                            if (cellGrid.CheckForItem(x, y) != Cell.Contents.NULL)
                            {
                                input = string.Format(input + cellGrid.CheckForItem(x, y).ToString() + ",");
                            }

                            break;

                        case Cell.Archetypes.DOOR:

                            //  Add the door modifier to the string
                            input = string.Format(input + cellGrid.CheckDoorModifier(x, y).ToString() + ",");

                            break;

                        case Cell.Archetypes.STAIR:

                            //  Add the destination file to the string
                            input = string.Format(input + cellGrid.Destination(x, y).ToString() + ",");

                            //  Add the destination coords to the string in x,y, format
                            input = string.Format(input + cellGrid.DestinationCoords(x, y)[0] + "," + cellGrid.DestinationCoords(x, y)[1] + ",");

                            break;
                    }

                    //  Add the input to fileStrings
                    fileStrings.Add(input);
                }
            }
            //  Add the saveTarget file to the end of the directory string
			savePath = savePath + saveTarget;


            //  save filestrings to file.
			using(System.IO.StreamWriter file = new System.IO.StreamWriter(savePath))
            {
                foreach (string line in fileStrings)
                {
                    file.WriteLine(line);
                }
            }
        }

        #endregion
    }
}
