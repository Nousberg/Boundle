using UnityEngine;
using Assets.Scripts.Ui.Player;
using Assets.Scripts.Ui.Crosshair;

namespace Assets.Scripts.Inventory.Scriptables
{
    [CreateAssetMenu(fileName = "BaseItemData", menuName = "ScriptableObjects/Inventory/BaseItemData")]
    public class BaseItemData : ScriptableObject
    {
        [field: SerializeField] public bool Dropable { get; private set; }
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public float Weight { get; private set; }
        [field: SerializeField] public Vector3 ColliderScale { get; private set; }
        [field: SerializeField] public CrosshairController.Crosshair Crosshair { get; private set; }
        [field: SerializeField] public CrosshairController.Crosshair ActionCrosshair { get; private set; }
        [field: SerializeField] public string prefab { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
