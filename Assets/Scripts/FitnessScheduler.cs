using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FitnessScheduler : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite armImg;
    public Sprite chestImg;
    public Sprite legsImg;
    public Sprite absImg;

    [Header("Object References")]
    public GameObject daysParent;
    public GameObject dayHolderPrefab;
    public TextMeshProUGUI YEAR_TEXT;
    public TextMeshProUGUI MONTH_TEXT;

    [Header("Settings")]
    public Month[] allMonths;

    private List<WorkoutData> allData;

    private int currentYear = 2024;
    private int currentMonthIndex = 0;

    public static FitnessScheduler instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeMonth();
    }

    private void InitializeMonth()
    {
        ClearMonth();

        for (int day = 1; day < allMonths[currentMonthIndex].dayCount + 1; day++)
        {
            GameObject instantiatedDay = Instantiate(dayHolderPrefab);

            instantiatedDay.transform.SetParent(daysParent.transform, false);

            WorkoutData data = instantiatedDay.GetComponent<DayHolder>().data;

            data.day = day;
            data.month = currentMonthIndex;
            data.year = currentYear;
        }
    }

    public void ChangeYear(int delta)
    {
        currentYear += delta;
        YEAR_TEXT.text = currentYear.ToString();
    }

    public void ChangeMonth(int delta)
    {
        currentMonthIndex += delta;

        if(currentMonthIndex < 0) 
        { 
            currentMonthIndex = 11;
            ChangeYear(-1);
        }
        else if(currentMonthIndex > 11)
        {
            currentMonthIndex = 0;
            ChangeYear(1);
        }

        if(currentMonthIndex == 1)
        {
            allMonths[currentMonthIndex].dayCount = currentYear % 4 == 0 ? 29 : 28;
        }

        MONTH_TEXT.text = allMonths[currentMonthIndex].monthName.ToUpper();

        InitializeMonth();
    }

    private void ClearMonth()
    {
        for(int i = 0; i < daysParent.transform.childCount; i++)
        {
            Destroy(daysParent.transform.GetChild(i).gameObject);
        }
    }


    public Sprite ReturnWorkoutSprite(WorkoutType type)
    {
        switch (type)
        {
            case WorkoutType.None:
                return null;
            case WorkoutType.Arms:
                return armImg;
            case WorkoutType.Chest:
                return chestImg;
            case WorkoutType.Legs:
                return legsImg;
            case WorkoutType.Abs:
                return absImg;
            default:
                return null;
        }
    }
}

public enum WorkoutType
{
    None,
    Arms,
    Chest,
    Legs,
    Abs
}

[System.Serializable]
public class WorkoutData
{
    public int day;
    public int month;
    public int year;
    public WorkoutType workoutType = WorkoutType.None;
}

/*
public enum Month
{
    January = 31,
    February = 28,
    March = 31,
    April = 30,
    May = 31,
    June = 30,
    July = 31,
    August = 31,
    September = 30,
    October,
    November,
    December
}
*/
[System.Serializable]
public class Month
{
    public int dayCount = 30;
    public string monthName = "January";
}