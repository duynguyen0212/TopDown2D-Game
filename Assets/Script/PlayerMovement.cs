using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState{
    walk,
    attack,
    interact
}
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Vector2 movement;
    private Animator animator;
    public bool facingRight = true;
    public Vector3 refVel = Vector3.zero;
    public PlayerState currentState;
   
    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX",0);
        animator.SetFloat("moveY", -1);
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
       
        if(Input.GetButtonDown("Attack") && currentState != PlayerState.attack){
            StartCoroutine(AttackCo());
            // Attack();   
            
                   
        }

        else if(currentState == PlayerState.walk){
            UpdateAnimationAndMove();
        }


        if(change.x < 0 && facingRight ||change.x > 0 && !facingRight){
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *=-1f;
            transform.localScale = localScale;
        }
        
    }

    // void Attack(){
    //     animator.SetTrigger("attack");
    // }
    private IEnumerator AttackCo(){
        animator.SetBool("attacking",true);
        currentState = PlayerState.attack; //meaning not in walking state/ can't move
        yield return null;
        animator.SetBool("attacking",false);
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.walk; 
        
    }

    private void FixedUpdate() {
        if(currentState == PlayerState.walk){
            change.Normalize();
            myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
            
        }
    }
    void UpdateAnimationAndMove(){
        if(change !=Vector3.zero){
            //MoveCharacter();
            animator.SetBool("moving",true);
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetFloat("speed", change.sqrMagnitude);
        }
        else{
            animator.SetFloat("speed",0);
            animator.SetBool("moving",false);
        }
    }
}
