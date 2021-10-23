using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBonus : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int manaReward;
    [SerializeField] float cooldown;

    [Header("References")]
    [SerializeField] CharacterAttack characterAttack;
    [SerializeField] Collider selfCollider;
    [SerializeField] GameObject graphics;
    [SerializeField] AudioSource audioSource;


    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //Play Sound 
            audioSource.Play(); 

            //Deactivate graphics and colliders
            selfCollider.enabled = false;
            graphics.SetActive(false);

            //Give mana back to player
            characterAttack.GainMana(manaReward);

            //Start cooldown
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        selfCollider.enabled = true;
        graphics.SetActive(true);
    }

}
