using UnityEngine;

namespace Assets.Scripts.Core.Environment
{
    public class PhysicsPropertiesContainer : MonoBehaviour
    {
        public const float MIN_TEMPERATURE = -273.15f;

        [Range(0f, 1f)][SerializeField] private float temperatureCombineDistanceDecrease;
        [SerializeField] private float temperatureCombineDistance;

        [field: SerializeField] public float Temperature { get; private set; }

        private void FixedUpdate()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, temperatureCombineDistance);

            foreach (Collider coll in colls)
            {
                if (coll.gameObject.name != gameObject.name)
                {
                    PhysicsPropertiesContainer container = coll.GetComponent<PhysicsPropertiesContainer>();

                    if (container != null)
                        container.CombineTemperature((container.Temperature + Temperature / Mathf.Clamp(Vector3.Distance(transform.position, container.transform.position) * (1f - temperatureCombineDistanceDecrease), 1f, float.PositiveInfinity)) / 2f);
                }
            }
        }
        public void CombineTemperature(float value, bool deltaFactor = false)
        {
            Temperature = Mathf.Clamp(

                Mathf.Lerp(Temperature, value, deltaFactor ? 0.5f * Time.deltaTime : 0.5f),

                MIN_TEMPERATURE,
                float.PositiveInfinity
            );
        }
    }
}