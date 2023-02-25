
// View of the Snake Game
// This class will create the player's view like GUI for the gameand the players' names and scores
// Also it will draw the GUI controls, such as text boxes and buttons; Also register basic
// event handlers for user inputs and controller events.
// University of Utah
using System.Net;
using System.Net.Sockets;

namespace SnakeGame;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SnakeGame;

/// <summary>
/// This class is view, which will be informed to invalidate the view by the method in game controller
/// </summary>
public partial class MainPage : ContentPage
{
    // Fields property
    private String name;
    private int playerId;
    private int size;
    private World theWorld;
    public delegate void GameUpdateHandler();
    private GameController gameController;
    
    /// <summary>
    /// Constructor sets up the components and call the game Controller to register handlers.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
        graphicsView.Invalidate();
        gameController = new GameController();
        // these call the event Handler in the game Controller
        gameController.registerConnectInfoHandler(setUpRegisterInformation);
        gameController.registerRefreshevent(OnFrame);
        gameController.RegisterNetworkErrorHandler(NetworkErrorHandler);
        gameController.RegisterConnectErrorHanler(connectErrorHandler);
        gameController.RegisterSuccessConnectHandler(connectSuccessHandler);
    }
    /// <summary>
    /// This method sets up the register information by the given world, name, size, playerid
    /// </summary>
    /// <param name="world"></param>
    /// <param name="name"></param>
    /// <param name="size"></param>
    /// <param name="playerId"></param>
    public void setUpRegisterInformation(World world, String name, int size, int playerId)
    {
        this.theWorld = world;
        this.name = name;
        this.size = size;
        this.playerId = playerId;
        this.worldPanel.setWorld(this.theWorld);
        this.worldPanel.mainPlayerId = this.playerId;
    }

    /// <summary>
    ///   Element must be able to receive focus for this to work. Calling Focus on offscreen
    ///   or unrealized elements has undefined behavior.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnTapped(object sender, EventArgs args)
    {
        keyboardHack.Focus();
    }
    /// <summary>
    /// When the text Changed, call the method in the game controller that sends json to the server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnTextChanged(object sender, TextChangedEventArgs args)
    {
        Entry entry = (Entry)sender;
        String text = entry.Text.ToLower();
        // Send the key input'w' 'a' 's' 'd' from the user to the game Controller
        if (text == "w")
        {
            gameController.sendCommand("up");
        }
        else if (text == "a")
        {
            if (!this.theWorld.snakes.ContainsKey(this.playerId))
            {
                return;
            }  
            gameController.sendCommand("left");
        }
        else if (text == "s")
        {
            if (!this.theWorld.snakes.ContainsKey(this.playerId))
            {
                return;
            }
            gameController.sendCommand("down");
        }
        else if (text == "d")
        {
            if (!this.theWorld.snakes.ContainsKey(this.playerId))
            {
                return;
            }
            gameController.sendCommand("right");
        }
        entry.Text = "";
    }

    /// <summary>
    /// Network Error Handler which will display an alert message.
    /// </summary>
    private void NetworkErrorHandler()
    {
        Dispatcher.Dispatch(() => DisplayAlert("Error", "Disconnected from server", "OK"));
    }
    /// <summary>
    /// This method will be called when the connection success
    /// </summary>
    private void connectSuccessHandler()
    {
        // Set the name text, connect button and server text not able to be changed to clicked
        Dispatcher.Dispatch(() =>
        {
            nameText.IsEnabled = false;
            connectButton.IsEnabled = false;
            serverText.IsEnabled = false;
        });
    }

    /// <summary>
    /// Event handler for the connect button
    /// We will put the connection attempt interface here in the view.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ConnectClick(object sender, EventArgs args)
    {
        // If the text of the server is empty, show this message
        if (serverText.Text == "")
        {
            DisplayAlert("Error", "Please enter a server address", "OK");
            return;
        }
        // If the text of the name is empty, show this message
        if (nameText.Text == "")
        {
            DisplayAlert("Error", "Please enter a name", "OK");
            return;
        }
        // If the text of the name is longer than 16 chars, show this message
        if (nameText.Text.Length > 16)
        {
            DisplayAlert("Error", "Name must be less than 16 characters", "OK");
            return;
        }
        // else, set the name and call the connect button method in game controller.
       this.name = nameText.Text;
        String serverDomain = serverText.Text;
        gameController.setName(this.name);
        gameController.connectButton(serverDomain);
        keyboardHack.Focus();
    }
        /// <summary>
        /// Use this method as an event handler for when the controller has updated the world
        /// </summary>
        public void OnFrame()
    {
        Dispatcher.Dispatch( () => graphicsView.Invalidate() );
    }

    /// <summary>
    /// Connect Error Handler which will display an alert message, and then sets the connectButton, 
    /// serverText, and nameText to be enabled 
    /// </summary
    public void connectErrorHandler()
    {
        // Set the name text, connect button and server text be able to be changed to clicked
        // And also display a error message to user
        DisplayAlert("Error", "Unable to connect to server", "OK");
        Dispatcher.Dispatch(() =>
        {
            serverText.IsEnabled = true;
            nameText.IsEnabled = true;
            connectButton.IsEnabled = true;
        });
    }

    /// <summary>
    /// When this button is clicked, it will show the controls message with W, A, S, D, which
    /// is move up, left, down and right.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ControlsButton_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Controls",
                     "W:\t\t Move up\n" +
                     "A:\t\t Move left\n" +
                     "S:\t\t Move down\n" +
                     "D:\t\t Move right\n",
                     "OK");
    }

    /// <summary>
    /// When this button is clicked, it will show the controls message  witch the information of the game
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AboutButton_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("About",
      "SnakeGame solution\nArtwork by Jolie Uk and Alex Smith\nGame design by Daniel Kopta and Travis Martin\n" +
      "Implementation by Qiaoyi Cai and Yamin Zhuang\n" +
        "CS 3500 Fall 2022, University of Utah", "OK");
    }

    /// <summary>
    /// Element must be able to receive focus for this to work. Calling Focus on offscreen
    /// or unrealized elements has undefined behavior.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentPage_Focused(object sender, FocusEventArgs e)
    {
        if (!connectButton.IsEnabled)
            keyboardHack.Focus();
    }
}
