using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AIProject.Messages;

namespace AIProject
{
    //A unit is one individual player and is part of a team in the game
    public class Unit : GameObject, IRadioable
    {
        private static int idCounter = 0;
        private static int resourceCapacity = 3; //How much of a "resource chunk" can a given unit hold?

        public Color TeamColor;
        public Vector3 Pos, Vel;
        public float Rotation;
        public Map Map { get; set; }

        private float turnSpeed = 4f;
        private float moveSpeed = 5f;
        private float viewRadius = 5f; //How far a unit can see
        private float radioRadius = 9f; //How far away a unit can talk to another unit of the same team
        private float maxCoolDownTime = 1.2f; //How long it takes before a unit can fire another shot
        private float currentCoolDown = 1.2f;
        private float strafeSpeed;  //negative = left, positive = right
        private float desiredAngle; //The angle we want the unit to eventually face
        private Circle bounds; //The minimal circle to enclose the unit
        private Circle viewCircle;
        private Circle radioCircle;
        private BehaviorManager bManager; //Manages all the behaviors for one unit
        private Buffer<GameObject> visibleObjects; //A list of all game objects that this unit can see
        private Buffer<GameObject> radioObjects;   //A list of all game objects that this unit can "talk" to
        private ResourceChunk bestChunk; //The best chunk this unit has recently seen
        private RadioTower mostRecentTower; //The most recent radio tower this unit has seen
        private Random rand;
        private int id;
        private int currentResources;
        private HQ headquarters;
        private Buffer<Message> inbox, outbox;
        private bool wantsToShoot = false;

        public Unit(Random r, HQ h)
        {
            rand = r;
            setUpBehaviors();
            visibleObjects = new Buffer<GameObject>(20);
            radioObjects = new Buffer<GameObject>(20);
            inbox = new Buffer<Message>(20);
            outbox = new Buffer<Message>(20);

            headquarters = h;
            id = idCounter;
            idCounter++;
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

        public void SetMostRecentTower(RadioTower t)
        {
            mostRecentTower = t;
        }

        public RadioTower GetMostRecentTower()
        {
            return mostRecentTower;
        }

        private void setUpBehaviors()
        {
            //This is where you can add new behaviors, or change the ordering
            //Behaviors at the end of the array take precedence over earlier
            //behaviors

            WanderBehavior wb = new WanderBehavior(this, rand);
            GoToResourceBehavior gtrb = new GoToResourceBehavior(this);
            TakeResourceBehavior trb = new TakeResourceBehavior(this);
            GoToHQBehavior gthqb = new GoToHQBehavior(this);
            DepositResourcesBehavior drb = new DepositResourcesBehavior(this);
            GoToTowerBehavior gttb = new GoToTowerBehavior(this);
            RestoreTowerBehavior rtb = new RestoreTowerBehavior(this);
            HelpTeammateBehavior htb = new HelpTeammateBehavior(this);
            AttackEnemyBehavior aeb = new AttackEnemyBehavior(this, rand);

            bManager = new BehaviorManager(new IBehavior[]{wb, gtrb, trb, gthqb, drb, gttb, rtb, htb, aeb});

            //In this example, aeb takes precedence over all other behaviors
            //In other words, attacking an enemy is more important than
            //anything else

            //The wandering behavior is only activated if there is nothing
            //else to do
        }

        public Unit(Vector3 Pos, Random r, HQ h) : this(r, h)
        {
            this.Pos = Pos;           
            bounds = new Circle(Pos, (float)Math.Sqrt(0.5f*0.5f + 0.25f * 0.25f));
            viewCircle = new Circle(Pos, viewRadius);
            radioCircle = new Circle(Pos, radioRadius);
        }

        public void ClearVisibleObjects()
        {
            visibleObjects.Clear();
            wantsToShoot = false;
        }

        public bool WantsToShoot()
        {
            return wantsToShoot;
        }

        //Flags this unit as "wanting to shoot" as long as it
        //isn't cooling down
        public void Shoot()
        {
            if (currentCoolDown >= maxCoolDownTime)
            {
                wantsToShoot = true;
                currentCoolDown = 0f;
            }
        }

        public void AddVisibleObject(GameObject g)
        {
            visibleObjects.Add(g);
        }

        public void AddVisibleObjects(Buffer<GameObject> list)
        {
            for (int i = 0; i < list.GetCount(); i++)
            {
                visibleObjects.Add(list[i]);
            }
        }

        public Buffer<GameObject> GetVisibleObjects()
        {
            return visibleObjects;
        }

        public void ClearRadioObjects()
        {
            radioObjects.Clear();
        }

        public void AddRadioObject(GameObject g)
        {
            radioObjects.Add(g);
        }

        public void AddRadioObjects(Buffer<GameObject> list)
        {
            for (int i = 0; i < list.GetCount(); i++)
            {
                radioObjects.Add(list[i]);
            }
        }

        public Buffer<GameObject> GetRadioObjects()
        {
            return radioObjects;
        }

        public void ReceiveMessage(Message m)
        {
            if (m.GetSender().Equals(this))
            {
                return;
            }
            inbox.Add(m);
        }

        public Buffer<Message> GetOutbox()
        {
            return outbox;
        }

        public Buffer<Message> GetInbox()
        {
            return inbox;
        }

        public void AddMessage(Message m)
        {
            outbox.Add(m);
        }

        public Color GetTeamColor()
        {
            return TeamColor;
        }

        private void UpdateBounds()
        {
            bounds = new Circle(Pos, (float)Math.Sqrt(0.5f * 0.5f + 0.25f * 0.25f));
            viewCircle = new Circle(Pos, viewRadius);
            radioCircle = new Circle(Pos, radioRadius);
        }

        public void Update(GameTime g)
        {
            wantsToShoot = false;
            UpdateCoolDown(g);

            outbox.Clear();

            UpdateRotation(g);
            UpdateMove(g);
            UpdateBounds();
            SetStrafeSpeed(0f);
            UpdateBehavior(g);
            UpdateBestChunk();
            UpdateMostRecentTower();

            inbox.Clear();
        }

        private void UpdateCoolDown(GameTime g)
        {
            if (currentCoolDown < maxCoolDownTime)
            {
                currentCoolDown += (float)g.ElapsedGameTime.TotalSeconds;
            }

            currentCoolDown = MathHelper.Clamp(currentCoolDown, 0f, maxCoolDownTime+1f);
        }

        private void UpdateMostRecentTower()
        {
            for (int i = 0; i < visibleObjects.GetCount(); i++)
            {
                RadioTower t = visibleObjects[i] as RadioTower;
                if (t != null && !t.IsActive())
                {
                    if (GameMath.TestCircleCircle(t.GetBounds(), this.GetViewCircle()))
                    {
                        mostRecentTower = t;
                    }
                }
            }
        }

        private void UpdateBestChunk()
        {
            //If we're near HQ, get info from it
            if (GameMath.TestCircleCircle(this.GetRadioCircle(), GetHQ().GetBounds()))
            {
                GetHQ().SetBestChunk(GetBestChunk());
                this.SetBestChunk(GetHQ().GetBestChunk());
            }

            for (int i = 0; i < visibleObjects.GetCount(); i++)
            {
                GameObject g = visibleObjects[i];
                if ((g as ResourceChunk) != null)
                {
                    ResourceChunk c = g as ResourceChunk;
                    if (GameMath.TestCircleCircle(c.GetBounds(), GetBounds()))
                    {
                        SetBestChunk(c);
                    }
                }
            }
        }

        private void UpdateBehavior(GameTime g)
        {
            bManager.UpdateBehavior(g);
        }

        public override Circle GetBounds()
        {  
            return bounds;
        }

        public Circle GetViewCircle()
        {
            return viewCircle;
        }

        public Circle GetRadioCircle()
        {
            return radioCircle;
        }

        public Vector3 GetDirection()
        {
            return new Vector3((float)Math.Cos(Rotation), (float)Math.Sin(Rotation), 0);
        }

        public HQ GetHQ()
        {
            return headquarters;
        }

        public int GetResourceCapacity()
        {
            return resourceCapacity;
        }

        public int GetCurrentResources()
        {
            return currentResources;
        }

        public bool ResourcesFull()
        {
            return currentResources >= resourceCapacity;
        }

        public bool HasResources()
        {
            return currentResources > 0;
        }

        public void TakeResources(ResourceChunk chunk)
        {
            if (ResourcesFull())
            {
                return;
            }

            while (!chunk.IsDepleted() && currentResources < resourceCapacity)
            {
                currentResources += chunk.TakeChunk();
            }
        }

        public int PlaceResources()
        {
            int temp = currentResources;
            currentResources = 0;
            return temp;
        }

        public bool Intersects(Unit other)
        {
            return bounds.Intersects(other.bounds);
        }

        public void Stop()
        {
            moveSpeed = 0;
        }

        public void Move()
        {
            if (GetHQ().WonTheGame())
            {
                //Move faster to celebrate!
                moveSpeed = 25;
            }
            else
            {
                moveSpeed = 8;
            }
        }

        public void SetStrafeSpeed(float speed)
        {
            strafeSpeed = speed;
        }

        private void UpdateMove(GameTime g)
        {
            float dt = (float)g.ElapsedGameTime.TotalSeconds;
            //Add forward speed
            Pos += new Vector3((float)Math.Cos(Rotation) * moveSpeed * dt, (float)Math.Sin(Rotation) * moveSpeed * dt, 0f);
            //Add strafe speed
            Pos += new Vector3((float)Math.Cos(Rotation - MathHelper.PiOver2) * strafeSpeed * dt, (float)Math.Sin(Rotation - MathHelper.PiOver2) * strafeSpeed * dt, 0f);
        }

        private void UpdateRotation(GameTime g)
        {
            float dt = (float)g.ElapsedGameTime.TotalSeconds;

            //There's probably a better way to do this, but this code
            //makes the unit rotate to the desired angle taking the
            //shorter rotation direction

            if (Math.Abs(Rotation - desiredAngle) < turnSpeed * dt)
            {
                Rotation = desiredAngle;
                return;
            }

            if (Rotation < desiredAngle)
            {
                if(desiredAngle-Rotation > MathHelper.Pi)
                {
                    Rotation -= turnSpeed * dt;
                }
                else
                {
                    Rotation += turnSpeed * dt;
                }
            }
            else
            {
                if (Rotation - desiredAngle > MathHelper.Pi)
                {
                    Rotation += turnSpeed * dt;
                }
                else
                {
                    Rotation -= turnSpeed * dt;
                }
            }

            Rotation = WrapRotation(Rotation);
        }

        //Keep our angle in the range of [0-2Pi]
        private float WrapRotation(float angle)
        {
            float newVal = 0;
            if (angle < 0)
            {
                newVal = MathHelper.TwoPi - Math.Abs(angle);
                return newVal;
            }
            else
            {
                newVal = angle % MathHelper.TwoPi;
                return newVal;
            }
        }

        public void RotateTo(float angle)
        {
            desiredAngle = Math.Abs(angle) % MathHelper.TwoPi;
        }

        public void RotateTo(Vector3 pos)
        {
            Vector2 dir = new Vector2(pos.X - Pos.X, pos.Y - Pos.Y);
            //dot = |v1|*|v2| cos(theta)
            //theta = acos(dot/|v1|*|v2|)
            float angle = (float)Math.Acos(Vector2.Dot(Vector2.UnitX, dir) / dir.Length());
            if (Vector2.UnitX.X * dir.Y - Vector2.UnitX.Y * dir.X < 0)
            {
                angle = -angle;
            }
            angle = WrapRotation(angle);
            RotateTo(angle);
        }

        public float GetDesiredRotation()
        {
            return desiredAngle;
        }

        //Draw a rotated triangle for our unit
        public void Draw(PrimitiveDrawer pd, GameTime g, Camera cam)
        {
            Vector3 v1 = new Vector3(0.5f, 0, 0);
            Vector3 v2 = new Vector3(-0.5f, -0.25f, 0);
            Vector3 v3 = new Vector3(-0.5f, 0.25f, 0);

            Matrix rotMatrix = Matrix.CreateRotationZ(Rotation);
            v1 = Vector3.Transform(v1, rotMatrix);
            v2 = Vector3.Transform(v2, rotMatrix);
            v3 = Vector3.Transform(v3, rotMatrix);

            pd.FillTriangle(Pos + v1, Pos + v2, Pos + v3, TeamColor);
        }

        public override bool Equals(object obj)
        {
            Unit u = obj as Unit;
            if (u != null)
            {
                return this.id == u.id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}
