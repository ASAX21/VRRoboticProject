using UnityEngine;

public class Wall : MonoBehaviour {

    public Material wallMat;
    public Material deleteMat;

    public Renderer rend;

    private void OnMouseEnter()
    {
        if (ObjectManager.instance.removingWalls)
        {
            rend.material = deleteMat;
            ObjectManager.instance.cancelRemoveEvent = SetMaterialToNormal;
        }
    }

    private void OnMouseExit()
    {
        if (ObjectManager.instance.removingWalls)
        {
            rend.material = wallMat;
        }
    }

    private void OnMouseDown()
    {
        if (ObjectManager.instance.removingWalls)
        { 
            ObjectManager.instance.cancelRemoveEvent = null;
            Destroy(gameObject);  
        }
    }

    // Triggered when delete wall is cancelled
    public void SetMaterialToNormal()
    {
        rend.material = wallMat;
    }

}
