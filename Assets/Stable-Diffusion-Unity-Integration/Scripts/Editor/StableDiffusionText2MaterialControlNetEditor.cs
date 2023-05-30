using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector to add a models list as a drop-down selection UI 
/// and add a "Generate" button for the StableDiffusionMaterial.
/// </summary>
[CustomEditor(typeof(StableDiffusionText2MaterialControlNet))]
public class StableDiffusionText2MaterialControlNetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StableDiffusionText2MaterialControlNet myComponent = (StableDiffusionText2MaterialControlNet)target;

        // Draw the drop-down list for the Samplers list
        myComponent.selectedSampler = EditorGUILayout.Popup("Sampler", myComponent.selectedSampler, myComponent.samplersList);

        // Draw the drop-down list for the Models list
        myComponent.selectedModel = EditorGUILayout.Popup("Model", myComponent.selectedModel, myComponent.modelsList);
        
        // Apply the changes to the serialized object
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate"))
            myComponent.Generate();
        if (GUILayout.Button("Clear state"))
            myComponent.generating = false;
    }
}
