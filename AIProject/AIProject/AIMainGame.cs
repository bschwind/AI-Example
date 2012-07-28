using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AIProject.Messages;

namespace AIProject
{
    //This is where the main game logic occurs
    public class AIMainGame
    {
        private const int gridLength = 200;

        private const int unitsPerTeam = 5;

        private Map map;
        private Camera cam;
        private PrimitiveDrawer pd;
        List<Unit> units;
        List<ResourceChunk> resources;
        List<RadioTower> towers;
        List<HQ> hqs;
        List<ResourceChunk> removeChunks; //Chunks to delete on a given frame
        List<Bullet> bullets;
        Random rand;

        Color[] teamColors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.OrangeRed, Color.White, Color.LightSkyBlue, Color.HotPink, Color.Brown };

        public AIMainGame(int teamCount, GraphicsDevice gd)
        {
            rand = new Random();

            pd = new PrimitiveDrawer(gd);
            Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), (float)gd.Viewport.Width / gd.Viewport.Height, 0.1f, 1000f);

            cam = new Camera(new Vector3(gridLength/2, gridLength/2, 30), proj);
            map = new Map(gd, gridLength, gridLength);

            if (teamCount > teamColors.Length)
            {
                teamCount = teamColors.Length;
            }

            //Initialize all the game objects
            units = new List<Unit>();
            resources = new List<ResourceChunk>();
            towers = new List<RadioTower>();
            removeChunks = new List<ResourceChunk>();
            hqs = new List<HQ>();
            bullets = new List<Bullet>(200);

            initializeTeams(teamCount);
            initializeResources();
            initializeTowers();
        }

        private void initializeTowers()
        {
            //Add a random number of radio towers to the game
            int towerCount = rand.Next(10, 20);
            for (int i = 0; i < towerCount; i++)
            {
                towers.Add(new RadioTower(randomPointInGrid()));
            }
        }

        private void initializeResources()
        {
            //Add a random number of resource chunks to the game
            int resourceCount = rand.Next(50,90);
            for (int i = 0; i < resourceCount; i++)
            {
                resources.Add(new ResourceChunk(rand.Next(1, 20), randomPointInGrid()));
            }
        }

        private Vector3 randomPointInGrid()
        {
            return new Vector3(rand.Next(1, map.GetWidth()-1),
                                      rand.Next(1, map.GetHeight()-1),
                                      0f);
        }

        private void initializeTeams(int teamCount)
        {
            for (int i = 0; i < teamCount; i++)
            {
                //This code initializes each team member in a circle
                //around their HQ

                float angle = i * MathHelper.TwoPi / teamCount;
                angle += MathHelper.PiOver4;

                Vector2 center = new Vector2(gridLength / 2, gridLength / 2);

                Vector2 teamSpawn = center + new Vector2((float)Math.Cos(angle) * (gridLength/2f) * 0.9f, (float)Math.Sin(angle) * (gridLength/2f) * 0.9f);
                HQ hq = new HQ(new Vector3(teamSpawn, 0), teamColors[i], rand, map);
                hqs.Add(hq);

                for (int j = 0; j < unitsPerTeam; j++)
                {
                    Vector2 spawnPoint = randomPointInCircle(teamSpawn, gridLength/20f);
                    Unit u = new Unit(new Vector3(spawnPoint, 0), rand, hqs[i]);
                    u.Map = map;
                    u.TeamColor = teamColors[i];
                    u.RotateTo(new Vector3(gridLength / 2f, gridLength / 2f, 0f));
                    units.Add(u);
                    hq.AddUnit(u);
                }
            }
        }

        private Vector2 randomPointInCircle(Vector2 center, float radius)
        {
            //Gets a random point in a circle
            //Note: This doesn't give an even distribution, as most points
            //will clump towards the center, but hey, good enough

            float angle = (float)rand.NextDouble() * MathHelper.TwoPi;
            float randomRadius = (float)rand.NextDouble() * radius;

            return center + new Vector2((float)Math.Cos(angle) * randomRadius, (float)Math.Sin(angle) * randomRadius);
        }

        public void Update(GameTime g)
        {
            cam.Update(g);

            //Clear the grid, and re-register all the objects
            map.ClearGridData();

            //Clear out depleted resource chunks
            ResourceChunk depletedChunk = null;
            foreach (ResourceChunk chunk in resources)
            {
                if (chunk.IsDepleted())
                {
                    depletedChunk = chunk;
                    continue;
                }
                //Register non-depleted resource chunks
                map.RegisterGridObject(chunk);
            }
            //Remove depleted chunks
            //Usually only one chunk gets depleted in a given frame
            resources.Remove(depletedChunk);

            //Register radio towers
            foreach (RadioTower t in towers)
            {
                map.RegisterGridObject(t);
            }
            
            //Register units on the grid
            foreach (Unit u in units)
            {
                map.RegisterGridObject(u);
            }

            //Set each unit's list of objects it can see and objects it can radio
            foreach (Unit u in units)
            {
                u.ClearVisibleObjects();
                u.ClearRadioObjects();
                u.AddVisibleObjects(map.GetNeighbors(u.GetViewCircle(), u));
                u.AddRadioObjects(map.GetNeighbors(u.GetRadioCircle(), u));

                u.Update(g);
                //Keep the unit in bounds
                u.Pos.X = MathHelper.Clamp(u.Pos.X, 0f, map.GetWidth() - 1f);
                u.Pos.Y = MathHelper.Clamp(u.Pos.Y, 0f, map.GetHeight() - 1f);

                //Add new bullets this unit has fired
                if (u.WantsToShoot())
                {
                    bullets.Add(new Bullet(u.Pos, u.GetDirection(), u.GetTeamColor()));
                }
            }

            //Update bullets and kill units accordingly
            foreach (Bullet b in bullets)
            {
                if (!b.IsActive())
                {
                    continue;
                }
                b.Update(g);
                Buffer<GameObject> gos = map.GetNeighbors(b.GetBounds(), null);
                for (int i = 0; i < gos.GetCount(); i++)
                {
                    Unit u = gos[i] as Unit;
                    if (u != null && !b.GetTeamColor().Equals(u.GetTeamColor()))
                    {
                        units.Remove(u);
                        u.GetHQ().RemoveUnit(u);
                        b.Deactivate();
                    }
                }

                Vector3 bPos = b.GetBounds().Center;
                if (bPos.X < 0 || bPos.X > map.GetWidth() || bPos.Y < 0 || bPos.Y > map.GetHeight())
                {
                    b.Deactivate();
                }
            }

            //Update messaging - take messages from each unit's outbox and distribute to nearby units
            foreach (Unit u in units)
            {
                Buffer<Message> outbox = u.GetOutbox();
                Buffer<GameObject> radioNeighbors = u.GetRadioObjects();
                //Loop through each message in the outbox of each unit,
                //and send it to every unit on our team within radio distance
                for (int i = 0; i < outbox.GetCount(); i++)
                {
                    for (int j = 0; j < radioNeighbors.GetCount(); j++)
                    {
                        IRadioable r = radioNeighbors[j] as IRadioable;
                        if (r != null && r.GetTeamColor().Equals(u.GetTeamColor()))
                        {
                            r.ReceiveMessage(outbox[i]);
                        }
                    }
                }
            }

            //Get outbox of each unit within a radio tower and distribute to all other units
            foreach (RadioTower t in towers)
            {
                if (!t.IsActive())
                {
                    continue;
                }

                Buffer<GameObject> objects = map.GetNeighbors(t.GetCommCircle(), null);
                for (int i = 0; i < objects.GetCount(); i++)
                {
                    Unit u = objects[i] as Unit;
                    //If the unit and tower are of different teams, skip
                    if (u != null && !u.GetTeamColor().Equals(t.GetTeamColor()))
                    {
                        continue;
                    }

                    if (u != null)
                    {
                        Buffer<Message> outbox = u.GetOutbox();
                        for (int j = 0; j < outbox.GetCount(); j++)
                        {
                            for (int k = 0; k < objects.GetCount(); k++)
                            {
                                Unit receiver = objects[k] as Unit;
                                if (receiver != null && receiver.GetTeamColor().Equals(t.GetTeamColor()))
                                {
                                    receiver.ReceiveMessage(outbox[j]);
                                }
                            }
                        }
                    }
                }
            }

            HQ deadHQ = null;
            foreach (HQ h in hqs)
            {
                h.Update(g);
                for (int i = 0; i < h.GetNewUnits().GetCount(); i++)
                {
                    units.Add(h.GetNewUnits()[i]);
                }

                if (h.LostTheGame())
                {
                    deadHQ = h;
                }
            }

            if (deadHQ != null)
            {
                hqs.Remove(deadHQ);
            }

            //Check for winning condition
            if (hqs.Count == 1)
            {
                Console.WriteLine("Team " + hqs[0].GetTeamColor() + " won the game!");
                hqs[0].SetWinner();
            }
        }

        public void Draw(GameTime g)
        {
            map.Draw(pd, g, cam);

            pd.Begin(PrimitiveType.TriangleList, cam);
            foreach (Unit u in units)
            {
                u.Draw(pd, g, cam);
            }
            pd.End();

            pd.Begin(PrimitiveType.LineList, cam);
            foreach (ResourceChunk c in resources)
            {
                c.Draw(pd);
            }
            foreach (RadioTower r in towers)
            {
                r.Draw(pd);
            }
            foreach (HQ h in hqs)
            {
                h.Draw(pd);
            }
            foreach (Bullet b in bullets)
            {
                b.Draw(pd);
            }
            //Debug drawing for units' viewing circles
            /*foreach (Unit u in units)
            {
                pd.DrawCircle(u.GetViewCircle(), Color.Red); 
            }*/
            pd.End();
        }
    }
}
