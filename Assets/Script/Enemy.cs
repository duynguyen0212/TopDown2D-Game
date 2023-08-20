using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth, currentHealth;
    public string enemyName;
    public int baseAttackDmg;
    public int criticalHitBonus;
    public SpriteRenderer spriteRenderer;
    private float fadeDuration = 4f;
    public Color targetColor;

    public Transform originalPos;
    public delegate void EnemyKilled();   
    public static event EnemyKilled OnEnemyKilled; 
 
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0){
            Destroy(gameObject);
            if(OnEnemyKilled!=null)
                OnEnemyKilled();
            
        }
    }

    public void DealDmg(PlayerMovement player)
    {
        // Calculate damage
        int damage = baseAttackDmg + criticalHitBonus;

        // Apply damage to the player
        player.TakeDamage(damage);
    }

    // Not working yet
    public IEnumerator DestroyCo()
    {
        //Color originalColor = spriteRenderer.color;
        

        float timer = 0f;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            //spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);
            spriteRenderer.color = new Color(1f,1f,1f, Mathf.SmoothStep(0f,2f,t));
           
            timer += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = targetColor; // Ensure the target color is set precisely
        yield return new WaitForSeconds(0.5f);
        // Once faded out, perform additional actions (e.g., destroy the enemy object)
        //Destroy(gameObject);
    }

}

