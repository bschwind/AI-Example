using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class GoToTowerBehavior : IBehavior
    {
        private Unit unit;
        private RadioTower tower;

        public GoToTowerBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {

        }

        public void Act(GameTime g)
        {
            unit.RotateTo(tower.GetBounds().Center);
            unit.Move();
        }

        public bool ShouldTakeControl()
        {
            //Return true if we have a radio tower in mind that isn't active
            if (unit.GetMostRecentTower() != null && !unit.GetMostRecentTower().IsActive() && unit.HasResources())
            {
                tower = unit.GetMostRecentTower();
                return true;
            }

            //or return true if we can see a radio tower
            for (int i = 0; i < unit.GetVisibleObjects().GetCount(); i++)
            {
                RadioTower t = unit.GetVisibleObjects()[i] as RadioTower;
                //If there's a tower I can see, and it's inactive, and I have resources...
                if (t != null && !t.IsActive() && unit.HasResources())
                {
                    tower = t;
                    return true;
                }
            }

            return false;
        }
    }
}
