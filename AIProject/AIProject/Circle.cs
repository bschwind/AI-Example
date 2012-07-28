using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public struct Circle
    {
        public Vector3 Center;
        public float Radius;

        public Circle(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Intersects(Circle other)
        {
            Vector3 diff = other.Center - Center;
            return diff.LengthSquared() < (Radius + other.Radius) * (Radius + other.Radius);
        }
    }
}
