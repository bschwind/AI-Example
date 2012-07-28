using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AIProject.Messages;

namespace AIProject
{
    public class AttackEnemyBehavior : IBehavior
    {
        //This attack behavior results in really dumb battles
        //The AI units strafe back and forth, shooting at each other
        //until one gets a lucky shot

        private Unit unit;
        private Unit enemy;
        private Random rand;
        bool strafeLeft;

        public AttackEnemyBehavior(Unit u, Random r)
        {
            rand = r;
            unit = u;
        }

        public void Begin()
        {
            strafeLeft = rand.Next(0, 2) == 0 ? true : false;
        }

        public void Act(GameTime g)
        {
            unit.RotateTo(enemy.GetBounds().Center);
            unit.Stop();
            unit.SetStrafeSpeed(strafeLeft? -4 : 4);
            unit.Shoot();
        }

        public bool ShouldTakeControl()
        {
            //Only attack if we can see an enemy unit
            for (int i = 0; i < unit.GetVisibleObjects().GetCount(); i++)
            {
                Unit u = unit.GetVisibleObjects()[i] as Unit;
                if (u != null)
                {
                    if (!u.GetTeamColor().Equals(unit.GetTeamColor()))
                    {
                        enemy = u;
                        //Send out a message - "Help, I've found an enemy!"
                        unit.AddMessage(new FoundEnemyMessage(unit, enemy));
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
