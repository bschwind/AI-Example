using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIProject
{
    public class Bullet : GameObject
    {
        private Circle bounds;
        private float speed = 12f;
        private Vector3 pos, dir;
        private Color teamColor;
        bool isActive;
        float timeout = 2f;
        float currentTime = 0f;

        public Bullet(Vector3 pos, Vector3 dir, Color teamColor)
            : base()
        {
            this.pos = pos;
            this.dir = dir;
            this.teamColor = teamColor;
            updateBounds();
            isActive = true;
        }

        public Color GetTeamColor()
        {
            return teamColor;
        }

        public void Update(GameTime g)
        {
            currentTime += (float)g.ElapsedGameTime.TotalSeconds;
            if (currentTime >= timeout)
            {
                Deactivate();
                return;
            }
            pos += (dir * speed * (float)g.ElapsedGameTime.TotalSeconds);
            updateBounds();
        }

        public void Deactivate()
        {
            isActive = false;
        }

        public bool IsActive()
        {
            return isActive;
        }

        private void updateBounds()
        {
            bounds = new Circle(pos, 0.2f);
        }

        public override Circle GetBounds()
        {
            return bounds;
        }

        public void Draw(PrimitiveDrawer pd)
        {
            if (!isActive)
            {
                return;
            }
            pd.DrawCircle(bounds, Color.DarkGoldenrod);
        }
    }
}
