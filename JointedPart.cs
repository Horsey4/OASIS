using System;
using UnityEngine;

namespace OASIS
{
    public class JointedPart : BasePart
    {
        public FixedJoint joint { get; private set; }
        public Action<int, float> onBreak;
        public float breakForce = Mathf.Infinity;
        public float breakForceStep;
        public int unbreakableAtTightness = -1;

        public void Start()
        {
            if (bolts == null) return;

            for (var i = 0; i < bolts.Length; i++)
            {
                bolts[i].onTightnessSet += (deltaTightness) =>
                {
                    if (unbreakableAtTightness >= 0 && tightness >= unbreakableAtTightness)
                    {
                        joint.breakForce = Mathf.Infinity;
                        joint.breakTorque = Mathf.Infinity;
                    }
                    else
                    {
                        joint.breakForce = breakForce + breakForceStep * tightness;
                        joint.breakTorque = joint.breakForce;
                    }
                };
            }
        }

        public void OnJointBreak(float breakForce)
        {
            var index = attachedTo;
            detach();
            onBreak?.Invoke(index, breakForce);
            if (!disableSound) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "disassemble");
        }

        public override void attach(int index)
        {
            base.attach(index);

            if (joint) Destroy(joint);
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = transform.parent.GetComponentInParent<Rigidbody>();
            joint.breakForce = breakForce;
            joint.breakTorque = breakForce;
        }

        public override void detach()
        {
            base.detach();

            if (joint) Destroy(joint);
        }
    }
}