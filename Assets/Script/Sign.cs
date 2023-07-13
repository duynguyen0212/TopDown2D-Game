using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Sign : MonoBehaviour
{
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public string dialog;
    public bool dialogActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && Input.GetButtonDown("Attack")){
          
            StartCoroutine(dialogTrigger());
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            
            dialogBox.SetActive(false);
        }
    }

    private IEnumerator dialogTrigger(){
       
        dialogBox.SetActive(true);
        dialogText.text = dialog;
        yield return new WaitForSeconds(4f);

    }
}
