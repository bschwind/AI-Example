using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AIProject.Messages
{
    public interface IRadioable
    {
        void ReceiveMessage(Message m);
        void AddMessage(Message m);
        Buffer<Message> GetOutbox();
        Buffer<Message> GetInbox();
        Color GetTeamColor();
    }
}
