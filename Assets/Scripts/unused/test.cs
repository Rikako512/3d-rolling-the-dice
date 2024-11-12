using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using TMPro;

public class test : MonoBehaviour, IPointerClickHandler
{
    private Vector3 targetPosition = new Vector3(1, 1, 1);
    private float movementSpeed = 2.0f; // í≤êÆâ¬î\Ç»à⁄ìÆë¨ìx

    private bool isMoving = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveToTargetPosition());
        }
    }

    private IEnumerator MoveToTargetPosition()
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (transform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * movementSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null;
        }

        isMoving = false;
    }
}
