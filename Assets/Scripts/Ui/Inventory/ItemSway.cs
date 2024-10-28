using Assets.Scripts.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ui.Inventory
{
    public class ItemSway : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody playerRb;

        [Header("Position Properties")]
        [SerializeField] private float maxFallSwayAmount;
        [SerializeField] private float maxSwayAmount;
        [SerializeField] private float swayAmount;
        [SerializeField] private float swayLerpSpeed;

        [Header("Rotation Properties")]
        [SerializeField] private float maxRotSwayAmount;
        [SerializeField] private float rotSwayAmount;
        [SerializeField] private float rotSwayLerpSpeed;

        private Vector2 mouseInput;
        private Vector2 keyInput;
        private Vector3 targetPosition;
        private Vector3 initialPos;
        private Quaternion targetRotation;
        private Quaternion initialRot;

        private void Start()
        {
            initialPos = transform.localPosition;
            initialRot = transform.localRotation;
        }
        private void Update()
        {
            keyInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            targetPosition = initialPos + new Vector3(
                Mathf.Clamp(-mouseInput.x + -keyInput.y, -maxSwayAmount, maxSwayAmount), 
                Mathf.Clamp(-mouseInput.y, -maxSwayAmount, maxSwayAmount) + Mathf.Clamp(-playerRb.velocity.y, -maxFallSwayAmount, maxFallSwayAmount), 
                Mathf.Clamp(-keyInput.x, -maxSwayAmount, maxSwayAmount)) * swayAmount;

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, swayLerpSpeed * Time.deltaTime);

            targetRotation = initialRot * Quaternion.Euler(0f, 0f, Mathf.Clamp(mouseInput.x + keyInput.y, -maxRotSwayAmount, maxRotSwayAmount) * rotSwayAmount);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotSwayLerpSpeed * Time.deltaTime);
        }
    }
}