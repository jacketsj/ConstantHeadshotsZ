using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ConstantHeadshotsZ
{
    public class Circle
    {
        public Vector2 position;
        public float radius;

        public Circle(Vector2 newPosition, float newRadius)
        {
            position = newPosition;
            radius = newRadius;
        }

        public bool Intersects(Circle intersectingCircle)
        {
            float dx = (intersectingCircle.position.X - position.X);
            float dy = (intersectingCircle.position.Y - position.Y);
            float radii = radius + intersectingCircle.radius;
            if ((dx * dx) + (dy * dy) < radii * radii)
            {
                return true;
            }
            return false;
        }

        public bool Intersects(Rectangle intersectingRec)
        {
            Vector2 circleDistance = new Vector2();
            circleDistance.X = position.X - intersectingRec.X;
            circleDistance.Y = position.Y - intersectingRec.Y;

            if (circleDistance.X > (intersectingRec.Width / 2 + radius / 2))
            {
                return false;
            }
            if (circleDistance.Y > (intersectingRec.Height / 2 + radius / 2))
            {
                return false;
            }

            if (circleDistance.X <= (intersectingRec.Width / 2))
            {
                return true;
            }
            if (circleDistance.Y <= (intersectingRec.Height / 2))
            {
                return true;
            }

            float cornerDistance_sq = (float)Math.Pow((circleDistance.X - intersectingRec.Width / 2), 2) + (float)Math.Pow((circleDistance.Y - intersectingRec.Height / 2), 2);

            return (cornerDistance_sq <= (Math.Pow(radius * 2, 2)));
        }
    }
}
