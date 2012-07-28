using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class ResourceChunk : GameObject
    {
        private int amount; //Value between 1 and 20
        private Circle bounds;
        private Vector3 pos;

        public ResourceChunk(int amount, Vector3 pos)
        {
            this.amount = amount;
            this.pos = pos;
            UpdateBounds();
        }

        public int TakeChunk()
        {
            amount--;
            UpdateBounds();
            return 1;
        }

        private void UpdateBounds()
        {
            bounds = new Circle(pos, amount / 4f);
        }

        public bool IsDepleted()
        {
            return amount <= 0;
        }

        public int GetAmount()
        {
            return amount;
        }

        public override Circle GetBounds()
        {
            return bounds;
        }

        public void Draw(PrimitiveDrawer pd)
        {
            pd.DrawCircle(GetBounds(), Color.Tan);
        }
    }
}
