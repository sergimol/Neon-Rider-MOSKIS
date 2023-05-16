using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class Prueba : MonoBehaviour
{
    [SerializeField]
    GameObject o1;
    [SerializeField]
    GameObject o2;
    [SerializeField]
    Material mat;
    //void OnSceneGUI()
    //{
    //    Vector3 o1pos = o1.transform.position;
    //    Vector3 o2pos = o2.transform.position;
    //    Handles.DrawLine(o1pos, o2pos);
    //}
    Vector3 o1pos;
    Vector3 o2pos;
    private void Start()
    {
        o1pos = o1.transform.position;
        o2pos = o2.transform.position;
    }

    private void Update()
    {

    }

    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);

        Vector3 abobole = Camera.main.WorldToScreenPoint(o1pos);    
        Vector3 abobole2 = Camera.main.WorldToScreenPoint(o2pos);    
        GL.Vertex(abobole);
        GL.Vertex(abobole2);
        GL.End();

        GL.PopMatrix();
    }
}

