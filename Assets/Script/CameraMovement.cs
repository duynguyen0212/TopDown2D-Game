
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //player
    public Transform target;
    public Vector2 maxPos;
    public Vector2 minPos;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate() {
        Vector3 desiredPosittion = target.position + offset; 
        

        desiredPosittion.x = Mathf.Clamp(desiredPosittion.x,
                                        minPos.x,
                                        maxPos.x);
        desiredPosittion.y = Mathf.Clamp(desiredPosittion.y,
                                        minPos.y,
                                        maxPos.y);

        transform.position = desiredPosittion;  
    }
    
}
