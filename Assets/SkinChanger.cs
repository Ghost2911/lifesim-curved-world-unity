using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    SkinnedMeshRenderer mesh;

    void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();
        string savedMesh = PlayerPrefs.GetString("CharacterMesh","Chr_Fantasy_MalePeasant_01");
        GameObject meshes = Resources.Load("SkinMesh/Characters") as GameObject;

        foreach (SkinnedMeshRenderer m in meshes.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (m.name == savedMesh)
                mesh.sharedMesh = m.sharedMesh;
        }
        mesh.material = Resources.Load("Materials/" + PlayerPrefs.GetString("CharacterMaterial", "Material_Alt_A")) as Material;
    }
}
