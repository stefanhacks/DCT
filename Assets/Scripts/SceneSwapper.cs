using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that animates and interacts with UnityEngine's Scene Manager.
/// Relies in an Animator and a black sprite covering the play area.
/// </summary>
public class SceneSwapper : MonoBehaviour {

    private Animator anim;
    private string sceneName;

    public void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    /// <summary>
    /// Fires a transition effect from the current scene to the asked one,
    /// animating as a Fade Out effect.
    /// </summary>
    public void AnimateExit(string levelName)
    {
        sceneName = levelName;
        anim.Play("FadeOut");
    }

    /// <summary>
    /// Animation Event that triggers the Load Scene method. 
    /// Separated as it so it allows it's animation to fully trigger.
    /// </summary>
    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
