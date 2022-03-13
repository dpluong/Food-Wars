using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // Start is called before the first frame update
    public enum State
    {
        Idle,
        CheckMatch,
        SwapBack,
        MoveItemDown,
        CheckMatchAgain,
        PlayingView
    }
    public State state;

}
