using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PointerController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    //private List<Vector3Int> neighbours = new List<Vector3Int>();

    public LayerMask hexTileMask;

    //public HexGrid hexGrid;
    public UnityEvent<GameObject> OnUnitSelected;
    public UnityEvent<GameObject> TerrainSelected;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void HandlePointer(Vector3 mousePos) 
    {
        GameObject result;
        if (FindPointerTarget(mousePos, out result))
        {
            if (UnitSelected(result))
            {
                OnUnitSelected?.Invoke(result);
            }
            else
            {
                TerrainSelected?.Invoke(result);
            }
        }
    }

    private bool UnitSelected(GameObject result)
    {
        return result.GetComponent<Unit>() != null;
    }

    private bool FindPointerTarget(Vector3 mousePos, out GameObject target)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexTileMask))
        {
            target = hit.collider.gameObject;
            return true;
        }
        target = null;
        return false;
    }
}
