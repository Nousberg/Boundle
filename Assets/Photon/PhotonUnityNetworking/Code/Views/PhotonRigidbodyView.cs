﻿using UnityEngine;

namespace Photon.Pun
{
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Photon Networking/Photon Rigidbody View")]
    public class PhotonRigidbodyView : MonoBehaviourPun, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        public Rigidbody m_Body { get; private set; }

        private Vector3 m_NetworkPosition;
        private Quaternion m_NetworkRotation;

        public bool syncGravityUse;

        [HideInInspector]
        public bool m_SynchronizeVelocity = true;
        [HideInInspector]
        public bool m_SynchronizeAngularVelocity = false;

        [HideInInspector]
        public bool m_TeleportEnabled = false;
        [HideInInspector]
        public float m_TeleportIfDistanceGreaterThan = 3.0f;

        public void Awake()
        {
            this.m_Body = GetComponent<Rigidbody>();

            this.m_NetworkPosition = new Vector3();
            this.m_NetworkRotation = new Quaternion();
        }

        public void FixedUpdate()
        {
            if (!this.photonView.IsMine)
            {
                this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        [PunRPC]
        public void AddForce(Vector3 f)
        {
            if (photonView.IsMine)
                this.m_Body.AddForce(f);
        }
        [PunRPC]
        public void SetPosition(Vector3 p)
        {
            if (photonView.IsMine)
                this.m_Body.position = p;
        }
        [PunRPC]
        public void SetVelocity(Vector3 v)
        {
            if (photonView.IsMine)
                this.m_Body.velocity = v;
        }
        [PunRPC]
        public void SetGravityUse(bool state)
        {
            if (photonView.IsMine)
                this.m_Body.useGravity = state;
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (syncGravityUse)
                    stream.SendNext(m_Body.useGravity);

                stream.SendNext(this.m_Body.position);
                stream.SendNext(this.m_Body.rotation);

                if (this.m_SynchronizeVelocity)
                {
                    stream.SendNext(this.m_Body.velocity);
                }

                if (this.m_SynchronizeAngularVelocity)
                {
                    stream.SendNext(this.m_Body.angularVelocity);
                }
            }
            else
            {
                m_Body.useGravity = (bool)stream.ReceiveNext();

                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                if (this.m_TeleportEnabled)
                {
                    if (Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                    {
                        this.m_Body.position = this.m_NetworkPosition;
                    }
                }

                if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    if (this.m_SynchronizeVelocity)
                    {
                        this.m_Body.velocity = (Vector3)stream.ReceiveNext();
                        this.m_NetworkPosition += this.m_Body.velocity * lag;
                        this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
                    }

                    if (this.m_SynchronizeAngularVelocity)
                    {
                        this.m_Body.angularVelocity = (Vector3)stream.ReceiveNext();
                        this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * lag) * this.m_NetworkRotation;
                        this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
                    }
                }
            }
        }
    }
}