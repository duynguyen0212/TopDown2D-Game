using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bear : Enemy
{
    private float moveSpeed = 2f;             // Speed of the enemy movement
    private float maxRange = 5f;        // Distance at which the enemy detects the player
    private float minRange;
    public float attackRange = 1f;           // Distance at which the enemy attacks the player
    private bool shouldRotate = true;
    public LayerMask whatIsPlayer;
    
    private Transform player;                // Reference to the player's transform
    private Animator animator;               // Reference to the animator component
    private float moveX;                     // Horizontal movement input
    private float moveY;                     // Vertical movement input
    private Vector2 movement;
    private Vector3 direction;
    private bool isInFollowRange = false;
    private bool isInAttackRange;
    private bool facingRight = true;
    private Rigidbody2D rb;
    private float knockbackForce = 3f;
    private float cooldown = 1.5f;
    private float criticalHitChance = 0.15f;
    private float nextAttackTime;
    private bool isCoolingDown => Time.time < nextAttackTime;
    private bool attackAnimation = false;
    
    private void StartCoolDown(float cooldownTime) => nextAttackTime = Time.time + cooldownTime;
   
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform; // Assumes player has "Player" tag
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        base.maxHealth = 50;
        base.currentHealth = base.maxHealth;
        
    }
    

    private void Update()
    {

        isInFollowRange = Physics2D.OverlapCircle(transform.position, maxRange, whatIsPlayer);
        isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);
        

        direction = player.position-transform.position;
        float angle = Mathf.Atan2(direction.x,direction.y)*Mathf.Rad2Deg;
        direction.Normalize();
        movement = direction;
        if(shouldRotate){
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }

    
        // method to flip animation from left to right and vice versa
        if(movement.x < 0 && facingRight ||movement.x > 0 && !facingRight){
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *=-1f;
            transform.localScale = localScale;
        }

       

        
    }

    private void FixedUpdate() {
        if(isInFollowRange && !isInAttackRange){
            MoveCharacter(movement);
        }   
         if(isInAttackRange){
            if(isCoolingDown) return;
            StartCoroutine(AttackCo());
            StartCoolDown(cooldown);
            
        
        } else if(!isInAttackRange && attackAnimation == false){
            animator.SetBool("isAttacking",false);
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        } 
        
        if (!isInFollowRange){
            animator.SetBool("isWalking",false);
        }
         
    }

    private void Wander()
    {
        
            moveX = Mathf.Sign(Random.Range(-1f, 1f));
            moveY = Mathf.Sign(Random.Range(-1f, 1f));

        transform.Translate(new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime);
        
    }

    private void MoveCharacter(Vector2 dir)
    {
        animator.SetBool("isWalking", isInFollowRange);
        rb.MovePosition((Vector2)transform.position + (dir*moveSpeed*Time.deltaTime));
    }

    
    private void ApplyKnockback(PlayerMovement player){
        Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
        player.ApplyKnockback(knockbackDirection, knockbackForce);
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (other.gameObject.CompareTag("Player"))
            {
                float randomValue = Random.Range(0f,1f);
                if(randomValue<= criticalHitChance){
                    ApplyKnockback(player);
                    
                    base.criticalHitBonus = base.baseAttackDmg*80/100;
                }else{
                    base.criticalHitBonus = 0;
                }
                base.DealDmg(player);
                
            }
        
    }

    

    private IEnumerator AttackCo(){
        attackAnimation = true;
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking",true);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(.5f);
        animator.SetBool("isAttacking",false);
        attackAnimation = false;
        
    }

}
