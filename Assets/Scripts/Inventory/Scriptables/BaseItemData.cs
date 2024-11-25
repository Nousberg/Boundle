using UnityEngine;

namespace Assets.Scripts.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "BaseItemData", menuName = "ScriptableObjects/Inventory/BaseItemData")]
    public class BaseItemData : ScriptableObject
    {
        [field: SerializeField] public bool Dropable { get; private set; }
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public float Weight { get; private set; }
        [field: SerializeField] public GameObject prefab { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
