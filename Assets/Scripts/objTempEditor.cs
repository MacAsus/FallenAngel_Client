using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(objTempScripts))]
public class objTempEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        objTempScripts myScript = (objTempScripts)target;


        if (GUILayout.Button("Change Position"))
        {
            myScript.ChangePostion();
        }


    }
}
