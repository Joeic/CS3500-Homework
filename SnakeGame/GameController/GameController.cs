// Author: Qiao Yi Cai and Yamin Zhuang, Nov 2022
// Game controller of the Snake Game
// This class is responsible for pasring information received from the NetworkController, and update the model.
// After updating the model, it should inform the View that the world has changed, so that it can redraw.
// University of Utah
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using NetworkUtil;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SnakeGame;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Net.Sockets;

namespace SnakeGame
{
    // Declare delegates which are used to make event
    public delegate void SuccessConnectHandler();
    public delegate void ConnectInfoHandler(World world, String name, int size, int playerId);
    public delegate void RefreshEvent();
    public delegate void NetworkErrorHandler();
    public delegate void ConnectErrorHanler();
    /// <summary>
    /// Constructor which initialize the fields and delcare events which are needed
    /// </summary>
    public class GameController
    {
        // private fields
        private Socket? socket;
        private String? name;
        private int playerId;
        private int size;
        private World? world;
        private string cmdMessage = "none";
        // Private events
        private event SuccessConnectHandler? successConnectHandler;
        private event ConnectInfoHandler? connectInfoHandler;
        private event RefreshEvent? refreshEvent;
        private event NetworkErrorHandler? networkErrorHandler;
        private event ConnectErrorHanler? connectErrorHanler;
        /// <summary>
        /// This method register the connect info handler
        /// </summary>
        /// <param name="c"></param>
        public void registerConnectInfoHandler(ConnectInfoHandler c)
        {
            this.connectInfoHandler = c;
        }
        /// <summary>
        /// This method register the refresh event handler
        /// </summary>
        /// <param name="r"></param>
        public void registerRefreshevent(RefreshEvent r)
        {
            this.refreshEvent += r;
        }
        /// <summary>
        /// This method register the success connect handler
        /// </summary>
        /// <param name="m"></param>
        public void RegisterSuccessConnectHandler(SuccessConnectHandler m)
        {
            this.successConnectHandler += m;
        }
        /// <summary>
        /// This method registers network error handler
        /// </summary>
        /// <param name="n"></param>
        public void RegisterNetworkErrorHandler(NetworkErrorHandler n)
        {
            this.networkErrorHandler += n;
        }
        /// <summary>
        /// This method registers connect error handler
        /// </summary>
        /// <param name="connectError"></param>
        public void RegisterConnectErrorHanler(ConnectErrorHanler connectError)
        {
            this.connectErrorHanler += connectError;
        }
        /// <summary>
        /// This method set the name of the player
        /// </summary>
        /// <param name="name"></param>
        public void setName(String name)
        {
            this.name = name;
        }
        /// <summary>
        /// This class is called when the connect button is clicked
        /// </summary>
        /// <param name="ipAddress"></param>
        public void connectButton(String ipAddress)
        {
            // Calls the connectToServer which begins the asynchronous process of connecting to a server
            Networking.ConnectToServer(RegisterInformation, ipAddress, 11000);

        }
        /// <summary>
        /// This class register information which is a ConnectedCallback as the method to finalize the connection once it's made.
        /// </summary>
        /// <param name="state"></param>
        public void RegisterInformation(SocketState state)
        {
            // if there is error in the SocketState, invoke the connect error handler
            if (state.ErrorOccurred)
            {
                connectErrorHanler?.Invoke();
            }
            // Else, success connect handler is invoked and receive data from the network.
            else
            {
                successConnectHandler?.Invoke();
                state.OnNetworkAction = ReceiveStartup;
                Networking.Send(state.TheSocket, this.name);
                Networking.GetData(state);
            }
        }
        /// <summary>
        /// This method is called when data is received or when a connection is made.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveStartup(SocketState state)
        {
            // if there is error in the SocketState, invoke the connect error handler
            if (state.ErrorOccurred)
            {
                networkErrorHandler?.Invoke();
                return;
            }
            // Else, get the socket in the socket state and get the data from the socket state.
            else
            {
                this.socket = state.TheSocket;
                String strBuilder = state.GetData();
                // Try split the elements by \n and parse the size.
                try
                {

                    String[] group = strBuilder.ToString().Split('\n');
                    playerId = Int32.Parse(group[0]);
                    size = Int32.Parse(group[1]);
                    // strBuilder.Remove(0, group[0].Length);
                    // strBuilder.Remove(0, group[1].Length);
                    state.RemoveData(0, group[0].Length);
                    state.RemoveData(0, group[1].Length);
                    state.OnNetworkAction = ReceiveWorld;

                }
                catch (Exception ex)
                {
                }
                this.world = new World(size);
                //this.worldPanel.setWorld(theWorld);
                //theWorld.mainPlayerId = playerId;
                world.mainPlayerId = playerId;
                // Invoke the connect info handler
                connectInfoHandler(this.world, this.name, this.size, this.playerId);
                Networking.GetData(state);
            }
        }
        /// <summary>
        /// This method is called when ReceiveStartup is made.
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveWorld(SocketState state)
        {
            // if there is error in the SocketState, invoke the connect error handler
            if (state.ErrorOccurred)
            {
                // Invoke the network error handler
                networkErrorHandler?.Invoke();
                return;
            }
            // Else, get the data which are unprocessed from the state and split by \n
            // Then, it will process all the objects in the world and it should inform
            // the View that the world has changed, so that it can redraw.
            else
            {
                string totalData = state.GetData();
                string[] parts = Regex.Split(totalData, @"(?<=[\n])");

                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                Wall? wall = null;
                Power? power = null;
                Snake? snake = null;
                lock (world)
                {
                    foreach (String part in parts)
                    {
                        if (part.Length == 0)
                            continue;
                        // The regex splitter will include the last string even if it doesn't end with a '\n',
                        // So we need to ignore it if this happens. 
                        if (part[part.Length - 1] != '\n')
                            break;
                        if (part[0] == '{' && part[part.Length - 2] == '}')
                        {
                            JObject jsonObject = JObject.Parse(part);
                            JToken? tokenWall = jsonObject["wall"];
                            JToken? tokenPower = jsonObject["power"];
                            JToken? tokenSnake = jsonObject["snake"];
                            if (tokenWall != null)
                            {
                                wall = JsonConvert.DeserializeObject<Wall>(part);
                            }
                            if (tokenPower != null)
                            {
                                power = JsonConvert.DeserializeObject<Power>(part);
                            }
                            if (tokenSnake != null)
                            {
                                snake = JsonConvert.DeserializeObject<Snake>(part);
                            }
                        }
                        state.RemoveData(0, part.Length);
                        // Lock the world which prevent race condition
                        lock (world)
                        {
                            if (wall != null)
                            {
                                world.addWalls(wall);
                            }
                            if (power != null)
                            {
                                if (power.getDied() == false)
                                {
                                    world.addPower(power);
                                }
                                else
                                {
                                    world.removePower(power);
                                }
                            }
                            if (snake != null)
                            {

                                world.addSnakes(snake);
                            }
                        }
                    }
                }
                // Invoke the view to update
                refreshEvent?.Invoke();
                state.OnNetworkAction = new Action<SocketState>(this.ReceiveWorld);
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// Recieve the key from the user input and sent to the serveer
        /// </summary>
        /// <param name="command"></param>
        public void sendCommand(String command)
        {
            cmdMessage = command;
            if (!this.world.snakes.ContainsKey(this.playerId))
            {
                return;
            }
            Networking.Send(this.socket, CommandBind(cmdMessage));
            cmdMessage = "none";
        }

        /// <summary>
        /// Make it the Json syntax.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private String CommandBind(String command)
        {
            String result = "{\"moving\":" + "\"" + command + "\"}" + "\n";
            return result;
        }
    }
}