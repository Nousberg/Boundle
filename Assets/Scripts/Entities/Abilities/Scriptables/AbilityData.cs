﻿using UnityEngine;

namespace Assets.Scripts.Entities.Abilities.Scriptables
{

    [CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/Abilities/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [field: SerializeField] public KeyCode KeyBind { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float StartDelay { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
    }
}