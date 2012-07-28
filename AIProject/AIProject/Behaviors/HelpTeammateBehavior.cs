using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AIProject.Messages;

namespace AIProject
{
    public class HelpTeammateBehavior : IBehavior
    {
        private Unit unit, enemy;

        public HelpTeammateBehavior(Unit u)
        {
            unit = u;
        }

        public void Begin()
        {

        }

        public void Act(GameTime g)
        {
            unit.RotateTo(enemy.GetBounds().Center);
            unit.Move();
        }

        public bool ShouldTakeControl()
        {
            //We've received a request for help
            Buffer<Message> inbox = unit.GetInbox();
            for (int i = 0; i < inbox.GetCount(); i++)
            {
                FoundEnemyMessage fem = inbox[i] as FoundEnemyMessage;
                if (fem != null)
                {
                    enemy = fem.GetEnemy();
                    return true;
                }
            }

            return false;
        }
    }
}
