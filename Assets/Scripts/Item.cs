using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    View board;
    GameObject otherItem;
    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;
    public float swipeAngle = 0;
    public int id;



    public bool _isSwapping = false;
    public bool _swappingDone = false;
    public bool _allowOtherItemMove = false;

   
    
    private static int CameraNum = 2;
    private Camera[] cameras = new Camera[CameraNum];
    private Camera Camera2D;

    // Start is called before the first frame update
    void Start()
    {
        CameraNum = Camera.GetAllCameras(cameras);
        Camera2D = cameras[0];
        
        board = FindObjectOfType<View>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        SwapItemMotion();
    }

    private void SwapItemMotion()
    {
        if (_isSwapping)
        {
            targetX = column;
            targetY = row;
            if (Mathf.Abs(transform.position.y - 9) < .5)
            {
                _allowOtherItemMove = true;
            } 
            if (Mathf.Abs(targetX - transform.position.x) > .1)
            {
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, .1f);
            }
            else
            {
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = tempPosition;
                board.allItems[column, row] = this.gameObject;
            }
            if (Mathf.Abs(targetY - transform.position.y) > .1)
            {
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = Vector2.Lerp(transform.position, tempPosition, .1f);
            }
            else
            {
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = tempPosition;
                board.allItems[column, row] = this.gameObject;
            }
            if (transform.position.x == targetX && transform.position.y == targetY)
            {
                _allowOtherItemMove = false;
                _isSwapping = false;
                //Debug.Log(board.allItems[column, row].name);
                //Debug.Log(column + ", " + row);
                _swappingDone = true;
                if (board._swappedBack == true)
                {
                    _swappingDone = false;
                    board.swapPositions.Clear();
                    board.gameState.state = global::GameState.State.Idle;
                    board._swappedBack = false;
                }
                
            }
        }
    }

    private void OnMouseDown() 
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp() 
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        if (board.swapPositions.Count == 0 && (finalTouchPosition.y != firstTouchPosition.y || finalTouchPosition.x != firstTouchPosition.x))
        {
            MoveItems();
        }
    }

    void MoveItems()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
        {
            if (board.swapPositions.Count == 0)
            {
                board.swapPositions.Add((column, row));
                board.swapPositions.Add((column + 1, row));
            }
            otherItem = board.allItems[column + 1, row];
            otherItem.GetComponent<Item>().column -= 1;
            column += 1;
            
        } 
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            if (board.swapPositions.Count == 0)
            {
                board.swapPositions.Add((column, row));
                board.swapPositions.Add((column, row + 1));
            }
            otherItem = board.allItems[column, row + 1];
            otherItem.GetComponent<Item>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            if (board.swapPositions.Count == 0)
            {
                board.swapPositions.Add((column, row));
                board.swapPositions.Add((column - 1, row));
            }
            otherItem = board.allItems[column - 1, row];
            otherItem.GetComponent<Item>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            if (board.swapPositions.Count == 0)
            {
                board.swapPositions.Add((column, row));
                board.swapPositions.Add((column, row - 1));
            }
            otherItem = board.allItems[column, row - 1];
            otherItem.GetComponent<Item>().row += 1;
            row -= 1;
        }
        
        _isSwapping = true;
        otherItem.GetComponent<Item>()._isSwapping = true;
    }
}
