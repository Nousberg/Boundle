using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Crafting
{
    [CreateAssetMenu(fileName = "CraftRecipe", menuName = "ScriptableObjects/Inventory/CraftRecipe")]
    public class CraftingMaterialData : ScriptableObject
    {
        [field: SerializeField] public int Id;
        [field: SerializeField] public List<CraftingMaterial> RequiredItems { get; private set; } = new List<CraftingMaterial>();
    }
}
