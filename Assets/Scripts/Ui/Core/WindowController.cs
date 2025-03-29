using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui.Core
{
    public class WindowController : MonoBehaviour
    {
        [SerializeField] private List<Window> callableWindows = new List<Window>();

        private Window current;

        public void Open(int id, int caller)
        {
            Window findedWindow = callableWindows.Find(n => n.Identifier == id);

            if (findedWindow != null)
            {
                if (findedWindow.CloseOnRepeat && findedWindow.opened && findedWindow.lastCaller == caller)
                {
                    Close(id);
                    return;
                }

                if (current != null)
                {
                    if (id != current.Identifier && current.CloseOnNew)
                        Close(current.Identifier);
                }

                findedWindow.gameObject.SetActive(true);

                foreach (var subject in findedWindow.Intolerant)
                    Close(subject.Identifier);

                findedWindow.lastCaller = caller;
                findedWindow.opened = true;

                current = findedWindow;
            }
        }

        public void Close(int id)
        {
            Window findedWindow = callableWindows.Find(n => n.Identifier == id);

            if (findedWindow != null)
            {
                foreach (var parallel in findedWindow.Contained)
                    parallel.SetActive(false);

                findedWindow.opened = false;
                findedWindow.gameObject.SetActive(false);

                if (current != null && id == current.Identifier)
                    current = null;
            }
        }
    }
}
