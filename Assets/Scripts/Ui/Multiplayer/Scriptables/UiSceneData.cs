using UnityEngine;

namespace Assets.Scripts.Ui.Multiplayer.Scriptables
{
    [CreateAssetMenu(fileName = "Scene", menuName = "ScriptableObjects/Ui/Multiplayer/Scene")]
    public class UiSceneData : ScriptableObject
    {
        [field: SerializeField][field: TextArea] public string Description { get; private set; }
        [field: SerializeField] public int Index { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite Banner { get; private set; }
    }
}
