using UnityEngine;
using UnityEngine.EventSystems;
using Bam.Extensions;
using System;
using Bam.Singleton;

public class ClickChecker : Singleton<ClickChecker>
{
    public event Action<Operator> onOperatorClicked;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!Extensions.IsPointerOverObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var clickedTile = hit.collider.GetComponent<Tile>();
                if(clickedTile != null)
                {
                    onOperatorClicked?.Invoke(clickedTile.UnitOnTile);
                }
            }
            else onOperatorClicked?.Invoke(null);
        }
    }
}
