using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AIProject
{
    //Used for keeping track of the view information
    public struct Camera
    {
        public Matrix View, Projection;

        private Vector3 pos, vel;
        private Vector2 tilt;
        private float maxVel, dampingFactor, accFactor, zoomSpeed, tiltAmount;
        private bool tiltLocked;
        private GamePadState lastState;
        private GameObject target;

        public Camera(Vector3 pos, Matrix proj)
        {
            Projection = proj;
            View = Matrix.CreateLookAt(pos, pos + new Vector3(0, 0, -1), Vector3.Up);

            this.pos = pos;
            this.vel = Vector3.Zero;

            tilt = Vector2.Zero;

            maxVel = 100f;
            dampingFactor = 0.85f;
            accFactor = 25f;
            zoomSpeed = 10f;
            tiltAmount = 3f;
            tiltLocked = false;
            lastState = new GamePadState();
            target = null;
        }

        public void SetTarget(GameObject g)
        {
            target = g;
        }

        public Vector3 GetPos()
        {
            return pos;
        }

        public void Update(GameTime g)
        {
            float dt = (float)g.ElapsedGameTime.TotalSeconds;

            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyState = Keyboard.GetState();
            if (padState.IsButtonDown(Buttons.LeftStick) && lastState.IsButtonUp(Buttons.LeftStick))
            {
                tiltLocked = !tiltLocked;
            }

            if (!tiltLocked)
            {
                tilt = padState.ThumbSticks.Right * tiltAmount;
                tilt.X *= -1f;
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                tilt.X = -1f * tiltAmount;
                tilt.X *= -1f;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                tilt.X = 1f * tiltAmount;
                tilt.X *= -1f;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                tilt.Y = 1f * tiltAmount;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                tilt.Y = -1f * tiltAmount;
            }

            float zoomAcc = -padState.Triggers.Right + padState.Triggers.Left;
            if (keyState.IsKeyDown(Keys.OemPlus))
            {
                zoomAcc = -1f;
            }
            if (keyState.IsKeyDown(Keys.OemMinus))
            {
                zoomAcc = 1f;
            }

            zoomAcc *= zoomSpeed;
            Vector3 acc = new Vector3(padState.ThumbSticks.Left, zoomAcc);
            if(keyState.IsKeyDown(Keys.A))
            {
                acc.X = -1f;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                acc.X = 1f;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                acc.Y = 1f;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                acc.Y = -1f;
            }
            acc.X *= pos.Z;
            acc.Y *= pos.Z;

            vel += (acc * accFactor * dt);
            vel.X = MathHelper.Clamp(vel.X, -maxVel, maxVel);
            vel.Y = MathHelper.Clamp(vel.Y, -maxVel, maxVel);
            vel.Z = MathHelper.Clamp(vel.Z, -maxVel, maxVel);

            vel *= dampingFactor;

            pos += vel * dt;

            pos.Z = MathHelper.Clamp(pos.Z, 0.5f, 100f);

            if (target != null)
            {
                pos = new Vector3(target.GetBounds().Center.X, target.GetBounds().Center.Y, pos.Z);
            }

            Vector3 tiltDir = new Vector3(tilt, 1f);
            tiltDir.Normalize();
            Vector3 tiltPos = pos + (pos.Z * tiltDir);

            View = Matrix.CreateLookAt(tiltPos, pos + new Vector3(0, 0, -1), Vector3.Up);

            lastState = padState;
        }
    }
}
