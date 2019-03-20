using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapper : MonoBehaviour {

    private Animator anim;
    private string sceneName;

    public void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void AnimateExit(string levelName)
    {
        sceneName = levelName;
        anim.Play("FadeOut");
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
