using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMeasurement : MonoBehaviour
{
    private float startTime;
    //private bool scriptExecuted = false;

    void Start()
    {
        // �Q�[���N�����̎��Ԃ��L�^
        startTime = Time.time;
    }

    void Update()
    {
 /*
        // YourScript�����s����Ă��Ȃ��ꍇ
        if (!scriptExecuted)
        {
            // YourScript�����݂��Ă��邩�m�F���A���s���ꂽ�ꍇ�͎��Ԃ��v�����ă��O�ɕ\��
            YourScript yourScript = FindObjectOfType<YourScript>();
            if (yourScript != null && yourScript.HasFinishedExecution)
            {
                float elapsedTime = Time.time - startTime;
                Debug.Log("YourScript�̎��s�I���܂ł̎���: " + elapsedTime + "�b");
                scriptExecuted = true;
            }
        }
        */
    }
}
