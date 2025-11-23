using System;

namespace Chhipa_Motors.Gameplay
{
    public class SlowMove : IMoveBehavior { public int GetSpeedDelta(Random rand) => -2; }
    public class NormalMove : IMoveBehavior { public int GetSpeedDelta(Random rand) => -4; }
    public class FastMove : IMoveBehavior { public int GetSpeedDelta(Random rand) => -5; }
    public class VariableMove : IMoveBehavior
    {
        int min, max;
        public VariableMove(int minDelta = -1, int maxDelta = 3) { min = minDelta; max = maxDelta; }
        public int GetSpeedDelta(Random rand) => rand.Next(min, max + 1);
    }
}
