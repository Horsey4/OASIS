using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MSCLoader;
using System;
using System.Collections;
using UnityEngine;

namespace OASIS
{
    public class Bolt : Interactable
    {
        public Renderer renderer { get; private set; }
        public Material materialCache { get; set; }
        public int tightness
        {
            get => _tightness;
            set
            {
                onTightnessSet?.Invoke(value - _tightness);
                transform.localPosition += transform.localRotation * positionStep * (value - _tightness);
                transform.localRotation *= Quaternion.Euler(rotationStep * (value - _tightness));
                _tightness = value;
            }
        }
        public Action<int> onTightnessChanged;
        public float size = 1;
        public int maxTightness = 8;
        public bool canUseRatchet = true;
        public Vector3 positionStep = new Vector3(0, 0, -0.0025f);
        public Vector3 rotationStep = new Vector3(0, 0, 45);
        public bool disableSound;
        public bool useCustomLayerMask;
        internal Action<int> onTightnessSet;
        bool canBeBolted;
        bool onCooldown;
        int _tightness;

        static readonly FsmFloat wrenchSize = FsmVariables.GlobalVariables.FindFsmFloat("ToolWrenchSize");
        static readonly FsmBool usingRatchet = FsmVariables.GlobalVariables.FindFsmBool("PlayerHasRatchet");
        static readonly Material highlightMaterial;
        static readonly FsmBool ratchetSwitch;

        public override void mouseOver()
        {
            if (wrenchSize.Value == size && (!usingRatchet.Value || canUseRatchet))
            {
                if (!canBeBolted)
                {
                    canBeBolted = true;
                    materialCache = renderer.material;
                    renderer.material = highlightMaterial;
                }

                if (usingRatchet.Value)
                {
                    if (Input.mouseScrollDelta.y != 0)
                    {
                        if (ratchetSwitch.Value) StartCoroutine(tryChangeTightness(1, 0.08f));
                        else StartCoroutine(tryChangeTightness(-1, 0.08f));
                    }
                }
                else
                {
                    if (Input.mouseScrollDelta.y > 0) StartCoroutine(tryChangeTightness(1, 0.28f));
                    else if (Input.mouseScrollDelta.y < 0) StartCoroutine(tryChangeTightness(-1, 0.28f));
                }
            }
            else tryResetMaterial();
        }

        public override void mouseExit() => tryResetMaterial();

        public void OnDisable() => tryResetMaterial();

        public void Awake()
        {
            renderer = GetComponent<Renderer>();
            if (!useCustomLayerMask) layerMask = 1 << 12;

            gameObject.SetActive(false);
            transform.localPosition += transform.localRotation * positionStep * -maxTightness;
            transform.localRotation *= Quaternion.Euler(rotationStep * -maxTightness);
        }

        IEnumerator tryChangeTightness(int value, float cooldown)
        {
            if (onCooldown || tightness + value > maxTightness || tightness + value < 0) yield break;

            tightness += value;
            onTightnessChanged?.Invoke(value);
            if (!disableSound) MasterAudio.PlaySound3DAndForget("CarBuilding", sourceTrans: transform, variationName: "bolt_screw");

            onCooldown = true;
            yield return new WaitForSeconds(cooldown);
            onCooldown = false;
        }

        void tryResetMaterial()
        {
            if (canBeBolted)
            {
                canBeBolted = false;
                renderer.material = materialCache;
                materialCache = null;
            }
        }

        static Bolt()
        {
            var spanner = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").transform.Find("2Spanner");

            var fsm = spanner.Find("Pivot/Ratchet").GetComponent<PlayMakerFSM>();
            fsm.InitializeFSM();
            ratchetSwitch = fsm.FsmVariables.FindFsmBool("Switch");

            fsm = spanner.Find("Raycast").GetComponents<PlayMakerFSM>()[1];
            fsm.InitializeFSM();
            highlightMaterial = ((SetMaterial)fsm.FsmStates[2].Actions[1]).material.Value;
        }
    }
}