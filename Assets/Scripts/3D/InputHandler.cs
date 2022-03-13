using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public UnityEvent<Vector3> MouseClick;
   
    private void Update() 
    {
        GetPlayerInput();
    }

    public void GetPlayerInput()
    {
        // select position in map
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosDown = Input.mousePosition;
            MouseClick?.Invoke(mousePosDown);
        }

        
    }

}
