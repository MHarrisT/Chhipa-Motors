using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Chhipa_Motors.Entities;
using Chhipa_Motors.Gameplay;

namespace Chhipa_Motors.Managers
{
    public class TrafficManager
    {
        readonly Random rand;
        readonly int minSpacing;
        readonly int[] laneX;
        readonly List<Vehicle> traffic = new List<Vehicle>();

        public IEnumerable<Vehicle> Traffic => traffic;

        public TrafficManager(Random rand, int minSpacing, int[] laneX)
        {
            this.rand = rand;
            this.minSpacing = minSpacing;
            this.laneX = laneX;
        }

        IMoveBehavior PickRandomBehavior()
        {
            int r = rand.Next(0, 100);
            if (r < 20) return new SlowMove();
            if (r < 80) return new NormalMove();
            return new FastMove();
        }

        public Vehicle CreateTraffic(Rectangle startRect, Image? defaultImg)
        {
            var v = new Vehicle(startRect, defaultImg, PickRandomBehavior());
            // assign lane X if lanes provided
            if (laneX != null && laneX.Length > 0)
                v.Rect.X = laneX[rand.Next(0, laneX.Length)];
            traffic.Add(v);
            return v;
        }

        // move all vehicles, return indices that passed bottom
        public List<int> UpdateAll(int baseMoveSpeed, int clientHeight)
        {
            var passed = new List<int>();
            for (int i = 0; i < traffic.Count; i++)
            {
                traffic[i].Move(baseMoveSpeed, rand);
                if (traffic[i].Rect.Y > clientHeight) passed.Add(i);
            }
            return passed;
        }

        public void Respawn(Vehicle car)
        {
            if (laneX.Length > 0)
                car.Rect.X = laneX[rand.Next(0, laneX.Length)];

            int spawnY = -car.Rect.Height - rand.Next(100, 300);

            int attempts = 0;
            while (attempts < 10)
            {
                bool conflict = false;
                foreach (var other in traffic)
                {
                    if (ReferenceEquals(other, car)) continue;
                    if (Math.Abs(car.Rect.X - other.Rect.X) < 70)
                    {
                        if (Math.Abs(spawnY - other.Rect.Y) < minSpacing) { conflict = true; break; }
                    }
                    else
                    {
                        if (Math.Abs(spawnY - other.Rect.Y) < minSpacing / 2) { conflict = true; break; }
                    }
                }
                if (!conflict) break;
                spawnY -= rand.Next(minSpacing, minSpacing + 200);
                if (attempts % 2 == 1 && laneX.Length > 0)
                {
                    car.Rect.X = laneX[rand.Next(0, laneX.Length)];
                }
                attempts++;
            }

            car.Rect.Y = spawnY;
            car.MoveBehavior = PickRandomBehavior();
        }
    }
}
