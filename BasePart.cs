using System;
using UnityEngine;

namespace OASIS
{
    public class BasePart : Interactable
    {
        public Rigidbody rigidbody { get; internal set; }
        public int attachedTo
        {
            get => _attachedTo;
            set
            {
                if (value == _attachedTo) return;

                if (value == -1) detach();
                else attach(value);

                _attachedTo = value;
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
        internal string tagCache;
        internal int _attachedTo = -1;
        internal int triggerIndex = -1;

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

        internal virtual void attach(int index)
        {
            if (bolts != null)
            {
                for (var i = 0; i < bolts.Length; i++) bolts[i].gameObject.SetActive(true);
            }
            if (!rigidbody) rigidbody = GetComponent<Rigidbody>();

            transform.SetParent(triggers[index].transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            if (tag == "Untagged") tagCache = "PART";
            else tagCache = tag;
            tag = "Untagged";

            if (_attachedTo != -1) triggers[_attachedTo].enabled = true;
            triggers[index].enabled = false;
        }

        internal virtual void detach()
        {
            if (bolts != null)
            {
                for (var i = 0; i < bolts.Length; i++)
                {
                    bolts[i].gameObject.SetActive(false);
                    bolts[i].tightness = 0;
                }
            }

            transform.SetParent(null);
            tag = tagCache;
            tagCache = null;

            triggers[_attachedTo].enabled = true;
        }
    }
}