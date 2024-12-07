using Assets.Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    //this is a test component

    [RequireComponent(typeof(Entity))]
    public class HealthLine : MonoBehaviour
    {
        [SerializeField] private Image health;
        [SerializeField] private TextMeshProUGUI txt;

        private Entity player => GetComponent<Entity>();
        private Color initColor;

        private void Start()
        {
            initColor = health.color;
        }
        private void Update()
        {
            health.fillAmount = player.Health / player.BaseHealth;
            health.color = Color.Lerp(Color.red, initColor, player.Health / player.BaseHealth);
            txt.text = ((int)player.Health).ToString();
        }
    }
}