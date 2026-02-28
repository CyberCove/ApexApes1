using UnityEngine;

[ExecuteInEditMode] // This makes the change happen instantly in the scene view
public class MaterialMapper : MonoBehaviour
{
    [Header("Assign Materials Here")]
    public Material bodyFur;    // Element 0
    public Material chestFur;   // Element 1
    public Material faceTexture; // Element 2

    void Start()
    {
        ApplyMaterials();
    }

    // Using OnValidate makes it update the moment you drag a material into the slot
    void OnValidate()
    {
        ApplyMaterials();
    }

    void ApplyMaterials()
    {
        Renderer rend = GetComponent<Renderer>();

        if (rend != null)
        {
            Material[] newMats = new Material[3];

            newMats[0] = bodyFur;     
            newMats[1] = chestFur;    
            newMats[2] = faceTexture; 

            rend.sharedMaterials = newMats;
        }
    }
} // <--- This was the bracket you were missing!