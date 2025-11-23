using System;
using System.Windows.Forms;

namespace Chhipa_Motors.States
{
    // Small state implementations that call public methods on the Game form (context)
    public class MenuState : IGameState
    {
        readonly Game context;
        public MenuState(Game ctx) { context = ctx; }
        public void Enter() { context.ShowMenuPanel(); context.StopTimer(); }
        public void Exit() { context.HideMenuPanel(); }
        public void Update() { }
        public void OnKeyDown(KeyEventArgs e) { }
    }

    public class CarSelectState : IGameState
    {
        readonly Game context;
        public CarSelectState(Game ctx) { context = ctx; }
        public void Enter() { context.ShowCarSelectPanel(); context.StopTimer(); }
        public void Exit() { context.HideCarSelectPanel(); }
        public void Update() { }
        public void OnKeyDown(KeyEventArgs e) { }
    }

    public class PlayingState : IGameState
    {
        readonly Game context;
        public PlayingState(Game ctx) { context = ctx; }
        public void Enter() { context.HideMenuPanel(); context.HideCarSelectPanel(); context.HideGameOverPanel(); context.StartTimer(); }
        public void Exit() { context.StopTimer(); }
        public void Update() { }
        public void OnKeyDown(KeyEventArgs e) { context.HandlePlayerInput(e); }
    }

    public class GameOverState : IGameState
    {
        readonly Game context;
        public GameOverState(Game ctx) { context = ctx; }
        public void Enter() { context.ShowGameOverPanel(); context.StopTimer(); }
        public void Exit() { context.HideGameOverPanel(); }
        public void Update() { }
        public void OnKeyDown(KeyEventArgs e) { }
    }
}
