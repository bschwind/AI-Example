using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class RestoreTowerBehavior : IBehavior
    {
        private Unit unit;
        private RadioTower tower;

        public RestoreTowerBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {

        }

        public void Act(GameTime g)
        {
            int amt = unit.PlaceResources();

            tower.DepositResources(amt, unit.TeamColor);
        }

        public bool ShouldTakeControl()
        {
            //Return true if we're intersecting a radio tower
            for (int i = 0; i < unit.GetVisibleObjects().GetCount(); i++)
            {
                RadioTower t = unit.GetVisibleObjects()[i] as RadioTower;
                if (t != null && !t.IsActive() && unit.HasResources())
                {
                    if (GameMath.TestCircleCircle(t.GetBounds(), unit.GetBounds()))
                    {
                        tower = t;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
