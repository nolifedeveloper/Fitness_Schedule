using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;

public class FitnessScheduler : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite armImg;
    public Sprite chestImg;
    public Sprite legsImg;
    public Sprite absImg;
    public Sprite ultimateImg;

    [Header("Object References")]
    public GameObject daysParent;
    public GameObject dayHolderPrefab;
    public TextMeshProUGUI YEAR_TEXT;
    public TextMeshProUGUI MONTH_TEXT;

    [Header("Settings")]
    public Month[] allMonths;

    private int currentYear = 2024;
    private int currentMonthIndex = 0;

    public static FitnessScheduler instance;

    //Save stuff


    [HideInInspector]public List<WorkoutData> allWorkoutData = new List<WorkoutData>();

    private SavedDate lastSavedDate = new SavedDate();


    private string pathToSaveData => Application.persistentDataPath;
    private string workoutDataPath => $"{pathToSaveData}/wrktdt.nld";
    private string settingsDataPath => $"{pathToSaveData}/lst.nld";


    private void Awake()
    {
        instance = this;

        lastSavedDate = GetData(lastSavedDate, settingsDataPath);
        allWorkoutData = GetData(allWorkoutData, workoutDataPath);

        currentMonthIndex = lastSavedDate.lastMonth;
        currentYear = lastSavedDate.lastYear;
        ChangeMonth(0);
        ChangeYear(0);
    }

    private void Start()
    {
        InitializeMonth();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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

            WorkoutData savedData = ReturnSavedDay(data);

            if(savedData != null)
            {
                data.workoutType = savedData.workoutType;
                instantiatedDay.GetComponent<DayHolder>().dayImage.sprite = ReturnWorkoutSprite(data.workoutType);
            }
        }
    }

    public void ChangeYear(int delta)
    {
        currentYear += delta;
        YEAR_TEXT.text = currentYear.ToString();
        lastSavedDate.lastYear = currentYear;
        InitializeMonth();
        SetData(lastSavedDate, settingsDataPath);
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
        lastSavedDate.lastMonth = currentMonthIndex;

        SetData(lastSavedDate, settingsDataPath);
        InitializeMonth();
    }

    private void ClearMonth()
    {
        for(int i = 0; i < daysParent.transform.childCount; i++)
        {
            Destroy(daysParent.transform.GetChild(i).gameObject);
        }
    }


    public void AddWorkoutData(WorkoutData data)
    {
        allWorkoutData.Add(data);
        SetData(allWorkoutData, workoutDataPath);
    }

    public void RemoveWorkoutData(WorkoutData data)
    {
        WorkoutData toRemove = ReturnSavedDay(data);

        if(toRemove != null)
        {
            allWorkoutData.Remove(toRemove);
            Debug.Log("Removed");
        }

        SetData(allWorkoutData, workoutDataPath);
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
            case WorkoutType.Ultimate:
                return ultimateImg;
            default:
                return null;
        }
    }

    private T GetData<T>(T obj, string targetPath) where T : class
    {
        if (!Directory.Exists(pathToSaveData)) { Directory.CreateDirectory(pathToSaveData); }

        if (!File.Exists(targetPath))
        {
            File.WriteAllText(targetPath, JsonConvert.SerializeObject(obj, Formatting.Indented));
            return obj;
        }
        else
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(targetPath));
        }
    }

    public void SetData(object obj, string targetPath)
    {
        if (!Directory.Exists(pathToSaveData)) { Directory.CreateDirectory(pathToSaveData); }

        if (File.Exists(targetPath))
        {
            File.WriteAllText(targetPath, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }

    private WorkoutData ReturnSavedDay(WorkoutData checkedData)
    {
        foreach(WorkoutData data in allWorkoutData)
        {
            if(data.day == checkedData.day && data.month == checkedData.month && data.year == checkedData.year)
            {
                return data;
            }
        }

        return null;
    }
}

[System.Serializable]
public enum WorkoutType
{
    None,
    Arms,
    Chest,
    Legs,
    Abs,
    Ultimate
}

[System.Serializable]
public class WorkoutData
{
    public int day;
    public int month;
    public int year;
    public WorkoutType workoutType = WorkoutType.None;
}

[System.Serializable]
public class Month
{
    public int dayCount = 30;
    public string monthName = "January";
}

[System.Serializable]
public class SavedDate
{
    public int lastYear = 2024;
    public int lastMonth = 0;
}