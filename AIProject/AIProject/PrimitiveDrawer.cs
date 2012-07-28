using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIProject
{
    //Used to draw simple shapes on the graphics card
    public class PrimitiveDrawer
    {
        GraphicsDevice device;
        BasicEffect effect;
        int maxVertsPerDraw = 65535*2;
        int vertCounter = 0;
        int vertsPerPrimitive;
        VertexPositionColor[] verts;
        Camera currentCam;
        bool hasBegun = false;
        PrimitiveType currentType;

        public GraphicsDevice Device
        {
            get
            {
                return device;
            }
        }

        public PrimitiveDrawer(GraphicsDevice gd)
        {
            device = gd;
            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;
            verts = new VertexPositionColor[maxVertsPerDraw];
        }

        public void Begin(PrimitiveType primType, Camera cam)
        {
            if (hasBegun)
            {
                throw new Exception("Can't call Begin until current batch has ended");
            }

            vertCounter = 0;
            vertsPerPrimitive = numVertsPerPrimitive(primType);
            currentCam = cam;
            currentType = primType;

            hasBegun = true;
        }

        public void AddVertex(VertexPositionColor vpc)
        {
            if (!hasBegun)
            {
                throw new Exception("You must begin a batch before you can add vertices");
            }

            if (vertCounter >= verts.Length)
            {
                Flush();
            }

            verts[vertCounter] = vpc;
            vertCounter++;
        }

        public void DrawLine(Vector3 v1, Vector3 v2, Color color)
        {
            AddVertex(new VertexPositionColor(v1, color));
            AddVertex(new VertexPositionColor(v2, color));
        }

        public void DrawCircle(Circle c, Color color)
        {
            DrawCircle(c.Center, c.Radius, color);
        }

        public void DrawCircle(Vector3 center, float radius, Color color)
        {
            int iterations = 20;

            Vector3 prevPos = center + new Vector3(radius, 0, 0);

            for (int i = 0; i < iterations; i++)
            {
                float angle = ((float)i / iterations) * MathHelper.TwoPi;
                Vector3 pos = center + new Vector3((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius, 0);
                AddVertex(new VertexPositionColor(prevPos, color));
                AddVertex(new VertexPositionColor(pos, color));
                prevPos = pos;
            }

            AddVertex(new VertexPositionColor(prevPos, color));
            AddVertex(new VertexPositionColor(center + new Vector3(radius, 0, 0), color));
        }

        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
            AddVertex(new VertexPositionColor(v1, color));
            AddVertex(new VertexPositionColor(v2, color));
            AddVertex(new VertexPositionColor(v2, color));
            AddVertex(new VertexPositionColor(v3, color));
            AddVertex(new VertexPositionColor(v3, color));
            AddVertex(new VertexPositionColor(v1, color));
        }

        public void DrawGrid(int width, int height, Color color)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DrawLine(new Vector3(x, y, 0), new Vector3(x, y + 1, 0), color);
                    DrawLine(new Vector3(x + 1, y, 0), new Vector3(x, y, 0), color);
                }
            }

            for (int x = 0; x < width; x++)
            {
                DrawLine(new Vector3(x, height, 0), new Vector3(x + 1, height, 0), color);
                DrawLine(new Vector3(width, x, 0), new Vector3(width, x + 1, 0), color);
            }
        }

        public void FillTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
        {
            AddVertex(new VertexPositionColor(v1, color));
            AddVertex(new VertexPositionColor(v2, color));
            AddVertex(new VertexPositionColor(v3, color));
        }

        private void Flush()
        {
            int primitiveCount = vertCounter / vertsPerPrimitive;

            effect.View = currentCam.View;
            effect.Projection = currentCam.Projection;

            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(currentType, verts, 0, primitiveCount);

            vertCounter = 0;
        }

        public void DrawVertexBuffer(VertexBuffer buffer, PrimitiveType primType, Camera cam)
        {
            device.SetVertexBuffer(buffer);
            effect.View = currentCam.View;
            effect.Projection = currentCam.Projection;

            effect.CurrentTechnique.Passes[0].Apply();
            int passes = buffer.VertexCount / maxVertsPerDraw;
            int remainder = buffer.VertexCount % maxVertsPerDraw;
            int offset = 0;
            for (int i = 0; i < passes; i++)
            {
                device.DrawPrimitives(primType, offset, maxVertsPerDraw / numVertsPerPrimitive(primType));
                offset += maxVertsPerDraw;
            }

            device.DrawPrimitives(primType, offset, remainder / numVertsPerPrimitive(primType));
        }

        public void End()
        {
            if (!hasBegun)
            {
                throw new Exception("Can't end a batch without beginning it!");
            }

            Flush();
            hasBegun = false;
        }

        private static int numVertsPerPrimitive(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.LineList:
                    return 2;
                case PrimitiveType.TriangleList:
                    return 3;
                default:
                    throw new Exception("PrimitiveDrawer doesn't support " + type.ToString());
            }
        }
    }
}
