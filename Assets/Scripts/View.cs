using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    const int removedItemId = -100;
    const int addNewItemId = -200;
    const int generateItemRow = 10;
    const int itemLength = 3;
    const int appleBomb = 3;
    const int bomb3x3Id = 14;

    public int width;
    public int height;
    public GameObject GameState;
    public GameObject tilePrefab;
    public GameObject[] items;
    public GameObject[] bombs;
    public GameObject bomb3x3;
    public GameObject[,] allItems;
    public List<(int column, int row)> swapPositions = new List<(int column, int row)>();
    public GameState gameState;

    private BackgroundTile[,] allTiles;
    

    // Start is called before the first frame update

    //Flags
    public bool _swappedBack = false;
    

    void Start() {
        gameState = GameState.GetComponent<GameState>();
    }

    public void InitializeView(int [,] boardOfIds)
    {
        allTiles = new BackgroundTile[width, height];
        allItems = new GameObject[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                int itemToUse = boardOfIds[i, j];
                GameObject item = Instantiate(items[itemToUse], tempPosition, Quaternion.identity);
                item.transform.parent = this.transform;
                item.GetComponent<Item>().id = boardOfIds[i, j];
                //item.name = "( " + i + ", " + j + " )";
                allItems[i, j] = item;
            }
        }
    }

    public IEnumerator ClearMatchItem(int [,] boardOfIds, int [,] newBoardOfIds)
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (boardOfIds[i, j] == removedItemId || (boardOfIds[i, j] >= appleBomb && allItems[i, j].transform.name != "Bomb"))
                {
                    Destroy(allItems[i, j]);
                }
            }
        }
        //swapPositions.Clear();
        string board = "";
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                board += boardOfIds[j, i] + " ";
            }
            board += "\n";
        }
        Debug.Log(board);
        
        StartCoroutine(MoveItemDown(boardOfIds, newBoardOfIds));
    }

    public IEnumerator SwapBackIfNotMatch()
    {
        yield return new WaitForSeconds(0.3f);
        _swappedBack = true;
        int itemColumn = swapPositions[0].column;
        int itemRow = swapPositions[0].row;
        int otherItemColumn = swapPositions[1].column;
        int otherItemRow = swapPositions[1].row;
        allItems[itemColumn, itemRow].GetComponent<Item>().column = otherItemColumn;
        allItems[itemColumn, itemRow].GetComponent<Item>().row = otherItemRow;
        allItems[otherItemColumn, otherItemRow].GetComponent<Item>().column = itemColumn;
        allItems[otherItemColumn, otherItemRow].GetComponent<Item>().row = itemRow;
        allItems[itemColumn, itemRow].GetComponent<Item>()._isSwapping = true;
        allItems[otherItemColumn, otherItemRow].GetComponent<Item>()._isSwapping = true;
        swapPositions.Clear();
        //Debug.Log("swapping back");
    }

    public IEnumerator MoveItemDown(int [,] boardOfIds, int [,] newBoardOfIds)
    {
        yield return new WaitForSeconds(0.3f);
        
        for (int i = 0; i < width; i++)
        {
            int steps = 0;
            for (int j = 0; j < height; j++)
            {
                if (boardOfIds[i, j] == removedItemId)
                {
                    steps += 1;
                }
                if (steps != 0 && j < height - 1 && boardOfIds[i, j + 1] != removedItemId)
                {
                    int k = j + 1;
                    
                    while (k < height && boardOfIds[i, k] != removedItemId)
                    {                                                                                                                                                                                            
                        if (boardOfIds[i, k] != removedItemId && boardOfIds[i, k] != addNewItemId && allItems[i, k] != null)
                        {
                            
                            allItems[i, k].GetComponent<Item>().row -= steps;
                            allItems[i, k].GetComponent<Item>()._isSwapping = true;
                        }
                        k += 1;
                    }
                    
                    if (k < height && boardOfIds[i, k] == removedItemId)
                    {
                        j = k - 1;
                        continue;
                    }

                    j = k - 1;
                }
            }
            
            /*if (steps != 0)
            {
                
                for (int j = 0; j < height; j++)
                {
                    if (boardOfIds[i, j] == removedItemId)
                    {
                        while (boardOfIds[i, j] == removedItemId && height - steps > j)
                        {
                            boardOfIds[i, j] = boardOfIds[i, j + steps];
                            j += 1;
                        }
                        for (int k = j; k < height; k++)
                        {
                            if (height - steps <= k)
                            {
                                boardOfIds[i, k] = addNewItemId;
                            }
                            else
                            {
                                boardOfIds[i, k] = boardOfIds[i, k + steps];
                            }
                        }
                    }
                }
                
            }*/
        }

        for (int i = 0; i < width; i++)
        {
            int steps = 0;
            for (int j = 0; j < height; j++)
            {
                if (boardOfIds[i, j] == removedItemId)
                {
                    steps += 1;
                }

                if (steps != 0 && j < height - 1 && boardOfIds[i, j + 1] != removedItemId)
                {
                    int k = j + 1;
                    while (k < height && boardOfIds[i, k] != removedItemId)
                    {
                        boardOfIds[i, k - steps] = boardOfIds[i, k];
                        k += 1;
                    }

                    if (k < height && boardOfIds[i, k] == removedItemId)
                    {
                        j = k - 1;
                        continue;
                    }
                    
                    int l = k - steps;
                    for (; l < height; l++)
                    {
                        boardOfIds[i, l] = addNewItemId;
                    }
                    break;
                }

                if (steps != 0 && j == height - 1 && boardOfIds[i, j] == removedItemId)
                {
                    for (int k = height - steps; k < height; k++)
                    {
                        boardOfIds[i, k] = addNewItemId;
                        
                    }
                }
            }
        //Debug.Log("steps in board: " + steps);
        }
        //gameState.state = global::GameState.State.CheckMatchAgain;
        string board = "";
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                board += boardOfIds[j, i] + " ";
            }
            board += "\n";
        }
        Debug.Log(board);
        
        string board1 = "";
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                board1 += newBoardOfIds[j, i] + " ";
            }
            board1 += "\n";
        }
        Debug.Log(board1);

        StartCoroutine(GenerateNewItem(boardOfIds, newBoardOfIds));
    }

    public IEnumerator GenerateNewItem(int [,] boardOfIds, int [,] newBoardOfIds)
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (boardOfIds[i, j] == addNewItemId)
                {

                    if ((j != 0 && boardOfIds[i, j - 1] != addNewItemId ) || (j == 0 && boardOfIds[i, j] == addNewItemId)) // index out of range
                    {
                        Vector2 generatePosition = new Vector2(i, generateItemRow);
                        //int idOfItem = Random.Range(0, itemLength);
                        try
                        {
                            
                            
                            GameObject item = Instantiate(items[newBoardOfIds[i, j]], generatePosition, Quaternion.identity);
                            item.transform.parent = this.transform;
                            allItems[i, j] = item;
                            
                            
                        }
                        catch (System.IndexOutOfRangeException e)
                        {
                            Debug.Log("column is: " + i);
                            Debug.Log("row is: " + j);
                            Debug.Log("value is: " + newBoardOfIds[i, j]);
                            System.Console.WriteLine(e.Message);
                            // Set IndexOutOfRangeException to the new exception's InnerException.
                            throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
                        }
                        
                        StartCoroutine(MoveNewItemDown(i, j));
                    }
                    else
                    {
                        yield return new WaitUntil(() => allItems[i, j - 1].GetComponent<Item>()._allowOtherItemMove == true);
                        Vector2 generatePosition = new Vector2(i, generateItemRow);
                        GameObject item = Instantiate(items[newBoardOfIds[i, j]], generatePosition, Quaternion.identity);
                        item.transform.parent = this.transform;
                        allItems[i, j] = item;
                    
                        StartCoroutine(MoveNewItemDown(i, j));
                    }
                } 
                else if (boardOfIds[i, j] >= appleBomb && boardOfIds[i, j] < appleBomb + itemLength)
                {
                    /*
                    if (j == height - 1)
                    {
                        Vector2 bombPosition = new Vector2(i, generateItemRow);
                        GameObject bomb = Instantiate(bombs[newBoardOfIds[i, j] - itemLength], bombPosition, Quaternion.identity);
                        bomb.transform.parent = this.transform;
                        allItems[i, j] = bomb;
                        StartCoroutine(MoveNewItemDown(i, j));
                    }
                    else*/
                    if (allItems[i, j] != null && allItems[i, j].transform.name == "Bomb")
                    {
                        Debug.Log(allItems[i, j].transform.name + ": " + i + ", " + j + ": " + boardOfIds[i, j]);
                        Destroy(allItems[i, j]);
                    }
                    Vector2 bombPosition = new Vector2(i, j);
                    GameObject bomb = Instantiate(bombs[newBoardOfIds[i, j] - itemLength], bombPosition, Quaternion.identity);
                    bomb.transform.name = "Bomb";
                    bomb.transform.parent = this.transform;
                    allItems[i, j] = bomb;
                    //Debug.Log("Generating bomb");
                } 
                else if (boardOfIds[i, j] == bomb3x3Id)
                {
                    if (allItems[i, j] != null && allItems[i, j].transform.name == "Bomb")
                    {
                        Destroy(allItems[i, j]);
                    }
                    Vector2 bombPosition = new Vector2(i, j);
                    GameObject bomb = Instantiate(bomb3x3, bombPosition, Quaternion.identity);
                    bomb.transform.name = "Bomb";
                    bomb.transform.parent = this.transform;
                    allItems[i, j] = bomb;
                    //Debug.Log("Generating bomb");
                }
            }
        }
        yield return new WaitForSeconds(1f);
        gameState.state = global::GameState.State.CheckMatchAgain;
    }

    public IEnumerator MoveNewItemDown(int column, int row)
    {
        yield return new WaitForSeconds(0.1f);
        allItems[column, row].GetComponent<Item>().row = row;
        allItems[column, row].GetComponent<Item>()._isSwapping = true;
    }
}
