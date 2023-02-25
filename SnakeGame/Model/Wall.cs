// Author: Qiao Yi Cai and Yamin Zhuang, Nov 2022
// Walls class in the model of the Snake Game
// This class is responsible for making Json object and property format
// University of Utah
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnakeGame;
using System.Runtime.Serialization;

/// <summary>
/// this libary represent the properties of the Wall 
/// </summary>
namespace SnakeGame;

[DataContract(Namespace ="")]
[JsonObject(MemberSerialization.OptIn)]
public class Wall
{
    // Json property with name wall, p1, p2
    [DataMember]
    [JsonProperty(PropertyName = "wall")]
    private int ID;

    [DataMember]
    [JsonProperty(PropertyName = "p1")]
    private Vector2D p1;

    [DataMember]
    [JsonProperty(PropertyName = "p2")]
    private Vector2D p2;


    // constructor to initilize the field data of the wall
    public Wall(int ID, Vector2D p1, Vector2D p2)
    {
        this.ID = ID;
        this.p1 = p1;
        this.p2 = p2;

    }
    // access the wall ID
    public int getID()
    {
        return this.ID;
    }
    //access to the location of the wall
    public Vector2D getP1()
    {
        return this.p1;
    }

    //access to the location of the wall
    public Vector2D getP2()
    {
        return this.p2;
    }

    /// <summary>
    /// Set the location of the position1
    /// </summary>
    /// <param name="p1"></param>
    public void setP1(Vector2D p1)
    {
        this.p1 = p1;
    }


    /// <summary>
    /// Set the location of the position2
    /// </summary>
    /// <param name="p2"></param>
    public void setP2(Vector2D p2)
    {
        this.p2 = p2;
    }

    /// <summary>
    /// Convert this one to Json Object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

}