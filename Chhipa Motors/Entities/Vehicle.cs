using System;
using System.Drawing;
using Chhipa_Motors.Gameplay;

namespace Chhipa_Motors.Entities
{
    // Small DTO representing a moving vehicle (player or traffic).
    public class Vehicle
    {
        public Rectangle Rect;
        public Image? Img;
        public IMoveBehavior MoveBehavior;

        public Vehicle(Rectangle rect, Image? img, IMoveBehavior? moveBehavior = null)
        {
            Rect = rect;
            Img = img;
            MoveBehavior = moveBehavior ?? new NormalMove();
        }

        // Move: baseSpeed provided by the caller and augmented by MoveBehavior.
        public void Move(int baseSpeed, Random rand)
        {
            int delta = MoveBehavior?.GetSpeedDelta(rand) ?? 0;
            Rect.Y += baseSpeed + delta;
        }
    }
}
