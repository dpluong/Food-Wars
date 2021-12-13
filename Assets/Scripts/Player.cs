using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Move()) return;

    }

    private bool Move() 
    {
        RaycastHit hit;
        bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
        if (hasHit)
        {
            if (Input.GetMouseButton(0)) 
            {
                //GetComponent<Mover>().StartMoveAction(hit.point);
                navMeshAgent.destination = hit.point;

            }
            return true;
        }
        return false;
    }

    private static Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
