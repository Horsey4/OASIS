using UnityEngine;

namespace OASIS
{
    public class Part : BasePart
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

        public RigidbodyCache rigidbodyCache { get; internal set; }
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

        public override void attach(int index)
        {
            base.attach(index);

            if (_attachedTo == -1)
            {
                rigidbodyCache = new RigidbodyCache(rigidbody);
                Destroy(rigidbody);
            }
        }

        public override void detach()
        {
            base.detach();

            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbodyCache.applyTo(rigidbody);
            rigidbodyCache = null;
        }
    }
}