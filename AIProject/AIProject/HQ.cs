using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class HQ : GameObject
    {
        private Vector3 pos;
        private Color teamColor;
        private static int hqRadius = 2;
        private Circle bounds;
        private Vector3 v1, v2, v3; //Points for triangle on HQ
        private int resources;
        private Buffer<Unit> newUnits;
        private int resourcesPerUnit = 4;
        private Random rand;
        private Map map;
        private ResourceChunk bestChunk;
        private List<Unit> units;
        bool lostGame = false;
        bool wonGame = false;

        public HQ(Vector3 pos, Color teamColor, Random r, Map m)
        {
            this.map = m;
            this.rand = r;
            this.pos = pos;
            this.teamColor = teamColor;
            bounds = new Circle(pos, hqRadius);
            newUnits = new Buffer<Unit>(5);

            v1 = new Vector3(0, hqRadius, 0);
            v2 = Vector3.Transform(v1, Matrix.CreateRotationZ(MathHelper.ToRadians(360f/3)));
            v3 = Vector3.Transform(v2, Matrix.CreateRotationZ(MathHelper.ToRadians(360f / 3)));
            v1 += pos;
            v2 += pos;
            v3 += pos;

            units = new List<Unit>();
        }

        public void SetWinner()
        {
            wonGame = true;
        }

        public bool WonTheGame()
        {
            return wonGame;
        }

        public Color GetTeamColor()
        {
            return teamColor;
        }

        public bool LostTheGame()
        {
            return lostGame;
        }

        public void AddUnit(Unit u)
        {
            units.Add(u);
        }

        public void RemoveUnit(Unit u)
        {
            units.Remove(u);
        }

        public void SetBestChunk(ResourceChunk c)
        {
            if (c == null)
            {
                return;
            }

            if (bestChunk == null)
            {
                bestChunk = c;
                return;
            }

            if (c.GetAmount() > bestChunk.GetAmount())
            {
                bestChunk = c;
            }
        }

        public ResourceChunk GetBestChunk()
        {
            if (bestChunk != null && bestChunk.GetAmount() <= 0)
            {
                bestChunk = null;
            }
            return bestChunk;
        }

        public override Circle GetBounds()
        {
            return bounds;
        }

        public void DepositResources(int r)
        {
            resources += r;
        }

        public void Update(GameTime g)
        {
            newUnits.Clear();
            while (resources >= resourcesPerUnit && newUnits.GetCount() <= newUnits.GetCapacity())
            {
                resources -= resourcesPerUnit;
                Unit u = new Unit(this.pos, rand, this);
                u.Map = map;
                u.TeamColor = teamColor;
                newUnits.Add(u);
                units.Add(u);
            }

            if (units.Count <= 0)
            {
                lostGame = true;
            }
        }

        public Buffer<Unit> GetNewUnits()
        {
            return newUnits;
        }

        public void Draw(PrimitiveDrawer pd)
        {
            pd.DrawCircle(bounds, teamColor);
            pd.DrawTriangle(v1, v2, v3, teamColor);
        }
    }
}
