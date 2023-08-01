using System.Collections.Generic;
using UnityEngine;

namespace OASIS
{
    public abstract class Interactable : MonoBehaviour
    {
        public bool isMouseOver { get; private set; }
        public LayerMask layerMask = ~(1 << 2);
        public float maxInteractionDistance = 1;

        static Dictionary<int, RaycastHit> raycasts = new Dictionary<int, RaycastHit>();
        static Camera camera;
        static int lastFrame;
        static Ray ray;

        public virtual void mouseEnter() { }

        public virtual void mouseOver() { }

        public virtual void mouseExit() { }

        public void Update()
        {
            if (raycast(out var hit, maxInteractionDistance, layerMask) && hit.collider.gameObject == gameObject)
            {
                if (!isMouseOver)
                {
                    isMouseOver = true;
                    mouseEnter();
                }
                mouseOver();
            }
            else if (isMouseOver)
            {
                isMouseOver = false;
                mouseExit();
            }
        }

        public static bool raycast(out RaycastHit hit, float distance, int layerMask)
        {
            if (Time.frameCount != lastFrame)
            {
                raycasts.Clear();
                camera = Camera.main;
                if (camera) ray = camera.ScreenPointToRay(Input.mousePosition);

                lastFrame = Time.frameCount;
            }

            if (!camera)
            {
                hit = default;
                return false;
            }

            if (raycasts.TryGetValue(layerMask, out var cache))
            {
                if (cache.collider || cache.distance >= distance)
                {
                    hit = cache;
                    return cache.distance <= distance;
                }

                Physics.Raycast(ray.origin + ray.direction * cache.distance, ray.direction, out hit, distance - cache.distance, layerMask);
                hit.distance += cache.distance;
                raycasts[layerMask] = hit;
                return hit.collider;
            }
            else
            {
                Physics.Raycast(ray, out hit, distance, layerMask);
                raycasts.Add(layerMask, hit);
                return hit.collider;
            }
        }
    }
}