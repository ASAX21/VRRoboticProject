using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour {

    public delegate int TestFunc(int x);

    public TestFunc myDelegate;

    private void Start()
    {
        if(myDelegate == null)
        {
            Debug.Log("Delegate is initially null!");
        }
        myDelegate = functionOne;
        if (myDelegate == null)
        {
            Debug.Log("Delegate is null after 1!");
        }
        myDelegate -= functionOne;
        if (myDelegate == null)
        {
            Debug.Log("Delegate is null after 2!");
        }
    }


    public int functionOne(int x)
    {
        return 1;
    }

    public int functionTwo(int x)
    {
        return 2;
    }

    public int functionThree(int x)
    {
        return 3;
    }
}
