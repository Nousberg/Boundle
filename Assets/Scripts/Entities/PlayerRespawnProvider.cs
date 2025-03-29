using System.Collections;
using UnityEngine;
using Assets.Scripts.Core.Input_System;
using Assets.Scripts.Core.Environment;
using Assets.Scripts.Entities.Abilities;

namespace Assets.Scripts.Entities
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(FlyAbility))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRespawnProvider : MonoBehaviour
    {
        [SerializeField] private float respawnDuration;

        private Entity player => GetComponent<Entity>();
        private Rigidbody rb => GetComponent<Rigidbody>();
        private FlyAbility fly => GetComponent<FlyAbility>();

        private InputMachine input;
        private SceneData sceneData;
        private bool respawning;

        public void Init(InputMachine input, SceneData sceneData)
        {
            this.input = input;
            this.sceneData = sceneData;

            player.OnDeath += () => { StartCoroutine(Respawn()); };
        }
        private IEnumerator Respawn()
        {
            if (respawning)
                yield break;

            input.BlockInput(true);

            respawning = true;

            fly.Deactivate();

            rb.constraints = RigidbodyConstraints.None;
            
            yield return new WaitForSeconds(respawnDuration);

            input.BlockInput(false);
            player.Heal(player.BaseHealth);

            transform.position = sceneData.PlayersSpawnPosition;
            transform.rotation = sceneData.PlayersSpawnRotation;

            respawning = false;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
