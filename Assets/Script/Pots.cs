using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pots : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Smash(){
        anim.SetBool("break",true);
        StartCoroutine(breakPotCo());
    }
    private IEnumerator breakPotCo(){
        yield return new WaitForSeconds(.3f);
        this.gameObject.SetActive(false);       

    }
}
