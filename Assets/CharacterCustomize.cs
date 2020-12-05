using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomize : MonoBehaviour
{
    public Material[] materials;
    public SkinnedMeshRenderer character;
    public Mesh[] meshes;

    string modelName;
    string materialName;
    int meshNum = 0;
    int color = 0;
    int skin = 0;

    private void Awake()
    {
        color = PlayerPrefs.GetInt("CharacterColor", 0);
        skin = PlayerPrefs.GetInt("CharacterSkin", 0);
    }

    public void SetSkin(int id)
    {
        skin = id;
        character.material = materials[color * 3 + skin];
    }

    public void SetColor(int id)
    {
        color = id;
        character.material = materials[color * 3 + skin];
    }

    public void SetMesh()
    {
        System.Random rnd = new System.Random();
        meshNum = rnd.Next(meshes.Length);
        character.sharedMesh = meshes[meshNum];
    }

    public void SaveCharacter()
    {
        PlayerPrefs.SetInt("CharacterSkin", skin);
        PlayerPrefs.SetInt("CharacterColor", color);

        PlayerPrefs.SetString("CharacterMesh", meshes[meshNum].name);
        PlayerPrefs.SetString("CharacterMaterial", materials[color * 3 + skin].name);

        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

}
