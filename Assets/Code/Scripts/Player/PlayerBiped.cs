using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Bipedal;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Experimental.Rendering.Universal;

namespace BoschingMachine.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerBiped : Biped
    {
        [Space]
        [SerializeField] InputActionAsset inputAsset;
        [SerializeField] Cinemachine.CinemachineVirtualCamera fpCamera;
        [SerializeField] RenderObjects[] viewmodelObjects;

        [Space]
        [SerializeField] float defaultFOV;

        [Space]
        [SerializeField] float lookDeltaSensitivity;
        [SerializeField] float lookAdditiveSensitivity;

        [Space]
        [SerializeField] PlayerWeaponsManager weaponsManager;

        Vector2 lookRotation;

        InputAction move;
        InputAction jump;
        InputAction lookDelta;
        InputAction lookAdditive;

        InputAction fire;
        InputAction aim;
        InputAction reload;

        public override Vector3 MoveDirection
        {
            get
            {
                Vector2 input = move.ReadValue<Vector2>();
                return transform.TransformDirection(input.x, 0.0f, input.y);
            }
        }

        public override bool Jump => ReadFlag(jump);
        public override Vector2 LookRotation => lookRotation;
        public float FOV { get; set; }
        public float ViewmodelFOV { get; set; }

        protected override void Awake()
        {
            base.Awake();

            var playerMap = inputAsset.FindActionMap("Player");
            playerMap.Enable();

            move = playerMap.FindAction("move");
            jump = playerMap.FindAction("jump");
            lookDelta = playerMap.FindAction("lookDelta");
            lookAdditive = playerMap.FindAction("lookAdditive");

            fire = playerMap.FindAction("fire");
            aim = playerMap.FindAction("aim");
            reload = playerMap.FindAction("reload");
        }

        public bool ReadFlag(InputAction action) => action.ReadValue<float>() > 0.5f;

        protected override void OnEnable()
        {
            base.OnEnable();

            Cursor.lockState = CursorLockMode.Locked;

            fire.performed += _ => weaponsManager.PrimaryFireDown();
            fire.canceled += _ => weaponsManager.PrimaryFireUp();

            aim.performed += _ => weaponsManager.SeccondaryFireDown();
            aim.canceled += _ => weaponsManager.SeccondaryFireUp();

            reload.performed += _ => weaponsManager.Reload();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Cursor.lockState = CursorLockMode.None;

            fire.performed -= _ => weaponsManager.PrimaryFireDown();
            fire.canceled -= _ => weaponsManager.PrimaryFireUp();

            aim.performed -= _ => weaponsManager.SeccondaryFireDown();
            aim.canceled -= _ => weaponsManager.SeccondaryFireUp();

            reload.performed -= _ => weaponsManager.Reload();
        }

        protected override void Start()
        {
            base.Start();

            weaponsManager.Setup(this, 0);
        }

        protected override void Update()
        {
            lookRotation += GetLookDelta();
            lookRotation.y = Mathf.Clamp(lookRotation.y, -90.0f, 90.0f);
            base.Update();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            fpCamera.m_Lens.FieldOfView = FOV;
            foreach (var ro in viewmodelObjects)
            {
                ro.settings.cameraSettings.cameraFieldOfView = ViewmodelFOV;
            }

            FOV = defaultFOV;
        }

        private Vector2 GetLookDelta()
        {
            Vector2 input = Vector2.zero;

            if (lookDelta != null) input += lookDelta.ReadValue<Vector2>() * lookDeltaSensitivity;
            if (lookAdditive != null) input += lookAdditive.ReadValue<Vector2>() * lookAdditiveSensitivity * Time.deltaTime;

            return input;
        }
    }
}
