using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        Demo_Preset,
        Loading,
        Demo_Clip
    }

    private static Scene targetScene;

    public static void Load(Scene target)
    {
        targetScene = target;
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void OnLoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
