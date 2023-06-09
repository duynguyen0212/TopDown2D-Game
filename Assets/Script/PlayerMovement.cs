using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState{
    idle,
    walk,
    attack,
    interact,
    knockedback
}
public class PlayerMovement : MonoBehaviour
{
    private float speed = 7;
    public int maxHealth = 100;
    public int maxMana = 100;
    public int maxEXP = 1000;
    public int currentHealth,currentMana,currentEXP;
    private int baseAttackDmg = 10;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Vector2 movement;
    private Animator animator;
    public bool facingRight = true;
    public Vector3 refVel = Vector3.zero;
    public PlayerState currentState;
    Vector2 knockbackVec;
    public float criticalHitChance;
    private int criticalHitBonus;
    public Player_Info info;
    public bool isRegen = false;
    private float regenCooldown = 5f;
    [SerializeField]
    private GameObject healingParticle;

   
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEXP = 0;
        info.SetMaxHP(maxHealth);
        info.SetMaxMana(maxMana);
        info.SetMaxEXP(maxEXP);
        info.SetEXP(currentEXP);
        healingParticle.SetActive(false);
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        Enemy enemy = GetComponent<Enemy>();
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
        }
        
        

        if(change.x < 0 && facingRight ||change.x > 0 && !facingRight){
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *=-1f;
            transform.localScale = localScale;
        }
        
    }

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
        if(currentState == PlayerState.knockedback){
            myRigidbody.AddForce(knockbackVec, ForceMode2D.Impulse);
            StartCoroutine(KnockbackCo());
        }

        if(currentState == PlayerState.idle){
            if(regenCooldown>0 && currentHealth < maxHealth){
                regenCooldown -= Time.deltaTime;
            }
            else if(isRegen == false && regenCooldown < 0){
                StartCoroutine(HealingCo());
            }else if(currentHealth == maxHealth){
                isRegen=false;
                StopCoroutine(HealingCo());
            }
            else {
                regenCooldown = 5f;
                return;
            }
        }
        if(currentState != PlayerState.idle){
            isRegen = false;
            StopCoroutine(HealingCo());
        }

        if(change !=Vector3.zero  && currentState != PlayerState.attack){
            currentState = PlayerState.walk;
            //UpdateAnimationAndMove();
            animator.SetBool("moving",true);
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetFloat("speed", change.sqrMagnitude);
        }
        else if(change ==Vector3.zero  && currentState != PlayerState.attack){
            animator.SetFloat("speed",0);
            animator.SetBool("moving",false);  
            currentState = PlayerState.idle;
        }
        
        
    }


    public void ApplyKnockback(Vector2 direction, float force)
    {
        knockbackVec = direction * force;
        currentState = PlayerState.knockedback;
    }

    private IEnumerator KnockbackCo(){
        yield return new WaitForSeconds(0.15f);
        myRigidbody.velocity = Vector2.zero;
        currentState = PlayerState.walk;
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;
        info.SetHP(currentHealth);
    }

    private IEnumerator HealingCo(){
        
        isRegen=true;
        healingParticle.SetActive(true);
        while(currentHealth<maxHealth && isRegen){
            currentHealth ++;
            info.SetHP(currentHealth);
            yield return new WaitForSeconds(1f);
        }
        healingParticle.SetActive(false);
        isRegen=false;

    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (other.CompareTag("Enemy") && currentState == PlayerState.attack )
        {
            float randomValue = Random.Range(0f,1f);
            if(randomValue<= criticalHitChance){
                criticalHitBonus = baseAttackDmg*80/100;
            }else{
                criticalHitBonus = 0;
            }
            
            int damage = baseAttackDmg + criticalHitBonus;
            enemy.TakeDamage(damage);
            
        }

         if(other.CompareTag("Breakable")){
            other.GetComponent<Pots>().Smash();
        }
    }
}
