using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System.Collections.Generic;
using System;

[ExecuteInEditMode()]
#endif
[System.Serializable]
public class renderSorting : MonoBehaviour
{
    [SerializeField]
    public string m_sortingLayerName;
    [SerializeField]
    public int m_id;
    [SerializeField]
    public int m_sortingOrder;
#if UNITY_EDITOR
    public void OnEnable()
    {
        if (renderer != null && this != null)
        {
            this.applyLayerSetting();

            EditorApplication.update += CheckLayer;
        }
    }

    public void OnDestroy()
    {
        //Debug.Log("DestroyedRenderSorting");
        EditorApplication.update -= CheckLayer;
    }
    //--------------------------------------------------------------
    public void applyLayerSetting()
    {
        if (renderer != null && this != null)
        {
            List<string> names = new List<string>(GetSortingLayerNames());
            m_id = Array.IndexOf(names.ToArray(), m_sortingLayerName);
            if (m_id >= 0)
            {
                renderer.sortingLayerID = m_id;
                renderer.sortingOrder = m_sortingOrder;
            }
        }
    }

    //--------------------------------------------------------------
    public void CheckLayer()
    {
        if (renderer != null && this!=null)
        {
            this.applyLayerSetting();
        }
    }


    // Get the sorting layer names
    public string[] GetSortingLayerNames()
    {
        System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    // Get the unique sorting layer IDs -- tossed this in for good measure
    public int[] GetSortingLayerUniqueIDs()
    {
        System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    }
#endif
}