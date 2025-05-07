using Assets.Scripts.Core.Environment.Scriptables;
using Assets.Scripts.Core.Sound;
using Assets.Scripts.Effects;
using Assets.Scripts.Entities;
using Assets.Scripts.Movement;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Core.Environment
{
    public class EnterableLiquid : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private EnvironmentManager environment;
        [SerializeField] private EnterableLiquidData data;

        private EnvironmentManager.FogData fog = new EnvironmentManager.FogData(1f, Color.white, 0.01f);
        private PlayerMovementLogic movement;
        private Collider col;

        private void Start()
        {
            col = GetComponent<Collider>();

            fog.intensity = data.whileInTintIntensity;
            fog.color = data.whileInColor;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PhotonView>(out var view) && view.IsMine && other.TryGetComponent<Rigidbody>(out var rb))
            {
                if (other.TryGetComponent<EffectContainer>(out var container) && other.TryGetComponent<Entity>(out var entity))
                {
                    foreach (var effect in data.AppliedEffects)
                    {
                        switch (effect.effectType)
                        {
                            case EnterableLiquidData.Effect.Resistance:
                                container.ApplyEffect(new Resistance(entity, effect.duration, effect.amplifier, effect.infinite));
                                break;
                        }
                    }
                }

                if (movement == null && other.TryGetComponent<PlayerMovementLogic>(out var mov))
                    movement = mov;

                Transform t = other.transform;
                float velocity = rb.velocity.magnitude;

                if (data.AfterEnterFoam != null && velocity > data.FoamInstantiateVelocity)
                {
                    GameObject foam = Instantiate(data.AfterEnterFoam);
                    Vector3 scale = foam.transform.localScale;
                    Vector3 pos = new Vector3(t.position.x, col.bounds.max.y, t.position.z);
                    float scaleModifier = velocity / data.FoamInstantiateVelocity;

                    scale.x *= scaleModifier;
                    scale.y *= scaleModifier;

                    foam.transform.position = pos;

                    soundManager.View.RPC(nameof(SoundManager.Play), RpcTarget.All, data.splashSource, data.splashClip, pos, velocity, Mathf.Lerp(0f, 3f, 1f - velocity / 200f), false, false, true);
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<PhotonView>(out var view) && view.IsMine)
            {
                if (col.bounds.Contains(other.transform.position) && other.TryGetComponent<PhysicsPropertiesContainer>(out var phys))
                {
                    float waveIntensity = data.objectWaveIntensity / phys.Weight;

                    Vector3 vel = other.attachedRigidbody.velocity;
                    vel.y += waveIntensity;

                    Vector3 angVel = other.attachedRigidbody.angularVelocity;
                    angVel.x += waveIntensity;
                    angVel.z += waveIntensity;

                    other.attachedRigidbody.velocity = vel;
                    other.attachedRigidbody.angularVelocity = angVel;
                }

                if (movement != null && other.gameObject == movement.gameObject)
                {
                    movement.ToggleUnderwater(true);

                    if (col.bounds.Contains(other.bounds.max))
                    {
                        if (!environment.ContainsFog(fog))
                            environment.BlendFog(fog);

                        if (other.TryGetComponent<AudioReverbFilter>(out var filter))
                            filter.dryLevel = -10000f;
                    }
                    else
                    {
                        environment.RemoveFog(fog);

                        if (other.TryGetComponent<AudioReverbFilter>(out var filter))
                            filter.dryLevel = 0f;
                    }
                }

                other.attachedRigidbody.drag = data.dragModifier;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PhotonView>(out var view) && view.IsMine)
            {
                other.attachedRigidbody.drag = 0f;

                if (movement != null && other.gameObject == movement.gameObject)
                {
                    movement.ToggleUnderwater(false);
                    environment.RemoveFog(fog);
                }
            }
        }
    }
}