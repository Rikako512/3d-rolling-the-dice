using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMeasurement : MonoBehaviour
{
    private float startTime;
    //private bool scriptExecuted = false;

    void Start()
    {
        // ゲーム起動時の時間を記録
        startTime = Time.time;
    }

    void Update()
    {
 /*
        // YourScriptが実行されていない場合
        if (!scriptExecuted)
        {
            // YourScriptが存在しているか確認し、実行された場合は時間を計測してログに表示
            YourScript yourScript = FindObjectOfType<YourScript>();
            if (yourScript != null && yourScript.HasFinishedExecution)
            {
                float elapsedTime = Time.time - startTime;
                Debug.Log("YourScriptの実行終了までの時間: " + elapsedTime + "秒");
                scriptExecuted = true;
            }
        }
        */
    }
}
