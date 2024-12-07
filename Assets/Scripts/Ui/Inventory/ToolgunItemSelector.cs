using Assets.Scripts.Spawning;
using Assets.Scripts.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui.Inventory
{
    public class ToolgunItemSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform riflesCategoryParent;
        [SerializeField] private Transform meleesCategoryParent;
        [SerializeField] private Transform syringesParent;
        [SerializeField] private Transform propsCategoryParent;
        [SerializeField] private ToolgunDataController toolgun;
        [SerializeField] private Summonables spawnManager;

        private void Start()
        {
            foreach (var obj in spawnManager.objects)
            {
                GameObject icon = new GameObject("Icon");
                icon.transform.parent = MatchParent(obj.Category);
                
                Image iconImg = icon.AddComponent<Image>();
                iconImg.sprite = obj.Icon;

                Button iconButt = icon.AddComponent<Button>();
                iconButt.onClick.AddListener(() => toolgun.SelectObjectToSpawn(obj.ObjectId));
            }
        }
        private Transform MatchParent(Summonables.ObjectCategory category)
        {
            switch (category)
            {
                case Summonables.ObjectCategory.Rifle:
                    return riflesCategoryParent;
                case Summonables.ObjectCategory.Prop:
                    return propsCategoryParent;
                case Summonables.ObjectCategory.Syringe:
                    return syringesParent;
            }

            return null;
        }
    }
}