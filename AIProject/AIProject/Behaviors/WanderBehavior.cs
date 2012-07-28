using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class WanderBehavior : IBehavior
    {
        private Unit unit;
        private Vector3 destination;
        private Random rand;
        private int closeEnough = 3;  //How close do we have to be to be considered at the destination?

        public WanderBehavior(Unit u, Random r)
        {
            unit = u;
            rand = r;
        }

        public void Begin()
        {
            generateDestination();
        }

        private void generateDestination()
        {
            if (unit.GetBestChunk() != null)
            {
                destination = unit.GetBestChunk().GetBounds().Center;
                return;
            }

            destination = new Vector3(rand.Next(1, unit.Map.GetWidth()-1),
                                      rand.Next(1, unit.Map.GetHeight()-1),
                                      0f);
        }

        public void Act(GameTime g)
        {
            unit.RotateTo(destination);
            unit.Move();

            if ((destination - unit.Pos).LengthSquared() < closeEnough * closeEnough)
            {
                generateDestination();
            }
        }

        public bool ShouldTakeControl()
        {
            return true;
        }
    }
}
