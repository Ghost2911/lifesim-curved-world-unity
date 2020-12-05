using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string SceneName;
    public AnimationClip ZoomUp;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        Animation anim = Camera.main.GetComponent<Animation>();
        anim.clip = ZoomUp;
        anim.Play();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneName);
    }
   

}
