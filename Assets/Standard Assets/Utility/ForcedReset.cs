using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI.Image‚ğg—p‚·‚é‚½‚ß‚É’Ç‰Á
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))] // GUITexture‚©‚çImage‚É•ÏX
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