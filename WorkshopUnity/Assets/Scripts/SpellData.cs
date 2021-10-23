using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName ="Data/SpellData", order = 2)]
public class SpellData : ScriptableObject
{
    [Header("Stats")]
    public float maxManaPool;
    public float spellRange;
    public float blastForce;
    [Range(0f, 5f)]
    public float upwardForceFactor;
    public float cooldownDuration;
    public float spellManaCost;
    public bool applyDistanceFactor;
}
