using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Info : MonoBehaviour
{

    public Slider sliderHP, sliderEXP, sliderMana;
    public int currentHP, currentMana, currentEXP;
    private float velocity;
    // Start is called before the first frame update
    
    public void SetMaxHP(int hp){
        sliderHP.maxValue = hp;
        sliderHP.value = hp;
        currentHP = hp;
    }

    public void SetMaxMana(int mana){
        sliderMana.maxValue = mana;
        sliderMana.value = mana;
    }
    public void SetMaxEXP(int exp){
        sliderEXP.maxValue = exp;
    }
    public void SetEXP(int exp){
        sliderEXP.value = exp;
    }
    public void SetHP(int hp){
        currentHP = hp;
    }
    public void SetMana(int mana){
        sliderMana.value = mana;
    }

    private void Update(){
        sliderHP.value = Mathf.SmoothDamp(sliderHP.value, currentHP, ref velocity, 100*Time.deltaTime);
        
    }
    
}
