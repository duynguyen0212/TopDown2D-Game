using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RoomTransfer1 : MonoBehaviour
{
    public Vector2 maxPos;
    public Vector2 minPos;

    private Vector2 tempMaxPos;
    private Vector2 tempMinPos;
    //move player
    public Vector3 playerChange;
    public bool flipScence=false;
    private CameraMovement cam;
    public bool needText;
    private string  placeName;
    public GameObject text;
    public TextMeshProUGUI placeText;
    public string roomName1;
    public string roomName2;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        tempMaxPos = cam.maxPos;
        tempMinPos = cam.minPos;
       
        if(other.CompareTag("Player") && flipScence==false){
            cam.minPos += minPos;
            cam.maxPos += maxPos;
            other.transform.position += playerChange;
            flipScence = true;
            
            placeNameTransition();
        }    
        else if(other.CompareTag("Player") && flipScence==true){
            cam.minPos -= minPos;
            cam.maxPos -= maxPos;
            other.transform.position -= playerChange;
            flipScence = false;

            placeNameTransition();
        }    
    }

    private IEnumerator placeNameCo(){
        text.SetActive(true);
        if(flipScence==false){
            placeName = roomName1;
        }
        else{
            placeName = roomName2;
        }
            
        placeText.text = placeName;
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
        placeText.text = null;

    }

     private IEnumerator FadeOutCoroutine()
    {
        float fadeDuration = 3.0f; //for fadeout transition TMP
        canvasGroup = text.gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = text.gameObject.AddComponent<CanvasGroup>();
        }
       
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha1 = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha1;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0.0f;
        text.gameObject.SetActive(false);
        
    }

    private void placeNameTransition(){
        if(needText){
            StopAllCoroutines();
        StartCoroutine(placeNameCo());
        StartCoroutine(FadeOutCoroutine());
        }
        else{
            return;
        }
        
    }
}
