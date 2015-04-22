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

        //  The default map , used in the creation of new games.
        Grid.MapFiles defaultMap = Grid.MapFiles.map2;


        #region [ Colors ]

        //  Define the colors for the cells
        Color wallColor = Color.DimGray;
        Color floorColor = Color.White;
        Color doorUnlockedColor = Color.Chocolate;
        Color doorLockedColor = Color.SaddleBrown;
        Color doorExitColor = Color.Green;
        Color stairColor = Color.Yellow;
        Color kittenColor = Color.Purple;
        Color keyColor = Color.SpringGreen;

        // Define fog color
        Color fogColor = Color.Black;

        #endregion

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

        //  Form Closing
        private void FrmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Prompt user if they want to save game
            DialogResult newDialog = MessageBox.Show("Would you like to save the game before you leave?", "Save Game?",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (newDialog == DialogResult.Yes)
            {
                SaveGame();
            }

            if (newDialog == DialogResult.Cancel)
                e.Cancel = true;
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
            //  Create Items
            key = new Item(Item.ItemType.KEY);
            kitten = new Item(Item.ItemType.KITTEN);

            //  Create the Inventory
            Inventory = new List<Item>();

            //  Call the LoadGame method, passing in the player name
            //  This method will create the player, create the cellGrid, and populate the inventory
            LoadGame(name);

            //  Update Inventory Dialog
            UpdateInventory();

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


        #region [ Menu Button Clicks & Menu Tool Strip Clicks ]

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

        private void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //message box to go along with saving of new game...
            DialogResult newDialog = MessageBox.Show("Are you sure you want to save your game?", "SAVING...",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (newDialog == DialogResult.OK)
            {
                SaveGame();
            }
        }

        private void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Change the exit bool to false;
            exit = false;
            //Load the menu form
            FrmMain frm = new FrmMain();
            frm.Show();

            //Close this form
            this.Close();
        }

        private void exitGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
            //  If the cell is a Door and is NOT UNLOCKED, disable movement
            else if (cellGrid.CheckType(x, y - 1) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x, y - 1) != Cell.Modifiers.UNLOCKED)
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
            //  If the cell is a Door and is NOT UNLOCKED, disable movement
            else if (cellGrid.CheckType(x, y + 1) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x, y + 1) != Cell.Modifiers.UNLOCKED)
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
            //  If the cell is a Door and is NOT UNLOCKED, disable movement
            else if (cellGrid.CheckType(x - 1, y) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x - 1, y) != Cell.Modifiers.UNLOCKED)
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
            //  If the cell is a Door and is NOT UNLOCKED, disable movement
            else if (cellGrid.CheckType(x + 1, y) == Cell.Archetypes.DOOR & cellGrid.CheckDoorModifier(x + 1, y) != Cell.Modifiers.UNLOCKED)
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
             * 6 = Exit Door
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

            //Checks if special actions concerns Stairs
            #region [ Special Action Stairs ]
            //checks if the player is standing on stairs
            if ((!interactionFound) && (cellGrid.CheckType(player.XCoord, player.YCoord) == Cell.Archetypes.STAIR))
            {
                //Set interactionType to use Stairs
                interactionType = 5;
                //Change name of button
                btnInteract.Text = "Use the Stairs";
                interactionFound = true;
            }
            #endregion

            //Checks if special actions concerns Unlocking a Door
            #region [ Special Action Door ]
            //Checks if a Door Should be Opened
            //First checks if the player has a key in its inventory
            if ((!interactionFound) && (Inventory.Contains<Item>(key)))
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
                                //Change name of button
                                btnInteract.Text = "Open the Door";
                                interactionFound = true;
                            }
                        }
                    }
                }

            }
            #endregion

            //  Checks for the completion of the game
            #region [ Check for Completion Exit ]
            //Checks for exit door condition
            if (!interactionFound)  //  Haven't found anything else
            {
                //  Gets the number of Kittens in the inventory so it can be checked against
                int kittenCount = 0;
                foreach (var k in Inventory)
                {
                    if (k == kitten)
                        kittenCount++;
                }

                //  Only with 4 kittens can the player exit the game so only check then
                if (kittenCount == 4)
                {
                    //  Search for a door
                    //  Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
                    int x = player.XCoord - 1;
                    int y = player.YCoord - 1;


                    //  Goes through each "column" of the search area
                    for (int ix = 0; ix < 3; ix++)
                    {
                        //Goes through each "row" of the column
                        for (int iy = 0; iy < 3; iy++)
                        {
                            //If the cell is a door, check if its locked.
                            if (cellGrid.CheckType((x + ix), (y + iy)) == Cell.Archetypes.DOOR)
                            {
                                //  Search found a locked door.
                                if (cellGrid.CheckDoorModifier((x + ix), (y + iy)) == Cell.Modifiers.EXIT)
                                {
                                    //Set interactionType to a Key being Used
                                    interactionType = 6;
                                    //Change name of button
                                    btnInteract.Text = "Exit the Mansion";
                                    interactionFound = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    interactionFound = false;
                }
            }
            #endregion

            //If Interaction found returns the corresponding int type
            if (interactionFound)
            {
                btnInteract.BackColor = Color.YellowGreen;
                btnInteract.Enabled = true;
                return interactionType;
            }
            else
            {
                //if no interaction found then returns 0
                btnInteract.BackColor = Color.Crimson;
                btnInteract.Text = "Interact";
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
                    btnInteract.Text = "Interact";
                    break;

                //Use Key
                case 2:
                    UseKey();
                    btnInteract.Text = "Interact";
                    break;

                //Pickup Kitten
                case 3:
                    PickupKitten();
                    btnInteract.Text = "Interact";
                    break;

                //Pickup both Key and Kitten
                case 4:
                    PickupKey();
                    PickupKitten();
                    btnInteract.Text = "Interact";
                    break;

                //Use Stairs
                case 5:
                    UseStairs();
                    btnInteract.Text = "Interact";
                    break;

                //  Completion Exit
                case 6:
                    FinishGame();
                    btnInteract.Text = "Interact";
                    break;

                //Default
                default:
                    MessageBox.Show("There is nothing to Interact With", "Nothing Around!");
                    break;
            }
        }

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

                //  Save the current room
                SaveRoom();

                //  Recreate the grid
                cellGrid = new Grid(destination, player.Name);

                //  Update Player's current map
                player.CurrentMap = destination;

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

        //  Finish the Game
        private void FinishGame()
        {
            //  First, double check for completion requirements
            //  Gets the number of Kittens in the inventory so it can be checked against
            int kittenCount = 0;
            foreach (var k in Inventory)
            {
                if (k == kitten)
                    kittenCount++;
            }
            //  Only with 4 kittens can the player exit the game so only check then
            if (kittenCount == 4)
            {
                //  Search for a door
                //  Creates starting point for search, 1 cell up and 1 cell left, centered on the player.
                int x = player.XCoord - 1;
                int y = player.YCoord - 1;


                //  Goes through each "column" of the search area
                for (int ix = 0; ix < 3; ix++)
                {
                    //Goes through each "row" of the column
                    for (int iy = 0; iy < 3; iy++)
                    {
                        //If the cell is a door, check if its locked.
                        if (cellGrid.CheckType((x + ix), (y + iy)) == Cell.Archetypes.DOOR)
                        {
                            //  Search found an Exit door.
                            if (cellGrid.CheckDoorModifier((x + ix), (y + iy)) == Cell.Modifiers.EXIT)
                            {
                                //  Output necessary prompt
                                string output = "Robbie escaped the mansion with all 4 Jewel Encrusted Kittens!, Good Job.";
                                lstDialog.Items.Add(output);
                                lstDialog.SelectedIndex = lstDialog.Items.Count - 1;
                                lstDialog.SelectedIndex = -1;

                                //  Disable Movement Keys
                                btnMoveEast.Enabled = false;
                                btnMoveWest.Enabled = false;
                                btnMoveNorth.Enabled = false;
                                btnMoveSouth.Enabled = false;

                                //  Stop the timer
                                tmrGameClock.Enabled = false;

                                //  Fog Everything
                                InitialFog();

                                //  Load the "Good Game" map
                                cellGrid = new Grid(Grid.MapFiles.End, player.Name);

                                //  View Everything
                                ClearAllFog();

                            }
                        }
                    }
                }
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
                        //Change name of button
                        btnInteract.Text = "Pickup Key";
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
                        //Change name of button
                        btnInteract.Text = "Pickup Kitten";

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
                                    case Cell.Modifiers.EXIT:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = doorExitColor;
                                        }
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

        //Clears the fog and refreshes colors in a 5x5 square, centered on the players location
        public void ClearAllFog()
        {

            //  NOTE :: The Grid and the Map have flipped X and Y values, as well as the Map being indented X+1 and Y+1 from the Grid.

            //  For-loop through each 'column' of the Grid ( 'row' of the Map), starting at the lowerX bound and stopping at the upperX bound
            for (int x = 0; x < cellGrid.MapSize; x++)
            {
                //  For-loop through each 'row' of the Grid ( 'column' of the Map), starting at the lowerY bound and stopping at the upperY bound
                for (int y = 0; y < cellGrid.MapSize; y++)
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
                                    case Cell.Modifiers.EXIT:
                                        {
                                            grdconMap[y + 1, x + 1].BackColor = doorExitColor;
                                        }
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

        //  Saves the PLAYER and CURRENT ROOM to the SaveGames Directory - for when the player saves the game before quiting
        public void SaveGame()
        {
            //  Saves the current room, assumes all other rooms have already been saved
            SaveRoom();

            //  Creates the target string for the player.txt file
            string target = ".txt";
            target = player.Name + target;

            //  Determine if the player's save folder exists
            string directory = Directory.GetCurrentDirectory();
            directory = string.Format(directory + "\\SaveGames\\" + player.Name + "\\");
            if (!System.IO.Directory.Exists(directory)) //  If the folder doesn't exist create it
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            //  Create list to hold strings
            List<String> fileStrings = new List<string>();

            //  Store the player's Name - Not sure if this is necessary because its the name of the text file
            fileStrings.Add(player.Name);
            //  Store the player's current coordinates
            fileStrings.Add(string.Format(player.XCoord.ToString() + "," + player.YCoord.ToString() + ","));

            //  Store the player's current room
            fileStrings.Add(string.Format(player.CurrentMap.ToString()));

            //  Get the player's current count of keys and kittens
            int keyCount = 0;
            int kittenCount = 0;
            foreach (var item in Inventory)
            {
                if (item == key)
                    keyCount++;
                else if (item == kitten)
                    kittenCount++;
            }

            //  Store the player's current count of keys on line 4 and kittens on line 5
            fileStrings.Add(keyCount.ToString());
            fileStrings.Add(kittenCount.ToString());

            //  Store the player's timer
            fileStrings.Add(string.Format(player.GameClock.ToString()));

            //  Add the target file to the end of the directory string
            directory = directory + target;

            //  save filestrings to file.
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(directory))
            {
                foreach (string line in fileStrings)
                {
                    file.WriteLine(line);
                }
            }
        }

        //  Saves the  CURRENT ROOM to the SaveGames Directory - for when the player transitions between rooms
        public void SaveRoom()
        {
            //  Determine which room is currently open
            string target = ".txt";
            target = cellGrid.MapFile.ToString() + target;

            //  Determine if the player's save folder exists
            string directory = Directory.GetCurrentDirectory();
            directory = string.Format(directory + "\\SaveGames\\" + player.Name + "\\");
            if (!System.IO.Directory.Exists(directory)) //  If the folder doesn't exist create it
            {
                System.IO.Directory.CreateDirectory(directory);
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
            //  Add the target file to the end of the directory string
            directory = directory + target;


            //  save filestrings to file.
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(directory))
            {
                foreach (string line in fileStrings)
                {
                    file.WriteLine(line);
                }
            }
        }

        //  Loads the Game - Will call the NewGame method if the save file does not exist
        public void LoadGame(string playerName)
        {
            //  Checks if the SaveGames directory exists
            if (Directory.Exists(@"SaveGames"))
            {
                //  Checks if the Player's directory exists
                string target = string.Format(@"SaveGames\\" + playerName);
                if (Directory.Exists(target))
                {
                    #region [ Load Player ]
                    //  Load the "player.txt" file if it exists
                    target = string.Format(target + @"\\" + playerName + ".txt");
                    if (File.Exists(target))
                    {
                        string[] playerLines = File.ReadAllLines(target);
                        try
                        {
                            // Instantiate the player with the name from line 1 of the file
                            player = new Player(playerLines[0].ToString());

                            //  Get the coordinates from line 2 of the file
                            string[] playerCoords = playerLines[1].Split(',').ToArray<string>();
                            player.XCoord = int.Parse(playerCoords[0]);
                            player.YCoord = int.Parse(playerCoords[1]);

                            //  Get the current map from line 3 of the file
                            Grid.MapFiles map;
                            if (Enum.TryParse<Grid.MapFiles>(playerLines[2].ToString(), out map))
                            {
                                player.CurrentMap = map;
                            }
                            else
                                throw new ArgumentException();

                            //  Get the player's inventory, line 4 is KEYS and line 5 is KITTENS
                            int keyCount = int.Parse(playerLines[3]);
                            int kittenCount = int.Parse(playerLines[4]);
                            {
                                //  KEYS
                                for (int i = 0; i < keyCount; i++)
                                {
                                    Inventory.Add(key);
                                }

                                //  KITTENS
                                for (int i = 0; i < kittenCount; i++)
                                {
                                    Inventory.Add(kitten);
                                }
                            }

                            //  Get the player's time from line 6
                            int counter = int.Parse(playerLines[5]);
                            gameClock = counter;
                        }
                        catch
                        {
                            MessageBox.Show("Something went wrong during the loading process, the \"Player Name\".txt file may have been corrupted", "Loading Error");
                        }


                    }
                    else   //   If the file does not exist, then assume its broken and start a new game.
                    {
                        NewGame(playerName);
                    }
                    #endregion

                    #region [ Load Current Room ]

                    //  Get the room based on the player's current room
                    Grid.MapFiles currentMap = player.CurrentMap;

                    //  Calls the grid constructor based on that map
                    cellGrid = new Grid(currentMap, player.Name);

                    #endregion
                }
                else   //   If it does not exist, then call the NewGame method
                {
                    NewGame(playerName);
                }
            }
            else   //   If it does not exist, then call the NewGame method
            {
                NewGame(playerName);
            }
        }

        //  Creates a new game - called by the LoadGame method
        public void NewGame(string playerName)
        {
            //  Copies the contents of the DefaultMaps folder to the player's SaveGame folder
            string sourcePath = @"DefaultMaps";
            string targetPath = string.Format(@"SaveGames\" + playerName);

            if (!Directory.Exists(targetPath))  //  Creates the player folder if it doesn't exist
            {
                Directory.CreateDirectory(targetPath);
            }

            if (System.IO.Directory.Exists(sourcePath)) //  Checks to ensure the DefaultMaps folder exists
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);  //  Get an array of all files in default maps

                string fileName;
                string destFile;
                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else   //   DefaultMaps did not exist, return to main menu
            {
                MessageBox.Show(@"Unable to find the DefaultMaps directory. This folder should be in the same folder as Theif-Escape.exe. Without this folder the game cannot load.", "Critical Failure");
                FrmMain main = new FrmMain();
                main.Show();
                this.Close();
            }


            //  Load the starter room first
            //  Instantiates cellGrid using the default map.
            cellGrid = new Grid(defaultMap, playerName);

            //  Create the player next
            //  Instantiate player with name
            player = new Player(playerName);

            //  Sets player's current map to default map
            player.CurrentMap = defaultMap;

            //  Sets player's current coords to starter coords
            player.XCoord = cellGrid.StartingCell[0];
            player.YCoord = cellGrid.StartingCell[1];

            //  Sets player's gameclock to 0
            player.GameClock = 0;
        }
        #endregion

    }
}
