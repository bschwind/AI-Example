using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class BehaviorManager
    {
        private IBehavior[] behaviors;
        private IBehavior currentBehavior, lastBehavior;

        public BehaviorManager(IBehavior[] behaviors)
        {
            this.behaviors = behaviors;
        }

        public void UpdateBehavior(GameTime g)
        {
            //Simply loop through the list of behaviors, looking for the
            //last behavior which wants to activate. Order matters, so 
            //behaviors at the end of the list will always override
            //behaviors before it

            for (int i = 0; i < behaviors.Length; i++)
            {
                if (behaviors[i].ShouldTakeControl())
                {
                    currentBehavior = behaviors[i];
                }
            }

            if (currentBehavior != null && !currentBehavior.Equals(lastBehavior))
            {
                //If we have just changed behaviors, allow the new behavior to run
                //initialization logic
                currentBehavior.Begin();
            }

            if (currentBehavior != null)
            {
                currentBehavior.Act(g);
            }

            lastBehavior = currentBehavior;
        }
    }
}
