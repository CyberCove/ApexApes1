using UnityEngine;
using TMPro;
using System;

public class CountdownTimer : MonoBehaviour
{
    [Header("Text Display")]
    public TMP_Text countdownText;

    [Header("Target Date")]
    public int year = 2026;
    [Range(1, 12)] public int month = 1;
    [Range(1, 31)] public int day = 1;

    [Header("Target Time")]
    [Range(0, 23)] public int hour = 0;
    [Range(0, 59)] public int minute = 0;
    [Range(0, 59)] public int second = 0;

    private DateTime targetTime;

    void Start()
    {
        try
        {
            targetTime = new DateTime(year, month, day, hour, minute, second);
        }
        catch
        {
            Debug.LogError("Invalid Date Selected!");
        }
    }

    void Update()
    {
        if (countdownText == null) return;

        TimeSpan timeRemaining = targetTime - DateTime.Now;

        if (timeRemaining.TotalSeconds <= 0)
        {
            countdownText.text = "Leaving Now!";
            return;
        }

        int days = timeRemaining.Days;
        int hours = timeRemaining.Hours;
        int minutes = timeRemaining.Minutes;

        string dayText = days == 1 ? "Day" : "Days";
        string hourText = hours == 1 ? "Hour" : "Hours";
        string minuteText = minutes == 1 ? "Minute" : "Minutes";

        countdownText.text =
            $"Leaving In {days} {dayText} {hours} {hourText} And {minutes} {minuteText}";
    }
}