using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class TakeResourceBehavior : IBehavior
    {
        private Unit unit;
        private ResourceChunk desiredChunk;

        public TakeResourceBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {

        }

        public void Act(GameTime g)
        {
            unit.TakeResources(desiredChunk);
            unit.SetBestChunk(desiredChunk);
        }

        public bool ShouldTakeControl()
        {
            for (int i = 0; i < unit.GetVisibleObjects().GetCount(); i++)
            {
                GameObject g = unit.GetVisibleObjects()[i];
                ResourceChunk c = g as ResourceChunk;
                if (c != null)
                {
                    if (GameMath.TestCircleCircle(unit.GetBounds(), c.GetBounds()) && !c.IsDepleted() && !unit.ResourcesFull())
                    {
                        desiredChunk = c;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
