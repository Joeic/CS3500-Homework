// Author: Qiao Yi Cai and Yamin Zhuang, Nov 2022
// Snake class in the model of the Snake Game
// This class is responsible for making Json object and property format
// University of Utah
using Newtonsoft.Json;
using SnakeGame;
using System.Runtime.Intrinsics;

namespace SnakeGame
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Snake
    {
        // Json Property with name: snake, name, body, dir, score, died, alive, dc, join
        [JsonProperty(PropertyName = "snake")]
        private int id;
        [JsonProperty(PropertyName = "name")]
        private String name;
        [JsonProperty(PropertyName = "body")]
        public List<Vector2D> body;
        [JsonProperty(PropertyName = "dir")]
        public Vector2D dir;
        [JsonProperty(PropertyName = "score")]
        public int score;
        [JsonProperty(PropertyName = "died")]
        public bool died;
        [JsonProperty(PropertyName = "alive")]
        public bool alive;
        [JsonProperty(PropertyName = "dc")]
        private bool dc;
        [JsonProperty(PropertyName = "join")]
        public bool join;

        public bool dirChanged;
        public int respawnTime = 0;
        /// <summary>
        /// Snake constructor which sets up the value.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="body"></param>
        /// <param name="dir"></param>
        /// <param name="score"></param>
        /// <param name="died"></param>
        /// <param name="alive"></param>
        /// <param name="dc"></param>
        /// <param name="join"></param>
        public Snake(int id, string name, List<Vector2D> body, Vector2D dir, int score, bool died, bool alive, bool dc, bool join)
        {
            this.id = id;
            this.name = name;
            this.body = body;
            this.dir = dir;
            this.score = score;
            this.died = died;
            this.alive = alive;
            this.dc = dc;
            this.join = join;
        }



        /// <summary>
        /// Get the head of the body by return the type of the vector 2D
        /// </summary>
        /// <returns></returns>
        public Vector2D getCentral()
        {
            return this.body.Last();
        }
        public void setDc(bool dc)
        {
            this.dc = true;
        }
        /// <summary>
        /// Get the id of the snake
        /// </summary>
        /// <returns></returns>
        public int getId()
        {
            return this.id;
        }

        /// <summary>
        /// Get the name of the snake
        /// </summary>
        /// <returns></returns>
        public String getName()
        {
            return this.name;
        }

        /// <summary>
        /// Get the body of the snake
        /// </summary>
        /// <returns></returns>
        public List<Vector2D> getBody()
        {
            return this.body;
        }

        /// <summary>
        /// Get the direction of the snake
        /// </summary>
        /// <returns></returns>
        public Vector2D getDir()
        {
            return this.dir;
        }

        /// <summary>
        /// Get the score of the snake
        /// </summary>
        /// <returns></returns>
        public int getScore()
        {
            return this.score;
        }

        /// <summary>
        /// Get died info of the snake
        /// </summary>
        /// <returns></returns>
        public bool getDied()
        {
            return this.died;
        }

        /// <summary>
        /// Get the alive info of the snake
        /// </summary>
        /// <returns></returns>
        public bool getAlive()
        {
            return this.alive;
        }

        /// <summary>
        /// Check if it is disconnected
        /// </summary>
        /// <returns></returns>
        public bool getDc()
        {
            return this.dc;
        }

        /// <summary>
        /// Check if a new snake is joining
        /// </summary>
        /// <returns></returns>
        public bool getJoin()
        {
            return this.join;
        }

        /// <summary>
        /// Get the head of the body
        /// </summary>
        /// <returns></returns>
        public Vector2D getHeader()
        {
            return this.body.Last();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void turn(String cmd, World w)
        {
            if (cmd == "up")
            {
                dir = new Vector2D(0, -1);
            }
            if (cmd == "down")
            {
                dir = new Vector2D(0, 1);
            }
            if (cmd == "left")
            {
                dir = new Vector2D(-1, 0);
            }
            if (cmd == "right")
            {
                dir = new Vector2D(1, 0);
            }
            if (cmd == "none")
            {
                return;
            }
        }
        public IEnumerable<(Vector2D v1, Vector2D v2)> Segments()
        {
            List<(Vector2D v1, Vector2D v2)> seg = new();

            if (this.body.First() != null)
            {
                for (int count = 0; this.body[count + 1] != null; count++)
                {
                    seg.Add((this.body[count], this.body[count + 1]));
                }
            }
            return seg;
        }
        public void Powerup()
        {
            score++;
        }
    }
}