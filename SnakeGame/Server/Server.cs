using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SnakeGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Server
{
    public class Server
    {
        private static Dictionary<long, SocketState> clients;
        private static World world;
        private static GameSettings gs;
        private static bool directionChanged = false;
        private static int powerCount = 0;
        private static bool isRunning = false;
        private static Stopwatch watch;
        private static int powerIndex = 0;

        public void read()
        {
            XmlReader reader = XmlReader.Create("settings.xml");
            DataContractSerializer Serializer = new DataContractSerializer(typeof(GameSettings));
            gs = (GameSettings)Serializer.ReadObject(reader)!;
            reader.Close();
        }
        public void StartServer()
        {
            // This begins an "event loop"
            Networking.StartServer(NewClientConnected, 11000);
            Console.WriteLine("Server is running");
            while (isRunning)
            {
                while (watch.ElapsedMilliseconds < gs.MSPerFrame) { }
                watch.Restart();
                update();
            }

        }

        private static void update()
        {
            //1. move every snakes forward 2. check collisons 3. generate pwo
            lock (world)
            {

                generatePower();

                foreach (Snake snake in world.snakes.Values)
                {
                    //if (snake.getDc() == true)

                    Vector2D direc = snake.dir;
                    if (!snake.dirChanged && !snake.died)
                    {
                        snake.body[snake.body.Count - 1] += direc * 3;
                        snake.dirChanged = false;

                    }

                    if (snake.dirChanged && !snake.died)
                    {
                        snake.body.Add(snake.body[snake.body.Count - 1] + direc * 6);


                        snake.dirChanged = false;

                    }
                    if (!snake.died)
                        snakeMove(snake);
                    // snake.dir.Normalize();

                    wrapCheck(gs.UniverseSize, snake);

                    //if(detectCollSnake(snake, snake))
                    //{
                    //    snakeDie(snake);

                    //}
                    if (collisionCheckGeneral(snake))
                    {
                        snakeDie(snake);
                    }
                    if (SelfCollisionCheck(snake))
                        snakeDie(snake);
                    if (CollWithWall(snake.getHeader(), 30))
                        snakeDie(snake);
                    if (SelfCollisionCheck(snake))
                    {
                        snakeDie(snake);
                        while (!CollWithWall(snake.getHeader(), 400) && !CollWithWall(snake.body[0], 400))
                            respawn(snake);
                    }
                    //  snakeDie(snake);

                    if (snake.getDc() == false && snake.getDied())
                    {
                        Console.WriteLine("snake is dead");
                        respawn(snake);

                    }
                    else
                    {
                        foreach (Power powerup in world.powers.Values)
                        {

                            if (!powerup.getDied() && (detectCollPower(powerup.getLoc(), snake.getHeader())))
                            {
                                snake.Powerup();
                                growing(snake);
                                powerup.died = true;
                                powerCount--;
                                Console.WriteLine("power count" + world.powers.Count());
                            }


                        }


                    }

                }

                sendinfo();
                sendPower();
                powerupCleaner();


            }
            lock (clients)
            {
                foreach (SocketState state in clients.Values)
                {
                    if (!state.TheSocket.Connected)
                    {
                        lock (Server.world)
                        {

                            world.snakes[(int)state.ID].setDc(true);


                        }
                    }
                }
                snakeCleaner();
            }
        }
        private static void snakeDie(Snake snake)
        {

            snake.alive = false;
            snake.died = true;
            snake.score = 0;
            Console.WriteLine("snake is alive : " + snake.alive);
        }



        private static void respawn(Snake snake)
        {
            // Wait for 12 frames before respawning the snake.
            if (snake.respawnTime < gs.RespawnRate)
            {
                snake.respawnTime++;
                return;
            }

            Vector2D oldDir = snake.dir;

            Vector2D newDir = randomDir();
            newDir.Normalize();
            ;
            snake.body = createBody(newDir);

            snake.dir = newDir;
            snake.alive = true;
            snake.died = false;
            snake.respawnTime = 0;
        }
        private static void snakeMove(Snake snake)
        {
            if (snake.body[0].X == snake.body[1].X)
            {
                if (snake.body[0].Y > snake.body[1].Y)
                {

                    snake.body[0].Y -= 3;
                }
                else
                {
                    snake.body[0].Y += 3;
                }
                if (Math.Abs(snake.body[0].Y - snake.body[1].Y) <= 3)
                {
                    if (snake.body.Count > 1)
                    {
                        snake.body.Remove(snake.body[0]);
                    }
                }
            }
            else if (snake.body[0].Y == snake.body[1].Y)
            {
                if (snake.body[0].X > snake.body[1].X)
                {
                    snake.body[0].X -= 3;
                }
                else
                {
                    snake.body[0].X += 3;
                }
                if (Math.Abs(snake.body[0].X - snake.body[1].X) <= 3)
                {
                    if (snake.body.Count > 1)
                    {
                        snake.body.Remove(snake.body[0]);
                    }
                }
            }
        }
        private static void growing(Snake snake)
        {
            snake.body.Add(snake.body[snake.body.Count - 1] + snake.dir * 12);
        }
        private static bool collisionCheckGeneral(Snake snake)
        {
            foreach (Snake anotherSnake in world.snakes.Values)
            {
                if (snake.getId() != anotherSnake.getId())
                {
                    int index = anotherSnake.getBody().Count - 1;
                    while (index > 0)
                    {
                        double currentX = anotherSnake.getBody()[index].X;
                        double currentY = anotherSnake.getBody()[index].Y;
                        Vector2D Current = new Vector2D(currentX, currentY);
                        double nextX = anotherSnake.getBody()[index - 1].X;
                        double nextY = anotherSnake.getBody()[index - 1].Y;
                        Vector2D next = new Vector2D(nextX, nextY);
                        if (detectCollSn(Current, next, snake.getHeader()))
                            return true;

                        index = index - 1;
                    }
                    //   return false;
                }
            }
            return false;
        }
        private static bool SelfCollisionCheck(Snake snake)
        {
            int index = snake.getBody().Count - 4;
            while (index > 0)
            {
                double currentX = snake.getBody()[index].X;
                double currentY = snake.getBody()[index].Y;
                Vector2D Current = new Vector2D(currentX, currentY);
                double nextX = snake.getBody()[index - 1].X;
                double nextY = snake.getBody()[index - 1].Y;
                Vector2D next = new Vector2D(nextX, nextY);
                if (detectCollSn(Current, next, snake.getHeader()))
                    return true;

                index = index - 1;
            }
            return false;
        }


        private static void wrapCheck(int universeSize, Snake snake)
        {
            int Number = universeSize / 2;
            double checkNumber = Convert.ToDouble(Number);
            Vector2D oldHead = snake.getHeader();
            Vector2D newHead = new Vector2D(snake.getHeader());
            if (newHead.Y < -checkNumber)
                newHead.Y = checkNumber;
            if (newHead.Y > checkNumber)
            {
                newHead.Y = -checkNumber;
            }
            if (newHead.X > checkNumber)
                newHead.X = -checkNumber;
            if (newHead.X < -checkNumber)
                newHead.X = checkNumber;
            Vector2D tail = snake.body.First();
            if (tail.Y < -checkNumber)
                tail.Y = checkNumber;
            if (tail.Y > checkNumber)
            {
                tail.Y = -checkNumber;
            }
            if (tail.X > checkNumber)
                tail.X = -checkNumber;
            if (tail.X < -checkNumber)
                tail.X = checkNumber;
            if (!newHead.Equals(oldHead))
            {
                snake.body.Add(newHead);
                snake.body.Add(new Vector2D(newHead));
            }

        }

        private static bool detectCollPower(Vector2D p_head, Vector2D s_head)
        {
            double length = (p_head - s_head).Length();
            return length <= 13.0;

        }
        private static bool detectCollWall(Wall w, Vector2D head, int paddding)
        {
            double maxX = Math.Max(w.getP1().X, w.getP2().X) + paddding;
            double minX = Math.Min(w.getP1().X, w.getP2().X) - paddding;
            double maxY = Math.Max(w.getP1().Y, w.getP2().Y) + paddding;
            double minY = Math.Min(w.getP1().Y, w.getP2().Y) - paddding;
            if ((head.X > minX && head.X < maxX) && (head.Y > minY && head.Y < maxY))
            {
                return true;
            }
            return false;
        }
        private static bool detectCollSn(Vector2D curSeg, Vector2D nextSeg, Vector2D head)
        {

            double maxX = Math.Max(curSeg.X, nextSeg.X) + 5;
            double minX = Math.Min(curSeg.X, nextSeg.X) - 5;
            double maxY = Math.Max(curSeg.Y, nextSeg.Y) + 5;
            double minY = Math.Min(curSeg.Y, nextSeg.Y) - 5;
            if ((head.X > minX && head.X < maxX) && (head.Y > minY && head.Y < maxY))
            {
                return true;
            }
            return false;
        }
        private static bool CollWithWall(Vector2D head, int padding)
        {
            foreach (Wall w in world.walls.Values)
            {
                if (detectCollWall(w, head, padding) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private static void powerupCleaner()
        {

            foreach (Power p in world.powers.Values)
            {
                if (p.died)
                {

                    world.powers.Remove(p.getId());

                    // powers
                }

            }
        }
        private static void snakeCleaner()
        {
            foreach (Snake snake in world.snakes.Values)
            {
                if (snake.getDc() == true)
                {
                    snakeDie(snake);
                    sendinfo();
                    world.snakes.Remove(snake.getId());
                    clients.Remove(snake.getId());
                }
            }
        }

        private static void generatePower()
        {

            for (; world.powers.Count < 20; powerCount++)
            {
                Random rand = new Random();
                // Generate a random position for the power-up.
                Vector2D powerPos = new Vector2D(
                    rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2,
                    rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2
                );

                // Check if the power-up is inside a wall.
                if (CollWithWall(powerPos, 200))
                {
                    // If the power-up is inside a wall, generate a new position.
                    // Keep trying new positions until you find one that is not inside a wall.
                    do
                    {
                        powerPos = new Vector2D(
                            rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2,
                            rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2
                        );
                    }
                    while (CollWithWall(powerPos, 30));
                }
                foreach (Snake snake in world.snakes.Values)
                {
                    if (detectCollPower(powerPos, snake.getHeader()))
                    {
                        // If the power-up is inside a snake, generate a new position.
                        // Keep trying new positions until you find one that is not inside a snake.
                        do
                        {
                            powerPos = new Vector2D(
                                rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2,
                                rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2
                            );
                        }
                        while (detectCollPower(powerPos, snake.getHeader()));
                    }
                }
                foreach (Power p in world.powers.Values)
                {
                    if (detectCollPower(powerPos, p.getLoc()))
                    {
                        // If the power-up is inside a snake, generate a new position.
                        // Keep trying new positions until you find one that is not inside a snake.
                        do
                        {
                            powerPos = new Vector2D(
                                rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2,
                                rand.NextDouble() * gs.UniverseSize - gs.UniverseSize / 2
                            );
                        }
                        while (detectCollPower(powerPos, p.getLoc()));
                    }
                }

                // Create a new power-up object at the valid position.
                Power powerup = new Power(powerIndex, powerPos);
                powerIndex++;
                world.powers.Add(powerup.getId(), powerup);
            }

        }

        private static void sendPower()
        {

            StringBuilder sb = new StringBuilder();
            foreach (Power p in world.powers.Values)
            {
                sb.Append(p.ToString() + "\n");
            }
            foreach (SocketState client in clients.Values)
            {

                Networking.Send(client.TheSocket, sb.ToString());
            }

        }


        private static void sendinfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Snake s in world.snakes.Values)
            {
                sb.Append(s.ToString() + "\n");
            }
            foreach (SocketState client in clients.Values)
            {
                if (client.TheSocket.Connected)
                {
                    Networking.Send(client.TheSocket, sb.ToString());
                }
                else
                {
                    continue;
                }
            }
        }

        private void NewClientConnected(SocketState state)
        {
            if (!isRunning || state.ErrorOccurred)
                return;

            // Save the client state
            // Need to lock here because clients can disconnect at any time


            // change the state's network action to the 
            // receive handler so we can process data when something
            // happens on the network

            state.OnNetworkAction = ReceiveNameAndSendData;
            Console.WriteLine("waiting for clients");
            Networking.GetData(state);
        }

        private void ReceiveNameAndSendData(SocketState state)
        {
            if (!isRunning || state.ErrorOccurred)
            {

                return;
            }
            String name = state.GetData();
            Vector2D direction = randomDir();
            Snake s = new Snake(Convert.ToInt32(state.ID), name, createBody(direction), direction, 0, false, true, false, true);
            Console.WriteLine("Player(" + state.ID.ToString() + ") \"" + name + "\" joined Game.");
            Networking.Send(state.TheSocket, state.ID.ToString() + "\n" + gs.UniverseSize + "\n");
            Networking.Send(state.TheSocket, sendWalls().ToString() + "\n");
            Networking.Send(state.TheSocket, s.ToString() + "\n");
            //world.walls = new Dictionary<int, Wall>(gs.Walls);

            lock (world)
            {
                world.addSnakes(s);
            }




            // world.snakes.Add(Convert.ToInt32(state.ID), s);



            state.OnNetworkAction = new Action<SocketState>(ReceiveMessage);
            lock (clients)
            {
                clients.Add(state.ID, state);
            }
            Networking.GetData(state);
        }

        private static Vector2D randomDir()
        {

            List<Vector2D> ranDirList = new List<Vector2D>();
            ranDirList.Add(new Vector2D(0, -1));//up
            ranDirList.Add(new Vector2D(0, 1));
            ranDirList.Add(new Vector2D(1, 0));
            ranDirList.Add(new Vector2D(-1, 0));

            var random = new Random();
            int index = random.Next(0, ranDirList.Count);

            return ranDirList[index];

        }
        private static Vector2D randomLoc()
        {
            double powerX;
            double powerY;
            Random random = new Random();
            double randomX = random.NextDouble() * 2.0 - 1.0;
            double randomY = random.NextDouble() * 2.0 - 1.0;
            Vector2D randomPos = new Vector2D(randomX * gs.UniverseSize / 2, randomY * gs.UniverseSize / 2);
            return randomPos;
        }
        private static List<Vector2D> createBody(Vector2D direction)
        {
            Vector2D location = randomLoc();
            while (CollWithWall(location, 150))
                location = randomLoc();


            List<Vector2D> body = new List<Vector2D>();

            double HeadX = location.X;
            double HeadY = location.Y;
            double tailX = 0;
            double tailY = 0;

            if (direction.Y == 1)
            {
                tailX = HeadX;
                tailY = HeadY - 120;
            }
            else if (direction.Y == -1)
            {
                tailX = HeadX;
                tailY = HeadY + 120;
            }
            else if (direction.X == 1)
            {
                tailY = HeadY;
                tailX = HeadX - 120;

            }
            else if (direction.X == -1)
            {

                tailY = HeadY;
                tailX = HeadX + 120;
            }
            body.Add(new Vector2D(tailX, tailY));
            body.Add(new Vector2D(HeadX, HeadY));
            return body;
        }



        private StringBuilder sendWalls()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Wall w in gs.Walls)
            {
                sb.Append(w.ToString() + "\n");
                if (world.walls.ContainsKey(w.getID()) == false)
                {
                    world.walls.Add(w.getID(), w);
                }
            }
            return sb;
        }
        private void ReceiveMessage(SocketState state)
        {

            if (state.ErrorOccurred)
            {

                return;
            }
            ProcessMessage(state);
            Networking.GetData(state);
        }

        private bool ifTurn(Vector2D dir, String cmd)
        {
            if (cmd == "up" && (dir.Y == 1 || dir.Y == -1)) { return false; }
            if (cmd == "down" && (dir.Y == -1 || dir.Y == 1)) { return false; }
            if (cmd == "right" && (dir.X == -1 || dir.X == 1)) { return false; }
            if (cmd == "left" && (dir.X == 1 || dir.X == -1)) { return false; }
            if (cmd == "none")
                return false;
            return true;
        }
        private void processCMD(Snake s, String cmd)
        {
            if (s.getAlive() == true && ifTurn(s.dir, cmd))
            {
                try
                {
                    lock (world)
                    {

                        s.dirChanged = true;

                        s.turn(cmd, world);
                    }
                }
                catch
                {//lock  world
                    return;
                }
            }
        }

        private String getCMD(String info)
        {
            if (info.Contains("up"))
            {

                return "up";
            }
            else if (info.Contains("down"))
            {

                return "down";
            }
            else if (info.Contains("left"))
            {

                return "left";
            }
            else if (info.Contains("right"))
            {

                return "right";
            }
            return "none";
        }
        private void ProcessMessage(SocketState state)
        {

            string totalData = state.GetData();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            try
            {
                foreach (string p in parts)
                {
                    lock (Server.world)
                    {
                        if (p.Contains("moving"))
                        {
                            String cmd = getCMD(p);
                            Snake s;
                            bool hasValue = world.snakes.TryGetValue(Convert.ToInt32(state.ID), out s);
                            if (hasValue)
                            {
                                processCMD(s, cmd);
                            }
                        }
                    }
                    state.RemoveData(0, p.Length);
                }
            }
            catch
            {
            }
        }


        static void Main(String[] args)
        {
            clients = new Dictionary<long, SocketState>();
            Server server = new Server();
            server.read();
            world = new World(gs.UniverseSize);
            watch = new Stopwatch();
            watch.Start();
            Server.isRunning = true;
            new Task((Action)(() => server.StartServer())).Start();
            Console.ReadLine();
            Server.isRunning = false;
            Console.Read();
        }
    }
} 