using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIProject.Messages
{
    //A message is how units pass information to each other
    //Inherit from this class to create a new message type
    //It's not very scalable, sadly...
    public class Message
    {
        private Unit sender;

        public Message(Unit sender)
        {
            this.sender = sender;
        }

        public Unit GetSender()
        {
            return sender;
        }
    }
}
