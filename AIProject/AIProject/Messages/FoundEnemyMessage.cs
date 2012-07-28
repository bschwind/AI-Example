using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIProject.Messages
{
    public class FoundEnemyMessage : Message
    {
        private Unit enemy;

        public FoundEnemyMessage(Unit sender, Unit enemy)
            : base(sender)
        {
            this.enemy = enemy;
        }

        public Unit GetEnemy()
        {
            return enemy;
        }
    }
}
