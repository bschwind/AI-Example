using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class DepositResourcesBehavior : IBehavior
    {
        private Unit unit;

        public DepositResourcesBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {

        }

        public void Act(GameTime g)
        {
            int amt = unit.PlaceResources();
            unit.GetHQ().DepositResources(amt);
            unit.GetHQ().SetBestChunk(unit.GetBestChunk());
            unit.SetBestChunk(unit.GetHQ().GetBestChunk());
        }

        public bool ShouldTakeControl()
        {
            //We should deposit resources if we have some and we're near the HQ
            return unit.HasResources() && GameMath.TestCircleCircle(unit.GetBounds(), unit.GetHQ().GetBounds());
        }
    }
}
