using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuadcopterController : MonoBehaviour {



    [SerializeField] private new Transform camera;

    [SerializeField] private Settings settings;
    [Serializable]
    public class Settings {
        [Header("Properties")]
        public float powerForce;
        public float tiltForce;
        public float angularDrag;
        public float drag;

        [Header("Tuning")]
        public AnimationCurve powerCurve;
        public float powerSensitivity;
        public bool powerInversion;
        public AnimationCurve rollCurve;
        public float rollSensitivity;
        public bool rollInversion;
        public AnimationCurve yawCurve;
        public float yawSensitivity;
        public bool yawInversion;
        public AnimationCurve pitchCurve;
        public float pitchSensitivity;
        public bool pitchInversion;
    }

    private Rigidbody body;
    private PlayerInputActions playerInputActions;


    private void Awake() {
        body = GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        // Cursor.lockState = CursorLockMode.Locked;

        LoadSettings();
    }



    private void FixedUpdate() {
        HandleControl();
        HandleDrag();
    }

    private void HandleDrag() {
        body.AddTorque(-body.angularVelocity * body.angularVelocity.magnitude * settings.angularDrag * Time.fixedDeltaTime);

        body.AddForce(-body.velocity * body.velocity.magnitude * settings.drag * Time.fixedDeltaTime);
    }

    private void HandleControl() {
        float power = EvaluateInput(playerInputActions.Player.Power, settings.powerSensitivity, settings.powerCurve, settings.powerInversion);
        float roll = EvaluateInput(playerInputActions.Player.Roll, settings.rollSensitivity, settings.rollCurve, settings.rollInversion);
        float yaw = EvaluateInput(playerInputActions.Player.Yaw, settings.yawSensitivity, settings.yawCurve, settings.yawInversion);
        float pitch = EvaluateInput(playerInputActions.Player.Pitch, settings.pitchSensitivity, settings.pitchCurve, settings.pitchInversion);

        Vector3 torque =
            transform.right * pitch +
            camera.transform.up * yaw +
            transform.forward * roll;
        body.AddTorque(torque * settings.tiltForce * Time.fixedDeltaTime);

        body.AddForce(transform.up * power * settings.powerForce * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Use to Evaluate Inputs such as Power, Yaw, Roll, Pitch.
    /// </summary> 
    /// <remarks>
    ///    Example: playerInputActions.Player.Power, 0.5f, powerCurve, false
    /// </remarks>
    /// <returns>flaot in Range(-1, 1) </returns>
    private float EvaluateInput(InputAction inputAction, float sensitivity, AnimationCurve animationCurve, bool inversion) {
        float input = inputAction.ReadValue<float>();
        if(inversion) input = -input;
        float inputEvaluated = animationCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(input)));
        if(input < 0) inputEvaluated = -inputEvaluated;
        return Mathf.Clamp(inputEvaluated * sensitivity, -1, 1);
    }











    // Save, Load and Reset Quadcopter settings
    private string SAVE_FILE = Application.dataPath + "/QuadcopterControllerSaveSettings.txt";
    public void SaveSettings() {
        string json = JsonUtility.ToJson(settings);
        File.WriteAllText(SAVE_FILE, json);
    }
    public void LoadSettings() {
        string json = File.ReadAllText(SAVE_FILE);
        settings = JsonUtility.FromJson<Settings>(json);
    }
    public void ResetSettings() {
        settings.powerForce = 1f;
        settings.tiltForce = 1f;

        settings.powerSensitivity = 1f;
        settings.rollSensitivity = 1f;
        settings.yawSensitivity = 1f;
        settings.pitchSensitivity = 1f;
    }

}

