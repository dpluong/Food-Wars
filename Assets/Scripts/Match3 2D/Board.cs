using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    //public GameObject tilePrefab;
    //public GameObject[] items;
    //public GameObject[,] allItems;
    
    public int[,] boardOfIDs;

    const int itemLength = 3;
    const int removedItemId = -100;
    const int addNewItemId = -200;
    const int appleBomb = 3;
    const int bomb3x3Id = 14;
    //private BackgroundTile[,] allTiles;

    // Flags
    bool _isMatch = false;
    bool _isSetUp = false;
    bool _trigger3x3Bomb = false;


    private List<int> columnMatch = new List<int>();
    private List<int> itemMatch = new List<int>();
    private List<int> bombColumn = new List<int>();
    private List<int> bombRow = new List<int>();
    private List<int> bombValue = new List<int>();


    // Start is called before the first frame update


    // Update is called once per frame
    public void SetUp()
    {
        _isSetUp = true;
        boardOfIDs = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {                                        
                int idOfItem = Random.Range(0, itemLength);
                boardOfIDs[i, j] = idOfItem;
            }
        }

        while (FindMatch())
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (boardOfIDs[i, j] == removedItemId)
                    {
                        int idOfItem = Random.Range(0, itemLength);
                        boardOfIDs[i, j] = idOfItem;
                    }
                }
            }
            _isMatch = false;
        }
        _isSetUp = false;
    }

    private int FindRowMatch(int collumn, int row)
    {
        int index = collumn + 1;
        int valueIfMatched = boardOfIDs[collumn, row];
        while (index < width && (boardOfIDs[collumn, row] == boardOfIDs[index, row] || Mathf.Abs(boardOfIDs[collumn, row] - boardOfIDs[index, row]) == itemLength) && boardOfIDs[collumn, row] != addNewItemId)
        {
            if ((Mathf.Abs(boardOfIDs[collumn, row] - boardOfIDs[index, row]) == itemLength || Mathf.Abs(boardOfIDs[collumn, row] - boardOfIDs[index - 1, row]) == itemLength) && index - collumn >= 2)
            {
                for (int i = 0; i < width; ++i) 
                {
                    boardOfIDs[i, row] = removedItemId;
                }
                _isMatch = true;
                return width;
            }
            index += 1;    
        }
        if (index - collumn >= 3)
        {
            //Debug.Log("row match in " + row + " th row");
            _isMatch = true;
            for (int i = collumn; i < index; ++i) 
            {
                boardOfIDs[i, row] = removedItemId;
            }
                //ClearRowMatch(collumn, row);
        } 
        if (index - collumn == 2 && _isSetUp == false)
        {
            for (int i = collumn - 1; i <= index; ++i)
            {
                if (columnMatch.Contains(i) && itemMatch.Contains(boardOfIDs[collumn, row]) && ((collumn > 0 && boardOfIDs[collumn - 1, row] == removedItemId) || (index < width && boardOfIDs[index, row] == removedItemId)))
                {
                    if (columnMatch.IndexOf(i) == itemMatch.IndexOf(boardOfIDs[collumn, row]))
                    {
                        _isMatch = true;
                        for (int j = collumn; j < index; ++j)
                        {
                            boardOfIDs[j, row] = removedItemId;
                        }
                        Generating3x3Bomb(i, row);
                        return index;
                    }
                }
            }   
        }
        
        if (index - collumn >= 4 && _isSetUp == false && valueIfMatched < itemLength && valueIfMatched >= 0)
        {
            //Debug.Log("Generating bomb in row: " + row);
            //GeneratingBomb(collumn, row, valueIfMatched);
            bombColumn.Add(collumn);
            bombRow.Add(row);
            bombValue.Add(valueIfMatched + itemLength);
        }
        return index;
    }

    private int FindCollumnMatch(int collumn, int row)
    {
        if (boardOfIDs[collumn, row] == bomb3x3Id && _trigger3x3Bomb)
        {
            _trigger3x3Bomb = false;
            _isMatch = true;
            if (collumn > 0 )
            {
                boardOfIDs[collumn - 1, row] = removedItemId;
            } 
            if (collumn < width - 1)
            {
                boardOfIDs[collumn + 1, row] = removedItemId;
            }
            if (row > 0)
            {
                boardOfIDs[collumn, row - 1] = removedItemId;
            }
            if (row < height - 1)
            {
                boardOfIDs[collumn, row + 1] = removedItemId;
            }
            if (collumn > 0 && row > 0)
            {
                 boardOfIDs[collumn - 1, row - 1] = removedItemId;
            }
            if (collumn < width - 1 && row < height - 1)
            {
                 boardOfIDs[collumn + 1, row + 1] = removedItemId;
            }
            if (collumn > 0 && row < height - 1)
            {
                boardOfIDs[collumn - 1, row + 1] = removedItemId;
            }
            if (collumn < width - 1 && row > 0)
            {
                boardOfIDs[collumn + 1, row - 1] = removedItemId;
            }
            boardOfIDs[collumn, row] = removedItemId;
        }
        int index = row + 1;
        int valueIfMatched = boardOfIDs[collumn, row];
        while (index < height && (boardOfIDs[collumn, row] == boardOfIDs[collumn, index] || Mathf.Abs(boardOfIDs[collumn, row] - boardOfIDs[collumn, index]) == itemLength) && boardOfIDs[collumn, row] != addNewItemId)
        {
            if ((Mathf.Abs(boardOfIDs[collumn, row] - boardOfIDs[collumn, index]) == itemLength || Mathf.Abs(boardOfIDs[collumn, row] - boardOfIDs[collumn, index - 1]) == itemLength) && index - row >= 2)
            {
                for (int i = 0; i < height; ++i) 
                {
                    boardOfIDs[collumn, i] = removedItemId;
                }
                _isMatch = true;
                return height;
            }
            
            index += 1;
            
        }
        if (index - row >= 3)
        {
            
            _isMatch = true;
            if (_isSetUp == false)
            {
                columnMatch.Add(collumn);
                itemMatch.Add(boardOfIDs[collumn, row]);
            }
            for (int i = row; i < index; ++i) 
            {
                boardOfIDs[collumn, i] = removedItemId;
            }
        } 
        
        if (index - row >= 4 && _isSetUp == false && valueIfMatched < itemLength && valueIfMatched >= 0)
        {
            //Debug.Log("Generating bomb in column: " + collumn);
            //GeneratingBomb(collumn, row, valueIfMatched);
            bombColumn.Add(collumn);
            bombRow.Add(row);
            bombValue.Add(valueIfMatched + itemLength);
        }

        return index;
    }

    
    public bool FindMatch()
    {
        for (int i = 0; i < width; i++)
        {
            int indexToFindMatch = 0;
            while (indexToFindMatch < height)
            { 
                indexToFindMatch = FindCollumnMatch(i, indexToFindMatch);
            }
        }

        for (int j = 0; j < height; j++)
        {
            int indexToFindMatch = 0;
            while (indexToFindMatch < width)
            {
                indexToFindMatch = FindRowMatch(indexToFindMatch, j);
            }
        }
        if (!_isSetUp)
        {
            GeneratingBomb();
        }
        // FindBombMatch();
        columnMatch.Clear();
        itemMatch.Clear();

        if (_isMatch)
        {
            _isMatch = false;
            return true;
        }
        return false;
    }

    public void UpdateBoardAfterSwapping(List<(int column, int row)> swapPositions)
    {
        int tempId = boardOfIDs[swapPositions[0].column, swapPositions[0].row];
        if (tempId == bomb3x3Id || boardOfIDs[swapPositions[1].column, swapPositions[1].row] == bomb3x3Id)
        {
            _trigger3x3Bomb = true;
        } 
            
        boardOfIDs[swapPositions[0].column, swapPositions[0].row] = boardOfIDs[swapPositions[1].column, swapPositions[1].row];
        boardOfIDs[swapPositions[1].column, swapPositions[1].row] = tempId;
    }
    
    public void UpdateBoardAfterMovingItemDown()
    {
        for (int i = 0; i < width; i++)
        {
            int steps = 0;
            for (int j = 0; j < height; j++)
            {
                if (boardOfIDs[i, j] == removedItemId)
                {
                    steps += 1;
                }

                if (steps != 0 && j < height - 1 && boardOfIDs[i, j + 1] != removedItemId )
                {
                    int k = j + 1;
                    while (k < height && boardOfIDs[i, k] != removedItemId)
                    {
                        boardOfIDs[i, k - steps] = boardOfIDs[i, k];
                        k += 1;
                    }

                    if (k < height && boardOfIDs[i, k] == removedItemId)
                    {
                        j = k - 1;
                        continue;
                    }
                    
                    int l = k - steps;
                    for (; l < height; l++)
                    {
                        //boardOfIDs[i, l] = addNewItemId;
                        
                        int idOfItem = Random.Range(0, itemLength);
                        boardOfIDs[i, l] = idOfItem;
                    }
                    break;
                }

                if (steps != 0 && j == height - 1 && boardOfIDs[i, j] == removedItemId)
                {
                    for (int k = height - steps; k < height; k++)
                    {
                        //boardOfIDs[i, k] = addNewItemId;
                        int idOfItem = Random.Range(0, itemLength);
                        boardOfIDs[i, k] = idOfItem;
                    }
                }
            }
        //Debug.Log("steps in board: " + steps);
        }
        
        /*string board = "";
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                board += boardOfIDs[j, i] + " ";
            }
            board += "\n";
        }
        Debug.Log(board);*/
    }

    public void GeneratingBomb()
    {
        for (int i = 0; i < bombValue.Count; ++i) 
        {
            boardOfIDs[bombColumn[i], bombRow[i]] = bombValue[i];
        }
        bombColumn.Clear();
        bombRow.Clear();
        bombValue.Clear();
    }

    public void Generating3x3Bomb(int column, int row)
    {
        boardOfIDs[column, row] = bomb3x3Id;
    }

    /*
        private void ClearRowMatch(int column, int row)
        {
            Destroy(allItems[column, row]);
            Destroy(allTiles[column, row]);
            Destroy(allItems[column + 1, row]);
            Destroy(allTiles[column + 1, row]);
            Destroy(allItems[column + 2, row]);
            Destroy(allTiles[column + 2, row]);
        }

        private void ClearColumnMatch(int column, int row)
        {
            Destroy(allItems[column, row]);
            Destroy(allTiles[column, row]);
            Destroy(allItems[column, row + 1]);
            Destroy(allTiles[column, row + 1]);
            Destroy(allItems[column, row + 2]);
            Destroy(allTiles[column, row + 2]);
        }

        IEnumerator MoveItemDown()
        {
            yield return new WaitForSeconds(0.25f);

        }

        private void CalculateNewPositions()
        {

        }
        */
}
