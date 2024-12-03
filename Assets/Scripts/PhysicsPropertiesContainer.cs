using System.Security;
using UnityEngine;

namespace Assets.Scripts
{
    public class PhysicsPropertiesContainer : MonoBehaviour
    {
        public const float MIN_TEMPERATURE = -273.15f;

        [Range(0f, 1f)][SerializeField] private float temperatureCombineDistanceDecrease;
        [SerializeField] private float temperatureCombineDistance;
        [SerializeField] private float temperatureStabilizationRate;

        [field: SerializeField] public float BaseTemperature { get; private set; }
        [field: SerializeField] public float Temperature { get; private set; }

        private void Start() => Temperature = BaseTemperature;

        private void FixedUpdate()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, temperatureCombineDistance);

            foreach (Collider coll in colls)
            {
                if (coll.gameObject.name != gameObject.name)
                {
                    PhysicsPropertiesContainer container = coll.GetComponent<PhysicsPropertiesContainer>();

                    if (container != null)
                        container.CombineTemperature(container.Temperature + Temperature / Mathf.Clamp(Vector3.Distance(transform.position, container.transform.position) * (1f - temperatureCombineDistanceDecrease), 1f, float.PositiveInfinity));
                }
            }

            Temperature = Mathf.Lerp(Temperature, BaseTemperature, temperatureStabilizationRate);
        }

        public void Extingush()
        {
            Temperature = BaseTemperature;
        }
        public void CombineTemperature(float value)
        {
            Temperature = Mathf.Clamp(

                Mathf.Lerp(BaseTemperature, value * 0.5f, 0.5f),

                MIN_TEMPERATURE,
                float.PositiveInfinity
            );
        }
    }
}