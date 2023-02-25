// Author: Qiao Yi Cai and Yamin Zhuang, Nov 2022
// World panel of the Snake Game
// This class is responsible for drawing all objects in the world.
// University of Utah
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System;
using IImage = Microsoft.Maui.Graphics.IImage;

#if MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#else
using Microsoft.Maui.Graphics.Win2D;
#endif
using Color = Microsoft.Maui.Graphics.Color;
using System.Reflection;
using Microsoft.Maui;
using System.Net;
using Font = Microsoft.Maui.Graphics.Font;
using SizeF = Microsoft.Maui.Graphics.SizeF;
using SnakeGame;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Maui.Graphics;

namespace SnakeGame;
/// <summary>
/// This class draw all the things that needed
/// </summary>
public class WorldPanel : IDrawable
{
    private IImage wall;
    private IImage background;
    private IImage apple;
    private IImage explosion;
    private bool initializedForDrawing = false;
    private World world;
    public delegate void ObjectDrawer(object o, ICanvas canvas);
    public int mainPlayerId;

    List<Color> cubList;
#if MACCATALYST
    private IImage loadImage(string name)
    {
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        string path = "SnakeGame.Resources.Images";
        return PlatformImage.FromStream(assembly.GetManifestResourceStream($"{path}.{name}"));
    }
#else
    /// <summary>
    /// Load the path of the image
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IImage loadImage( string name )
    {
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        string path = "SnakeGame.Resources.Images";
        var service = new W2DImageLoadingService();
        return service.FromStream(assembly.GetManifestResourceStream( $"{path}.{name}" ) );
    }
#endif
    /// <summary>
    /// Set the world by the given world
    /// </summary>
    /// <param name="world"></param>
    public void setWorld(World world)
    {
        this.world = world;
    }
    
    /// <summary>
    /// Constructor which initialize the cublist
    /// </summary>
    public WorldPanel()
    {
        cubList = new List<Color>();
    }

    /// <summary>
    /// load wall and background and Initialize Drawing
    /// </summary>
    private void InitializeDrawing()
    {
        // Load the image of wall, background, apple, and explosion
        wall = loadImage( "WallSprite.png" );
        background = loadImage( "Background.png" );
        apple = loadImage("apple.png");
        explosion = loadImage("explosion.png");
        initializedForDrawing = true;
    }

    /// <summary>
    /// This method performs the necessary transformation,
    /// given information that you supply, and then invokes your ObjectDrawer delegate.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="o"></param>
    /// <param name="worldX"></param>
    /// <param name="worldY"></param>
    /// <param name="angle"></param>
    /// <param name="drawer"></param>
    private void DrawObjectWithTransform( ICanvas canvas,object o,double worldX,double worldY,double angle,WorldPanel.ObjectDrawer drawer)
    {
        canvas.SaveState();
        canvas.Translate((float)worldX, (float)worldY);
        canvas.Rotate((float)angle);
        drawer(o, canvas);
        canvas.RestoreState();
    }

    /// <summary>
    /// Draw the walls
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    private void wallDrawer(object o, ICanvas canvas)
    {
        canvas.DrawImage(wall, (float)(-(double)50 / 2.0), (float)(-(double)50 / 2.0), 50, 50);
    }
    /// <summary>
    /// Draw the explosions
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    public virtual void ExplosionDrawer(object o, ICanvas canvas)
    {

        canvas.DrawImage(explosion, (float)(-(double)50 / 2.0), (float)(-(double)50 / 2.0), 50, 50);
    }

    /// <summary>
    /// Draw the segments of the snake
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    private void SnakeSegmentDrawer(object o, ICanvas canvas)
    {

        float snakeSegmentLength = float.Parse(o.ToString());

        if (snakeSegmentLength >= (float)this.world.Size)
            return;
        canvas.StrokeSize = 10f;
        // Draw the circle head, line body and circle tail.
        canvas.DrawLine(0.0f, 0.0f, 0.0f, -snakeSegmentLength);
        canvas.FillCircle(0.0f, 0.0f, 5f);
        canvas.FillCircle(0.0f, -snakeSegmentLength, 5f);
    }
    /// <summary>
    /// Get enumerator Vector 2D of all walls points
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    private IEnumerable<Vector2D> checkallWalls(Wall wall)
    {
        List<Vector2D> result = new List<Vector2D>();
        // If the x- cordinate of the end point is equal to the other the y- cordinate of the end point
        if (wall.getP1().X == wall.getP2().X)
        {
            // If the Y- cordinate of the end point is less than the other the Y- cordinate of the end point
            if (wall.getP1().Y < wall.getP2().Y)
            {
                double tempy = wall.getP1().Y;
                // loop while the Y- cordinate of the end point is less than the other the y- cordinate of the end point
                while (tempy <= wall.getP2().Y)
                {
                    Vector2D temp = new Vector2D(wall.getP1().X, tempy);
                    result.Add(temp);
                    tempy += 50;
                }
            }
            // Else if the Y- cordinate of the end point is greater than the other the Y- cordinate of the end point
            else
            {
                double tempy = wall.getP2().Y;
                // loop while the Y- cordinate of the end point is less than the other the y- cordinate of the end point
                while (tempy <= wall.getP1().Y)
                {
                    Vector2D temp = new Vector2D(wall.getP1().X, tempy);
                    result.Add(temp);
                    tempy += 50;
                }
            }
        }
        // If the Y- cordinate of the end point is equal to the other the Y- cordinate of the end point
        else if (wall.getP1().Y == wall.getP2().Y)
        {
            // If the X- cordinate of the end point is less than the other the X- cordinate of the end point
            if (wall.getP1().X < wall.getP2().X)
            {
                double tempx = wall.getP1().X;
                // loop while the X - cordinate of the end point is less than the other the X - cordinate of the end point
                while (tempx <= wall.getP2().X)
                {
                    Vector2D temp = new Vector2D(tempx, wall.getP1().Y);
                    result.Add(temp);
                    tempx += 50;
                }
            }
            // Else if the X- cordinate of the end point is greate than the other the X- cordinate of the end point
            else
            {
                double tempx = wall.getP2().X;
                // loop while the X - cordinate of the end point is less than the other the X - cordinate of the end point
                while (tempx <= wall.getP1().X)
                {
                    Vector2D temp = new Vector2D(tempx, wall.getP1().Y);
                    result.Add(temp);
                    tempx += 50;
                }
            }
        }
        return result;

    }

    /// <summary>
    /// Get the enumerable vector 2D list of the snake segments
    /// </summary>
    /// <param name="snake"></param>
    /// <returns></returns>
    public IEnumerable<(Vector2D v1, Vector2D v2)> Segments(Snake snake)
    {
        LinkedListNode<Vector2D> current = snake.getBody().First;
        if (current != null)
        {
            for (; current.Next != null; current = current.Next)
                yield return (current.Value, current.Next.Value);
        }
    }

    /// <summary>
    /// Check all snake Bodys and return the list of snake bodies
    /// </summary>
    /// <param name="snake"></param>
    /// <returns></returns>
    private List<Tuple<Vector2D,Vector2D>> checkAllSnakeBodys(Snake snake)
    {
        List<Tuple<Vector2D,Vector2D>> result = new List<Tuple<Vector2D,Vector2D>>();
        LinkedList<Vector2D> bodys = snake.getBody();
        int index = 0;
        Vector2D header = bodys.First();
        // loop while it is not the last segment of the snake
          while (index < bodys.Count()-1)
         {
              Tuple<Vector2D, Vector2D> tuple = new Tuple<Vector2D, Vector2D>(header, bodys.ElementAt(index+1));
               result.Add(tuple);
               header = bodys.ElementAt(index+1);
              index++;
           }
          return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    private void PowerupDrawer(object o, ICanvas canvas)
    {
        Power p = o as Power;
        int width = 16;
        //// Ellipses are drawn starting from the top-left corner.
        //// So if we want the circle centered on the powerup's location, we have to offset it
        //// by half its size to the left (-width/2) and up (-height/2)
        canvas.DrawImage(apple, -(width / 2), -(width / 2), width, width);
    }
    /// <summary>
    /// Draw the score board on the left of the player name
    /// </summary>
    /// <param name="o"></param>
    /// <param name="canvas"></param>
    public void ScordBoardDrawer(object o, ICanvas canvas)
    {
        Snake snake = o as Snake;
        String displayName = snake.getName() + ":" + snake.getScore();
        canvas.FontColor = Colors.White;
        SizeF size=canvas.GetStringSize(displayName, Font.Default, float.Parse("14"));
        canvas.DrawString(displayName, size.Width-70, 0, size.Width, size.Height, HorizontalAlignment.Center, VerticalAlignment.Center);
    
    }
    /// <summary>
    /// This list has 8 different colors which represent 8 different snakes.
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    private Color getColor(int playerId)
    {  
        List<Color> colorList = new List<Color>();
        colorList.Add(Colors.Red);
        colorList.Add(Colors.Yellow);
        colorList.Add(Colors.Black);
        colorList.Add(Colors.Blue);
        colorList.Add(Colors.Purple);
        colorList.Add(Colors.Pink);
        colorList.Add(Colors.Gray);
        colorList.Add(Colors.Orange);
        int index = -1;
        int temp = playerId;
        // use plaerId to gerenate different color of the snake
        while (temp / 8 != 0)
        {
            temp = temp / 8;
        }
        index = temp % 8;
        return colorList[index];
    }

    /// <summary>
    /// This method is automatically called when the view is informed to be invalidate
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if ( !initializedForDrawing )
            InitializeDrawing();

        // undo previous transformations from last frame
        canvas.ResetState();
        if (world != null)
        {
            // Lock the world which prevent race conditions.
            lock (world)
            {
                // If the player exists
                if (world.mainPlayerId != -1)
                {
                    // If we have this player id's key
                    // Draw the background of the image
                    if (world.snakes.ContainsKey(world.mainPlayerId) == true)
                    {
                        Snake snake = world.snakes[world.mainPlayerId];
                        float playerX = (float)(snake.getCentral().X);
                        float playerY = (float)(snake.getCentral().Y);
                        canvas.Translate(world.Size / 2-570 - playerX, world.Size / 2-570 - playerY);
                        canvas.DrawImage(background, -world.Size/2, -world.Size/2, world.Size, world.Size);
                    }
                }
                // Loop through the walls and use DrawObjectWithTransform to draw the walls
                foreach (Wall wall in world.walls.Values)
                {
                    foreach (Vector2D point in checkallWalls(wall))
                    {
                        DrawObjectWithTransform(canvas, wall, point.GetX(), point.GetY(), 0, wallDrawer);
                    }
                }

                // Loop through the powers and use DrawObjectWithTransform to draw the powerup
                foreach (Power power in world.powers.Values)
                {
                    // If the power is alived, draw the power up
                    if(!power.getDied())
                        DrawObjectWithTransform(canvas, power, power.getLoc().X, power.getLoc().Y, 0, PowerupDrawer);
                }

                // Loop through the snakes and use DrawObjectWithTransform to draw the snake
                foreach (Snake snake in this.world.snakes.Values)
                {
                    // if the snake is alived loop throgh the segments of the snake body.
                    if (snake.getAlive())
                    {
                        foreach(Tuple<Vector2D,Vector2D> segment in checkAllSnakeBodys(snake))
                        {
                            // Draw each segment of the snake
                            Vector2D vector2D = segment.Item2 - segment.Item1;
                            vector2D.Normalize();
                            Color color=getColor(snake.getId());
                            canvas.StrokeColor = color;
                            canvas.FillColor = color;
                            DrawObjectWithTransform(canvas, (segment.Item2 - segment.Item1).Length(), segment.Item1.X, segment.Item1.Y, vector2D.ToAngle(), SnakeSegmentDrawer);
                        }
                        // Draw the name on the top of the head of the snake
                        this.DrawObjectWithTransform(canvas, (object)snake, snake.getCentral().X, snake.getCentral().Y, 0.0, new WorldPanel.ObjectDrawer(this.ScordBoardDrawer));
                    }
                    // If the snake is died, draw the explosion effect
                    if(snake.getDied())
                    {
                        DrawObjectWithTransform(canvas, snake, snake.getCentral().X, snake.getCentral().Y, snake.getDir().ToAngle(), ExplosionDrawer);
                    }
                }
            }
        }  
    }
}