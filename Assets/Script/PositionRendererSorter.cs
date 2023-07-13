using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    private Renderer myRen;
    private int sortingOrderBase=3;
    private int offset = -1;
    private void Awake(){
        myRen = gameObject.GetComponent<Renderer>();
    }
    private void LateUpdate() {
        myRen.sortingOrder = (int)(sortingOrderBase-transform.position.y-offset);
    }
}
