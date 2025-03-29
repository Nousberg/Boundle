using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui.Core
{
    public class Window : MonoBehaviour
    {
        [field: SerializeField] public int Identifier { get; private set; }
        [field: SerializeField] public bool CloseOnNew { get; private set; }
        [field: SerializeField] public bool CloseOnRepeat { get; private set; }
        [field: SerializeField] public List<GameObject> Contained { get; private set; }
        [field: SerializeField] public List<Window> Intolerant { get; private set; }

        [Header("Other")]
        public int lastCaller;
        public bool opened;
    }
}
