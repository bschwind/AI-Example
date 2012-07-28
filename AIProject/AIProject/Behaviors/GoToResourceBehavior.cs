using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AIProject.Messages;

namespace AIProject
{
    public class GoToResourceBehavior : IBehavior
    {
        private Unit unit;
        private ResourceChunk desiredChunk;

        public GoToResourceBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {
            
        }

        public void Act(GameTime g)
        {
            unit.RotateTo(desiredChunk.GetBounds().Center);
            unit.Move();

            if (GameMath.TestCircleCircle(unit.GetBounds(), desiredChunk.GetBounds()))
            {
                unit.Stop();
            }
        }

        public bool ShouldTakeControl()
        {
            if (unit.ResourcesFull())
            {
                return false;
            }

            //If we can see a resource chunk, go to it
            for (int i = 0; i < unit.GetVisibleObjects().GetCount(); i++)
            {
                GameObject g = unit.GetVisibleObjects()[i];
                ResourceChunk c = g as ResourceChunk;
                if (c != null)
                {
                    if (GameMath.TestCircleCircle(unit.GetViewCircle(), c.GetBounds()))
                    {
                        desiredChunk = c;
                        //We'll send out a message that we've found resources
                        unit.AddMessage(new FoundResourceMessage(unit, desiredChunk));
                        return true;
                    }
                }
            }

            //Else, if we've received info from a teammate that there are resources around...
            Buffer<Message> inbox = unit.GetInbox();
            for (int i = 0; i < inbox.GetCount(); i++)
            {
                FoundResourceMessage frm = inbox[i] as FoundResourceMessage;
                if (frm != null)
                {
                    desiredChunk = frm.GetChunk();
                    unit.SetBestChunk(desiredChunk);
                    return true;
                }
            }

            return false;
        }
    }
}
