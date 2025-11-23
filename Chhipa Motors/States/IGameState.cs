using System.Windows.Forms;

namespace Chhipa_Motors.States
{
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Update();
        void OnKeyDown(KeyEventArgs e);
    }
}
