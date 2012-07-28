using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public interface IBehavior
    {
        //A behavior is very simple. It provides conditions
        //for when it should fire, a method for initialization
        //of the behavior (Begin()), and a main update method
        //for the behavior (Act())

        bool ShouldTakeControl();
        void Act(GameTime g);
        void Begin();
    }
}
