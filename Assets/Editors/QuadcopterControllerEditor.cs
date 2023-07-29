using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(QuadcopterController))]
public class QuadcopterControllerEditor : Editor {


    public override void OnInspectorGUI() {
        QuadcopterController quadcopterController = (QuadcopterController)target;

        if(GUILayout.Button("Save")) quadcopterController.SaveSettings();
        if(GUILayout.Button("Load")) quadcopterController.LoadSettings();
        if(GUILayout.Button("Reset")) quadcopterController.ResetSettings();


        base.OnInspectorGUI();
    }

}