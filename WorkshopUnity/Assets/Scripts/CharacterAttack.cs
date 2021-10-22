using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAttack : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] string attackButton;

    [Header("Stats")]
    [SerializeField] float maxManaPool;
    [SerializeField] float spellRange;
    [SerializeField] float blastForce;
    [SerializeField] float cooldownDuration;
    [SerializeField] float spellManaCost;


    [Header("References")]
    [SerializeField] CharacterControllerCustom controller;
    [SerializeField] Transform self;
    [SerializeField] Animator selfAnim;
    [SerializeField] Image manaFillBar;
    [SerializeField] Image spellFillBar;

    private float currentManaPool;
    private bool canCastSpell = true;
    private Collider[] colliders;
    private int mask = 1<<6;

    // Start is called before the first frame update
    void Start()
    {
        currentManaPool = maxManaPool;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();

        //Update ManaFill Bar
        manaFillBar.fillAmount = Mathf.Lerp(manaFillBar.fillAmount, currentManaPool / maxManaPool, Time.deltaTime*5);

        if (currentManaPool == 0) manaFillBar.fillAmount = 0;
        else if (currentManaPool == maxManaPool) manaFillBar.fillAmount = 1;

    }

    void Attack()
    {
        if(Input.GetButtonDown(attackButton) && canCastSpell && currentManaPool - spellManaCost >=0 && controller.isGrounded)
        {
            selfAnim.SetTrigger("Spell");
            selfAnim.SetFloat("Speed", 0);
            currentManaPool -= spellManaCost;
            canCastSpell = false;
            controller.canMove = false;

            StartCoroutine(CooldownCoroutine());
        }
    }

    public void Blast()
    {
        //ActuallyCastSpell;
        colliders = Physics.OverlapSphere(self.position, spellRange, mask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 forceDir = (colliders[i].transform.position - self.position) + Vector3.up * 3;
            colliders[i].GetComponent<Rigidbody>().AddForce(forceDir.normalized * blastForce, ForceMode.Impulse);
        }
    }
    public void PlayerCanMove()
    {
        controller.canMove = true;
    }

    IEnumerator CooldownCoroutine()
    {
        float elapsedTime = 0;
        spellFillBar.fillAmount = 1;
        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;
            spellFillBar.fillAmount = 1 - elapsedTime / cooldownDuration;
            yield return null;
        }
        spellFillBar.fillAmount = 0;
        canCastSpell = true;
    }
}
