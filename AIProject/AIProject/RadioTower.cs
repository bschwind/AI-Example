using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AIProject.Messages;

namespace AIProject
{
    public class RadioTower : GameObject, IRadioable
    {
        private Circle bounds;
        private Circle commCircle;
        private bool isActive;
        private int currentResources;
        private static int amountToRestore = 5;
        private Color teamColor;
        private Buffer<Message> inbox;
        private Buffer<Message> outbox;

        public RadioTower(Vector3 pos)
        {
            bounds = new Circle(pos, 2);
            commCircle = new Circle(pos, 25);

            inbox = new Buffer<Message>(20);
            outbox = new Buffer<Message>(20);
        }

        public void AddMessage(Message m)
        {
            inbox.Add(m);
        }

        public Buffer<Message> GetInbox()
        {
            return inbox;
        }

        public Buffer<Message> GetOutbox()
        {
            return outbox;
        }

        public Color GetTeamColor()
        {
            return teamColor;
        }

        public void DepositResources(int amt, Color teamColor)
        {
            currentResources += amt;
            if (currentResources >= amountToRestore)
            {
                Activate();
                this.teamColor = teamColor;
            }
        }

        public void ReceiveMessage(Message m)
        {
            inbox.Add(m);
        }        

        public override Circle GetBounds()
        {
            return bounds;
        }

        public Circle GetCommCircle()
        {
            return commCircle;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Update(GameTime g)
        {
            if (!isActive)
            {
                return;
            }

            //Relay inbox messages to other units and towers within radius
            //Assumes the inbox has been populated before the call to this.Update
            outbox.Clear();

            for (int i = 0; i < inbox.GetCount(); i++)
            {
                outbox.Add(inbox[i]);
            }

            inbox.Clear();
        }

        public void Draw(PrimitiveDrawer pd)
        {
            Vector3 v1 = new Vector3(0, 2f, 0);
            Vector3 v2 = new Vector3(0.5f, -0.25f, 0);
            Vector3 v3 = new Vector3(-0.5f, -0.25f, 0);
            Vector3 pos = bounds.Center;

            Color c = isActive ? teamColor : Color.Gray;

            pd.DrawTriangle(pos + v1, pos + v2, pos + v3, c);
            pd.DrawCircle(new Circle(pos + v1, 0.5f), c);
            pd.DrawCircle(commCircle, c);
        }
    }
}
