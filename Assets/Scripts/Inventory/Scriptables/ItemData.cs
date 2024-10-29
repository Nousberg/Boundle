using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Inventory/Item")]
public class ItemData : ScriptableObject
{
    [field: Header("Base Properties")]
    [field: SerializeField] public bool Stackable { get; private set; }
    [field: SerializeField] public bool Dropable { get; private set; }
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public int MaxQuantity { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: Header("Other Properties")]
    [field: SerializeField] public Vector3 PrefabScale { get; private set; }
}
