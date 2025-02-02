using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI.Imageを使用するために追加
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))] // GUITextureからImageに変更
public class ForcedReset : MonoBehaviour
{
    private void Update()
    {
        // if we have forced a reset ...
        if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
        {
            //... reload the scene
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).path);
        }
    }
}