using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    
    // Start is called before the first frame update
    public GameObject BoardObject;
    public GameObject ViewObject;
    public GameObject GameState;
    public GameState gameState;

    private Board board;
    private View view;
   
    void Start()
    {
        gameState = GameState.GetComponent<GameState>();
        gameState.state = global::GameState.State.Idle;
        board = BoardObject.GetComponent<Board>();
        view = ViewObject.GetComponent<View>();
        board.SetUp();
        view.InitializeView(board.boardOfIDs);
    }

    // Update is called once per frame
    void Update()
    {
        if (view.swapPositions.Count == 2 && gameState.state == global::GameState.State.Idle)
        {
            //Debug.Log("clear match");
            int itemColumn = view.swapPositions[0].column;
            int itemRow = view.swapPositions[0].row;
            int otherItemColumn = view.swapPositions[1].column;
            int otherItemRow = view.swapPositions[1].row;

            if (view.allItems[itemColumn, itemRow].GetComponent<Item>()._swappingDone && view.allItems[otherItemColumn, otherItemRow].GetComponent<Item>()._swappingDone)
            {
                view.allItems[itemColumn, itemRow].GetComponent<Item>()._swappingDone = false;
                view.allItems[otherItemColumn, otherItemRow].GetComponent<Item>()._swappingDone = false;
                board.UpdateBoardAfterSwapping(view.swapPositions);                                                                                                                                                                                                                                           
                if (board.FindMatch())
                {
                    view.swapPositions.Clear();
                    int[,] copyOfBoardOfIDs = new int [board.width, board.height];
                    for (int i = 0; i < board.width; i++)
                    {
                        for (int j = 0; j < board.height; j++)
                        {
                            copyOfBoardOfIDs[i, j] = board.boardOfIDs[i, j];
                        }
                    }
                    board.UpdateBoardAfterMovingItemDown();
                    StartCoroutine(view.ClearMatchItem(copyOfBoardOfIDs, board.boardOfIDs));
                } 
                else                                                                                                         
                {
                    gameState.state = global::GameState.State.SwapBack;
                    board.UpdateBoardAfterSwapping(view.swapPositions);
                    StartCoroutine(view.SwapBackIfNotMatch());    
                }
                //
            }
        } else if (gameState.state == global::GameState.State.CheckMatchAgain)
        {
            if (board.FindMatch())
            {
                int[,] copyOfBoardOfIDs = new int [board.width, board.height];
                //int [,] boardOfIDsWithNewItem = new int [board.width, board.height];
                for (int i = 0; i < board.width; i++)
                {
                    for (int j = 0; j < board.height; j++)
                    {
                        copyOfBoardOfIDs[i, j] = board.boardOfIDs[i, j];
                    }
                }
                board.UpdateBoardAfterMovingItemDown();
                /*for (int i = 0; i < board.width; i++)
                {
                    for (int j = 0; j < board.height; j++)
                    {
                        if (board.boardOfIDs[i, j] == addNewItemId)
                        {
                            int idOfItem = Random.Range(0, itemLength);
                            boardOfIDsWithNewItem[i, j] = idOfItem;
                        }
                    }
                }*/
                gameState.state = global::GameState.State.PlayingView;
                StartCoroutine(view.ClearMatchItem(copyOfBoardOfIDs, board.boardOfIDs));
            } else
            {
                gameState.state = global::GameState.State.Idle;
            }
        }
    }

}
    
