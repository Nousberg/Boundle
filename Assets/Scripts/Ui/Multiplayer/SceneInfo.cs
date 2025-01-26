using Assets.Scripts.Ui.Multiplayer.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class SceneInfo : MonoBehaviour
    {
        [field: SerializeField] public List<UiSceneData> Scenes { get; private set; } = new List<UiSceneData>();
    }
}
