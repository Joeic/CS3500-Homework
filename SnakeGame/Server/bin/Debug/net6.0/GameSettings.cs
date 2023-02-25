using System;
using SnakeGame;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [DataContract(Name = "GameSettings", Namespace = "")]
    public class GameSettings
    {
        [DataMember]
        public int FramesPerShot { get; set; }
        [DataMember]
        public int MSPerFrame { get; set; }
        [DataMember]
        public int RespawnRate { get; set; }
        [DataMember]
        public int UniverseSize { get; set; }
        [DataMember]
        public List<Wall>? Walls { get; set; }

        public GameSettings()
        {
            MSPerFrame = 33;

        }

        

    }
}