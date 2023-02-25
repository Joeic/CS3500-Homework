// Author: Qiao Yi Cai and Yamin Zhuang, Nov 2022
// World class in the model of the Snake Game
// This class is responsible for making Json object and property format
// University of Utah
using SnakeGame;

namespace SnakeGame
{
    /// <summary>
    /// This class decribe the state of the world, contains all objects like walls, powers and snakes.
    /// </summary>
    public class World
    {
        // Dictionary to store the walls, powers and snakes
        public Dictionary<int, Wall> walls;
        public Dictionary<int, Power> powers;
        public Dictionary<int, Snake> snakes;
        public int mainPlayerId;

        /// <summary>
        /// Field to get the size
        /// </summary>
        public int Size
        { get; private set; }

        /// <summary>
        /// Constructor which iniatialize the values
        /// </summary>
        /// <param name="_size"></param>
        public World(int _size)
        {
            Size = _size;
            this.walls = new Dictionary<int, Wall>();
            this.powers = new Dictionary<int, Power>();
            this.snakes = new Dictionary<int, Snake>();
            mainPlayerId = -1;
        }

        /// <summary>
        /// This method add walls to the wall dictionary
        /// </summary>
        /// <param name="wall"></param>
        public void addWalls(Wall wall)
        {
            walls[wall.getID()] = wall;
        }

        /// <summary>
        /// This method add power to the power dictionary
        /// </summary>
        /// <param name="power"></param>
        public void addPower(Power power)
        {
            powers[power.getId()] = power;
        }

        /// <summary>
        /// This method remove wall from the wall dictionary
        /// </summary>
        /// <param name="wall"></param>
        public void removePower(Power power)
        {
            powers.Remove(power.getId());
        }

        /// <summary>
        /// This method add snake to the snake dictionary
        /// </summary>
        /// <param name="snake"></param>
        public void addSnakes(Snake snake)
        {
            snakes[snake.getId()] = snake;
        }

        /// <summary>
        /// This method remove snake from the snake dictionary
        /// </summary>
        /// <param name="snake"></param>
        public void removeSnakes(Snake snake)
        {
            snakes.Remove(snake.getId());
        }

        public void update()
        {

        }
    }
}