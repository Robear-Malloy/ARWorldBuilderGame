using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class day_night_cycle : MonoBehaviour
{
    public TextMeshProUGUI Daytime;
    Vector3 light_rotation = Vector3.zero;
    float speed = 12; // rotate 6 degrees every seconds. Full rotation(360 degrees) in one minute
    float currentRotation = 0f;
    int hourInt;

    // Update is called once per frame
    void Update()
    {
        light_rotation.x = speed * Time.deltaTime;
        transform.Rotate(light_rotation,Space.World);

        currentRotation += speed * Time.deltaTime;
        if (currentRotation >= 360f)
        {
            currentRotation -= 360f; // to reset
        }

        float hours = (currentRotation / 360f) * 24f;
        hourInt = Mathf.FloorToInt(hours);
        int minutes = Mathf.FloorToInt((hours - hourInt) * 60f);

        Daytime.text = string.Format("Time of the day: {0:00}:{1:00}", hourInt, minutes);
    }

    public int GetHours()
    {
        return hourInt;
    }
}
