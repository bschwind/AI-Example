using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIProject.Messages
{
    public class FoundResourceMessage : Message
    {
        private ResourceChunk chunk;

        public FoundResourceMessage(Unit sender, ResourceChunk chunk)
            : base(sender)
        {
            this.chunk = chunk;
        }

        public ResourceChunk GetChunk()
        {
            return chunk;
        }
    }
}
