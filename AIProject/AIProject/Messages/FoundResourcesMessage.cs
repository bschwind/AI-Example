using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject.Messages
{
    public class FoundResourcesMessage : Message
    {
        private Vector3 pos;
        private int amount;

        public FoundResourcesMessage(Unit sender, ResourceChunk chunk)
            : base(sender)
        {
            this.pos = chunk.GetBounds().Center;
            amount = chunk.GetAmount();
        }
    }
}
