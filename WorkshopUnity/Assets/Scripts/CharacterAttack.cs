using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class CharacterAttack : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] string attackButton;

    [Header("References")]
    [SerializeField] SpellData data;
    [SerializeField] CharacterControllerCustom controller;
    [SerializeField] Transform self;
    [SerializeField] Animator selfAnim;
    [SerializeField] AudioSource selfAudio;
    [SerializeField] Image manaFillBar;
    [SerializeField] Image spellFillBar;
    [SerializeField] VisualEffect blastEffect;
    [SerializeField] ParticleSystem spellBall;
    [SerializeField] Transform spellBallTransform;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    private float currentManaPool;
    private bool canCastSpell = true;
    private bool castingSpell;
    private Collider[] colliders;
    private int mask = 1<<6;

    void Start()
    {
        currentManaPool = data.maxManaPool;
    }

    void Update()
    {
        Attack();

        if (castingSpell)
            spellBallTransform.position = leftHand.position + ((rightHand.position - leftHand.position)/2) + self.forward * 0.5f;

        //Update ManaFill Bar
        manaFillBar.fillAmount = Mathf.Lerp(manaFillBar.fillAmount, currentManaPool / data.maxManaPool, Time.deltaTime*5);

        if (currentManaPool == 0) manaFillBar.fillAmount = 0;
        else if (currentManaPool == data.maxManaPool) manaFillBar.fillAmount = 1;

    }

    void Attack()
    {
        if(Input.GetButtonDown(attackButton) && canCastSpell && currentManaPool - data.spellManaCost >=0 && controller.isGrounded)
        {
            selfAnim.SetTrigger("Spell");
            selfAnim.SetFloat("Speed", 0);
            currentManaPool -= data.spellManaCost;
            canCastSpell = false;
            controller.canMove = false;
            castingSpell = true;
            spellBall.Play();

            StartCoroutine(CooldownCoroutine());
        }
    }

    public void Blast()
    {
        //ActuallyCastSpell;
        blastEffect.Play();
        selfAudio.Play();
        colliders = Physics.OverlapSphere(self.position, data.spellRange, mask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 forceDir = (colliders[i].transform.position - self.position) + Vector3.up * data.upwardForceFactor;
            if(data.applyDistanceFactor)
                forceDir = forceDir * (1 - (colliders[i].transform.position - self.position).magnitude / data.spellRange);

            colliders[i].GetComponent<Rigidbody>().AddForce(forceDir.normalized * data.blastForce, ForceMode.Impulse);
        }
        castingSpell = false;
        spellBall.Stop();
    }

    public void PlayerCanMove()
    {
        controller.canMove = true;
    }

    IEnumerator CooldownCoroutine()
    {
        float elapsedTime = 0;
        spellFillBar.fillAmount = 1;
        while (elapsedTime < data.cooldownDuration)
        {
            elapsedTime += Time.deltaTime;
            spellFillBar.fillAmount = 1 - elapsedTime / data.cooldownDuration;
            yield return null;
        }
        spellFillBar.fillAmount = 0;
        canCastSpell = true;
    }

    public void GainMana(int amount)
    {
        if (currentManaPool + amount >= data.maxManaPool)
            currentManaPool = data.maxManaPool;
        else
            currentManaPool += amount;
    }
}
