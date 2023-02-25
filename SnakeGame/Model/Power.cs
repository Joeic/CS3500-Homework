// Author: Qiao Yi Cai and Yamin Zhuang, Nov 2022
// Power class in the model of the Snake Game
// This class is responsible for making Json object and property format
// University of Utah
using Newtonsoft.Json;
using SnakeGame;
using System.Numerics;

namespace SnakeGame
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Power
    {
        // Json property with name powerID, location, and died boolean
        [JsonProperty(PropertyName = "power")]
        private int id;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "died")]
        public bool died;
        
        /// <summary>
        /// Power constructor to set the id, location and died info of the power
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loc"></param>
        /// <param name="died"></param>
        public Power(int id,Vector2D loc)
        {
            this.id = id;
            this.loc = loc;
            
        }
        /// <summary>
        /// Get the location of this power up, return the type of vector 2D
        /// </summary>
        /// <returns></returns>
        public Vector2D getLoc()
        {
            return this.loc;
        }
        /// <summary>
        /// Get the Died info of this power up, return the type of bool
        /// </summary>
        /// <returns></returns>
        public bool getDied()
        {
            return this.died;
        }
        /// <summary>
        /// Get the ID info of this power up, return the type of Int
        /// </summary>
        /// <returns></returns>
        public int getId()
        {
            return this.id;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}