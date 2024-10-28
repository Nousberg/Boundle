using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class ItemContainer : MonoBehaviour
    {
        [field: SerializeField] public ItemData Data { get; private set; }
        [field: SerializeField] public Animator ItemAnimator { get; private set; }
        [field: SerializeField] public Animator HandsAnimator { get; private set; }
    }
}