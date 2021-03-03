using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    public enum GamePlayState
    {
        Playing = 0,
        Talking = 1,
        UI = 2,

    }
    public class GlobalManager : Singleton<GlobalManager>
    {
        public Action<GamePlayState> OnGamePlayStateChanged;
        private GamePlayState gamePlayState;

        public GamePlayState GamePlayState { get => gamePlayState; set { gamePlayState = value; OnGamePlayStateChanged(value); } }
        public Stack<GamePlayState> GamePlayStateStack=new Stack<GamePlayState>();
        public void EnterGamePlayState(GamePlayState gamePlayState)
        {
            Debug.Log($"EnterState {gamePlayState}");
            GamePlayStateStack.Push(gamePlayState);
            GamePlayState = gamePlayState;
        }
        public void QuitGamePlayState()
        {
            GamePlayStateStack.Pop();
            GamePlayState = GamePlayStateStack.Peek();
            Debug.Log($"QuitToState {gamePlayState}");
        }
    }
}
