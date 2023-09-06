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
    private int maxHealth = 100;
    private int maxMana = 100;
    private int maxEXP = 1000;
    public int currentHealth,currentMana,currentEXP;
    private int baseAttackDmg = 10;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Vector2 movement;
    private Animator animator;
    private bool facingRight = true;
    private Vector3 refVel = Vector3.zero;
    public PlayerState currentState;
    Vector2 knockbackVec;
    public float criticalHitChance;
    private int criticalHitBonus;
    public Player_Info info;
    private bool isRegen = false;
    private float regenCooldown = 5f;
    [SerializeField] private GameObject healingParticle;
    [SerializeField] private GameObject impactParticle;
    public float comboResetCooldown;
    public int noOfClicks = 0;
    float lastClickedTime = 0f;    
   
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
        currentState = PlayerState.idle;
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
        
        //impactParticle.transform.position = new(animator.GetFloat("moveX"),animator.GetFloat("moveY"),0);
        
        if(Time.time - lastClickedTime > comboResetCooldown){
            noOfClicks = 0;
        }

        // Attack function check how many attack button was click
        // perform combo depend on the number of clicks
        if(Input.GetButtonDown("Attack") && currentState != PlayerState.attack){
            lastClickedTime = Time.time;
            noOfClicks++;
            noOfClicks = Mathf.Clamp(noOfClicks,0,3);
            OnClick(noOfClicks);
            
        }
        
        // Check if animation combo attack finish
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack0")){
            animator.SetBool("combo0", false);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")){
            animator.SetBool("combo1", false);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")){
            animator.SetBool("combo2", false);
            noOfClicks = 0;
        }

        // check if the player is facing other direction, flip the sprite to opposite side
        if(change.x < 0 && facingRight ||change.x > 0 && !facingRight){
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *=-1f;
            transform.localScale = localScale;
        }
        
    }
   
    // Onclick method to turn on combo animation
    private void OnClick(int comboNum){
        if(comboNum == 1 && currentState!=PlayerState.attack){
            animator.SetBool("combo0", true);
            StartCoroutine(AttackCo());
            
        }
        else if(comboNum == 2 && currentState!=PlayerState.attack){
            animator.SetBool("combo0", false);  
            animator.SetBool("combo1", true);
            StartCoroutine(AttackCo());
        }

        else if(comboNum == 3 && currentState!=PlayerState.attack){
            animator.SetBool("combo1", false);
            animator.SetBool("combo2", true);
            StartCoroutine(AttackCo());
            noOfClicks = 0;
        }
    }

    void FinishCombo(){
        noOfClicks = 0;
    }

    private IEnumerator AttackCo(){
        currentState = PlayerState.attack; //meaning not in walking state/ can't move
        yield return new WaitForSeconds(0.3f);
        currentState = PlayerState.walk;
        if(noOfClicks>2){
            noOfClicks = 0;     
        }
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
        //TODO: Hurt animation
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
            // StartCoroutine(ImpactEffect());
            
        }

         if(other.CompareTag("Breakable")){
            other.GetComponent<Pots>().Smash();
        }
    }
    private IEnumerator ImpactEffect(){
        impactParticle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        impactParticle.SetActive(false);
    }
}
