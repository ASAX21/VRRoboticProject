using UnityEngine;
using UnityEngine.EventSystems;

public class Wall : MonoBehaviour
{
    public Material wallMat;
    public Renderer rend;
    public Collider col;
    private Color color = Color.white;

    public bool isPlaced = true;
    int collisionCount = 0;

    private void OnMouseEnter()
    {
        if (ObjectManager.instance.removingWalls)
        {
            rend.material = ObjectManager.instance.invalidMat;
            ObjectManager.instance.cancelRemoveEvent = SetMaterialToNormal;
        }
    }

    private void OnMouseExit()
    {
        if(ObjectManager.instance.removingWalls)
            SetMaterialToNormal();
    }

    private void OnMouseDown()
    {
        if (ObjectManager.instance.removingWalls)
        { 
            ObjectManager.instance.cancelRemoveEvent = null;
            Destroy(gameObject);  
        }
    }

    private void OnMouseOver()
    { 
        if(ObjectManager.instance.paintingWalls && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0))
        {
            color = ObjectManager.instance.paintColor;
            rend.material.color = color;
        }
    }

    // Triggered when delete wall is cancelled
    public void SetMaterialToNormal()
    {
        rend.material = wallMat;
        rend.material.color = color;
    }

    // Check if placement is valid
    public void BeginPlacement()
    {
        isPlaced = false;
        gameObject.name = "wall";
        rend.material = ObjectManager.instance.validMat;
        col.isTrigger = true;
        collisionCount = 0;
    }

    public void FinishPlacement()
    {
        isPlaced = true;
        rend.material = wallMat;
        col.isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced && other.tag != "NonPhysical")
        {
            collisionCount++;
            rend.material = ObjectManager.instance.invalidMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced && other.tag != "NonPhysical")
        {
            if (--collisionCount == 0)
                rend.material = ObjectManager.instance.validMat;
            else if (collisionCount < 0)
            {
                collisionCount = 0;
                rend.material = ObjectManager.instance.validMat;
            }
        }
    }

    public bool CanPlace()
    {
        return collisionCount == 0;
    }
}
