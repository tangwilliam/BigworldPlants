using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class GPUInstancingManager : MonoBehaviour
{
    //-------------------------------------------------
    // Singleton
    public static GPUInstancingManager instance = null;
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);    // Ensures that there aren't multiple Singletons

        instance = this;
    }

    // Singleton
    //-------------------------------------------------


    CullingGroup m_CullingGroup;

    GameObject[] m_PlantGOs;
    BoundingSphere[] m_BoundingSpheres;


    private void Start()
    {
        m_CullingGroup = new CullingGroup();
        m_CullingGroup.targetCamera = Camera.main;

        //m_CullingGroup.SetDistanceReferencePoint(Camera.main.transform);
        //m_CullingGroup.SetBoundingDistances(new float[] { 1, 5, 10, 30, 100 });

        m_CullingGroup.onStateChanged = StateChangedMethod;

        SetupCullingData();

    }

    private void Update()
    {

        // Allocate an array to hold the resulting sphere indices - the size of the array determines the maximum spheres checked per call
        //int[] resultIndices = new int[30];
        //// Also set up an int for storing the actual number of results that have been placed into the array
        //int numResults = 0;

        //// Find all spheres that are visible
        //numResults = m_CullingGroup.QueryIndices(true, resultIndices, 0);

        //PrintArray(ref resultIndices);

        //// Find all spheres that are in distance band 1
        //numResults = m_CullingGroup.QueryIndices(1, resultIndices, 0);
        //// Find all spheres that are hidden in distance band 2, skipping the first 100
        //numResults = m_CullingGroup.QueryIndices(false, 2, resultIndices, 100);


    }

    private void PrintArray( ref int[] array)
    {
        StringBuilder stringBuilder = new StringBuilder("array contents: ", 200);
        if (array == null) return;
        for(int i = 0; i < array.Length; i++)
        {
            stringBuilder.AppendFormat(" {0} ", array[i]);
        }
        Debug.Log(stringBuilder.ToString());
    }

    

    private void StateChangedMethod(CullingGroupEvent evt)
    {
        m_PlantGOs[evt.index].SetActive(evt.isVisible);
        Debug.LogFormat("Sphere {0} has changed visible state to {1}!", evt.index, evt.isVisible.ToString());

    }


    private void SetupCullingData()
    {
        m_BoundingSpheres = new BoundingSphere[1000];

        GameObject rootGO = GameObject.Find("Grass");
        int childrenCount = rootGO.transform.childCount;
        
        m_PlantGOs = new GameObject[childrenCount];
        for (int i = 0; i < childrenCount; ++i)
        {
            m_PlantGOs[i] = rootGO.transform.GetChild(i).gameObject;
            m_BoundingSpheres[i] = new BoundingSphere(m_PlantGOs[i].transform.position, 1.0f);
        }
            
        m_CullingGroup.SetBoundingSpheres(m_BoundingSpheres);
        m_CullingGroup.SetBoundingSphereCount(m_PlantGOs.Length);
    }


    private void OnDestroy()
    {
        m_CullingGroup.onStateChanged -= StateChangedMethod;
        m_CullingGroup.Dispose();
        m_CullingGroup = null;
    }
}
