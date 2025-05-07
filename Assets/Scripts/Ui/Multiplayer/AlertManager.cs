using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui.Multiplayer
{
    public class AlertManager : MonoBehaviour
    {
        [SerializeField] private Transform alertsParent;
        [SerializeField] private GameObject defaultAlert;
        [SerializeField] private GameObject warningAlert;
        [SerializeField] private GameObject loadingAlert;

        public List<Alert> Alerts { get; private set; } = new List<Alert>();

        private void Start()
        {
            if (DataContainer.wasBanned)
                WarningAlert("You have been banned in the current session.", 5f);
            else if (DataContainer.wasKicked)
                WarningAlert("You have been kicked out from the current session.", 5f);

            DataContainer.wasBanned = false;
            DataContainer.wasKicked = false;

            DefaultAlert("Thank you for playing this game!", 5f);
        }
        public void DefaultAlert(string text, float duration)
        {
            Alert alert = Instantiate(defaultAlert, alertsParent).GetComponent<Alert>();
            alert.Text.text = text;

            alert.OnShown(duration);
            Alerts.Add(alert);
        }
        public void WarningAlert(string text, float duration)
        {
            Alert alert = Instantiate(warningAlert, alertsParent).GetComponent<Alert>();
            alert.Text.text = text;

            alert.OnShown(duration);
            Alerts.Add(alert);
        }
        public int LoadingAlert(string text)
        {
            Alert alert = Instantiate(loadingAlert, alertsParent).GetComponent<Alert>();
            alert.Text.text = text;

            Alerts.Add(alert);

            return Alerts.IndexOf(alert);
        }
    }
}
