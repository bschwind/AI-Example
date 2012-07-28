using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class GameMath
    {
        public static bool TestCircleCircle(Circle c1, Circle c2)
        {
            return (c1.Center - c2.Center).LengthSquared() < (c1.Radius + c2.Radius) * (c1.Radius + c2.Radius);
        }

        public static bool TestCircleRect(Circle c, Rectangle r)
        {
            return SqDistPointRect(new Vector2(c.Center.X, c.Center.Y), r) < c.Radius * c.Radius;
        }

        public static float SqDistPointRect(Vector2 p, Rectangle r)
        {
            float sqDist = 0.0f;

            Vector2 min = new Vector2(r.X, r.Y);
            Vector2 max = new Vector2(r.X + r.Width, r.Y + r.Height);

            float v = p.X;
            if (v < min.X) sqDist += (min.X - v) * (min.X - v);
            if (v > max.X) sqDist += (v - max.X) * (v - max.X);

            v = p.Y;
            if (v < min.Y) sqDist += (min.Y - v) * (min.Y - v);
            if (v > max.Y) sqDist += (v - max.Y) * (v - max.Y);

            return sqDist;
        }

        public Vector2 Cross2D(Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }
    }
}
