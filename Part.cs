using System;
using UnityEngine;

namespace OASIS
{
    public class Part : Interactable
    {
        public class RigidbodyCache
        {
            public float mass;
            public float drag;
            public float angularDrag;
            public bool useGravity;
            public bool isKinematic;
            public RigidbodyInterpolation interpolation;
            public CollisionDetectionMode collisionDetectionMode;
            public RigidbodyConstraints constraints;

            public RigidbodyCache(Rigidbody rigidbody)
            {
                mass = rigidbody.mass;
                drag = rigidbody.drag;
                angularDrag = rigidbody.angularDrag;
                useGravity = rigidbody.useGravity;
                isKinematic = rigidbody.isKinematic;
                interpolation = rigidbody.interpolation;
                collisionDetectionMode = rigidbody.collisionDetectionMode;
                constraints = rigidbody.constraints;
            }

            public void applyTo(Rigidbody rigidbody)
            {
                rigidbody.mass = mass;
                rigidbody.drag = drag;
                rigidbody.angularDrag = angularDrag;
                rigidbody.useGravity = useGravity;
                rigidbody.isKinematic = isKinematic;
                rigidbody.interpolation = interpolation;
                rigidbody.collisionDetectionMode = collisionDetectionMode;
                rigidbody.constraints = constraints;
            }
        }

        public RigidbodyCache rigidbodyCache { get; private set; }
        public Rigidbody rigidbody { get; private set; }
        public int attachedTo
        {
            get => _attachedTo;
            set
            {
                if (value == _attachedTo) return;

                if (value == -1)
                {
                    if (bolts != null)
                    {
                        for (var i = 0; i < bolts.Length; i++)
                        {
                            bolts[i].gameObject.SetActive(false);
                            bolts[i].tightness = 0;
                        }
                    }

                    rigidbody = gameObject.AddComponent<Rigidbody>();
                    rigidbodyCache.applyTo(rigidbody);
                    rigidbodyCache = null;

                    transform.SetParent(null);
                    tag = "PART";

                    triggers[_attachedTo].enabled = true;
                }
                else
                {
                    if (bolts != null)
                    {
                        for (var i = 0; i < bolts.Length; i++) bolts[i].gameObject.SetActive(true);
                    }
                    if (!rigidbody) rigidbody = GetComponent<Rigidbody>();

                    if (_attachedTo == -1)
                    {
                        rigidbodyCache = new RigidbodyCache(rigidbody);
                        Destroy(rigidbody);
                    }
                    else triggers[_attachedTo].enabled = true;

                    transform.SetParent(triggers[value].transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    tag = "Untagged";

                    triggers[value].enabled = false;
                }

                _attachedTo = value;
            }
        }
        public int tightness
        {
            get
            {
                if (bolts == null) return 0;

                var sum = 0;
                for (var i = 0; i < bolts.Length; i++) sum += bolts[i].tightness;
                return sum;
            }
        }
        public Action<int> onAttach;
        public Action<int> onDetach;
        public Func<bool> canAttach;
        public Func<bool> canDetach;
        public Collider[] triggers;
        public Bolt[] bolts;
        public bool disableSound;
        public bool useCustomLayerMask;
        int _attachedTo = -1;
        int triggerIndex = -1;

        public override void mouseOver()
        {
            if (attachedTo == -1) return;
            if (bolts != null)
            {
                for (var i = 0; i < bolts.Length; i++)
                {
                    if (bolts[i].tightness != 0)
                    {
                        CursorGUI.disassemble = false;
                        return;
                    }
                }
            }
            if (canDetach != null && !canDetach.Invoke()) return;

            if (Input.GetMouseButtonDown(1))
            {
                var index = attachedTo;
                attachedTo = -1;
                onDetach?.Invoke(index);
                if (!disableSound) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "disassemble");
                CursorGUI.disassemble = false;
            }
            else CursorGUI.disassemble = true;
        }

        public override void mouseExit()
        {
            if (attachedTo != -1) CursorGUI.disassemble = false;
        }

        public void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            if (!useCustomLayerMask) layerMask = 1 << 19;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (transform.parent == null) return;

            var i = Array.IndexOf(triggers, other);
            if (i != -1) triggerIndex = i;
        }

        public void OnTriggerExit(Collider other)
        {
            if (triggerIndex != -1 && other == triggers[triggerIndex])
            {
                triggerIndex = -1;
                CursorGUI.assemble = false;
            }
        }

        public void LateUpdate()
        {
            if (attachedTo != -1 || triggerIndex == -1 || (canAttach != null && !canAttach.Invoke())) return;

            if (Input.GetMouseButtonDown(0))
            {
                attachedTo = triggerIndex;
                onAttach?.Invoke(triggerIndex);
                triggerIndex = -1;
                if (!disableSound) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "assemble");
                CursorGUI.assemble = false;
            }
            else CursorGUI.assemble = true;
        }
    }
}