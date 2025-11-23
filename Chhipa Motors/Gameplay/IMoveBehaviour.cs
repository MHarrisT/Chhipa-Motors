using System;

namespace Chhipa_Motors.Gameplay
{
    public interface IMoveBehavior
    {
        int GetSpeedDelta(Random rand);
    }
}
