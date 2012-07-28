using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIProject
{
    //Map is responsible for generating spawn points and other items in a map
    public class Map
    {
        private int width, height;
        private float depth = 0f;
        private Buffer<GameObject>[,] objectMap;
        private Buffer<GameObject> neighborList;

        public Map(GraphicsDevice device, int width, int height)
        {
            this.width = width;
            this.height = height;

            objectMap = new Buffer<GameObject>[width, height];
            initGrid();

            neighborList = new Buffer<GameObject>(40);
        }

        private void initGrid()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    objectMap[i, j] = new Buffer<GameObject>(20);
                }
            }
        }

        public void ClearGridData()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    objectMap[i, j].Clear();
                }
            }
        }

        public int GetObjectCountInCircle(Circle c)
        {
            int count = 0;

            int radius = (int)c.Radius;
            int centerXCoord = (int)c.Center.X;
            int centerYCoord = (int)c.Center.Y;
            int xMin = centerXCoord - radius;
            int xMax = centerXCoord + radius;
            int yMin = centerYCoord - radius;
            int yMax = centerYCoord + radius;

            xMin = (int)Math.Max(xMin, 0);
            yMin = (int)Math.Max(yMin, 0);
            xMax = (int)Math.Min(xMax, width);
            yMax = (int)Math.Min(yMax, height);

            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    for (int k = 0; k < objectMap[i, j].GetCount(); k++)
                    {
                        if (c.Intersects(objectMap[i, j][k].GetBounds()))
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        //Returns all the neighbors within a given circle
        public Buffer<GameObject> GetNeighbors(Circle c, Unit u)
        {
            neighborList.Clear();

            int xMin = (int)(c.Center.X - c.Radius);
            int xMax = (int)(c.Center.X + c.Radius);
            int yMin = (int)(c.Center.Y - c.Radius);
            int yMax = (int)(c.Center.Y + c.Radius);

            xMin = (int)Math.Max(xMin, 0f);
            yMin = (int)Math.Max(yMin, 0f);
            xMax = (int)Math.Min(xMax, width - 1);
            yMax = (int)Math.Min(yMax, height - 1);

            for (int i = xMin; i <= xMax; i++)
            {
                for (int j = yMin; j <= yMax; j++)
                {
                    for (int k = 0; k < objectMap[i, j].GetCount(); k++)
                    {
                        GameObject go = objectMap[i, j][k];
                        if (c.Intersects(go.GetBounds()) && !neighborList.Contains(go))
                        {
                            if (u == null)
                            {
                                neighborList.Add(go);
                            }
                            else if (!u.Equals(go))
                            {
                                neighborList.Add(go);
                            }
                        }
                    }
                }
            }

            return neighborList;
        }

        public int GetGridXOf(GameObject g)
        {
            return (int)g.GetBounds().Center.X;
        }

        public int GetGridYOf(GameObject g)
        {
            return (int)g.GetBounds().Center.Y;
        }

        public void RegisterGridObject(GameObject g)
        {
            Circle c = g.GetBounds();
            int xMin = (int)(c.Center.X - c.Radius);
            int xMax = (int)(c.Center.X + c.Radius);
            int yMin = (int)(c.Center.Y - c.Radius);
            int yMax = (int)(c.Center.Y + c.Radius);

            xMin = (int)Math.Max(xMin, 0f);
            yMin = (int)Math.Max(yMin, 0f);
            xMax = (int)Math.Min(xMax, width-1);
            yMax = (int)Math.Min(yMax, height-1);

            for (int i = xMin; i <= xMax; i++)
            {
                for (int j = yMin; j <= yMax; j++)
                {
                    objectMap[i, j].Add(g);
                }
            }
        }

        private bool withinBounds(int i, int j)
        {
            return i >= 0 && i < width && j >= 0 && j < height;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public void Draw(PrimitiveDrawer pd, GameTime g, Camera cam)
        {
            Color color = Color.FromNonPremultiplied(0, 120, 200, 45);

            //Draw the grid
            pd.Begin(PrimitiveType.LineList, cam);
            for (int i = 0; i <= width; i++)
            {
                pd.DrawLine(new Vector3(i, 0, depth), new Vector3(i, height, depth), color);
            }

            for (int i = 0; i <= height; i++)
            {
                pd.DrawLine(new Vector3(0, i, depth), new Vector3(width, i, depth), color);
            }
            pd.End();
        }
    }
}
