using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class GoToHQBehavior : IBehavior
    {
        private Unit unit;

        public GoToHQBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {

        }

        public void Act(GameTime g)
        {
            unit.RotateTo(unit.GetHQ().GetBounds().Center);
            unit.Move();

            if (GameMath.TestCircleCircle(unit.GetBounds(), unit.GetHQ().GetBounds()))
            {
                unit.Stop();
            }
        }

        public bool ShouldTakeControl()
        {
            return unit.ResourcesFull();
        }
    }
}
