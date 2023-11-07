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

        public RigidbodyCache rigidbodyCache { get; private set; }

        public override void attach(int index)
        {
            base.attach(index);

            if (rigidbody)
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