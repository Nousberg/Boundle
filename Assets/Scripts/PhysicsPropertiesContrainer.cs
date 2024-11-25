using System.Security;
using UnityEngine;

namespace Assets.Scripts
{
    public class PhysicsPropertiesContrainer : MonoBehaviour
    {
        public const float MIN_TEMPERATURE = -273.15f;

        [Range(0f, 1f)][SerializeField] private float temperatureCombineDistanceDecrease;
        [SerializeField] private float temperatureCombineDistance;
        [SerializeField] private float baseTemperature;
        [SerializeField] private float temperatureStabilizationRate;

        [field: SerializeField] public float Temperature { get; private set; }

        private void Start() => Temperature = baseTemperature;

        private void FixedUpdate()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, temperatureCombineDistance);

            foreach (Collider coll in colls)
            {
                if (coll.gameObject.name != gameObject.name)
                {
                    PhysicsPropertiesContrainer container = coll.GetComponent<PhysicsPropertiesContrainer>();

                    if (container != null)
                        container.CombineTemperature(container.Temperature + Temperature / Mathf.Clamp(Vector3.Distance(transform.position, container.transform.position) * (1f - temperatureCombineDistanceDecrease), 1f, float.PositiveInfinity));
                }
            }

            Temperature = Mathf.Lerp(Temperature, baseTemperature, temperatureStabilizationRate);
        }

        public void Extingush()
        {
            Temperature = baseTemperature;
        }
        public void CombineTemperature(float value)
        {
            Temperature = Mathf.Clamp(

                Mathf.Lerp(baseTemperature, value * 0.5f, 0.5f),

                MIN_TEMPERATURE,
                float.PositiveInfinity
            );
        }
    }
}