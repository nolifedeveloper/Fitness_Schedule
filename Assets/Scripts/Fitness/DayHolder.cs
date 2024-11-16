using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DayHolder : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public Image dayImage => transform.GetChild(0).GetComponent<Image>();
    [HideInInspector] public TextMeshProUGUI dayCount => transform.GetChild(2).GetComponent<TextMeshProUGUI>();

    public WorkoutData data;

    private void Start()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        dayCount.text = data.day.ToString("00");
        dayImage.sprite = FitnessScheduler.instance.ReturnWorkoutSprite(this.data.workoutType);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                ChangeDayStatus(ScrollType.Change);
                break;
            case PointerEventData.InputButton.Right:
                ChangeDayStatus(ScrollType.Empty);
                break;
            case PointerEventData.InputButton.Middle:
                Debug.Log("TODO. CHEST");
                break;
        }
    }

    private void ChangeDayStatus(ScrollType type)
    {
        FitnessScheduler.instance.RemoveWorkoutData(this.data);

        if (type == ScrollType.Change)
        {
            int workoutType = (int)this.data.workoutType + 1;
            
            if(workoutType > 5) { workoutType = 1; }
            this.data.workoutType = (WorkoutType)workoutType;
        }
        else
        {
            this.data.workoutType = WorkoutType.None;
        }

        dayImage.sprite = FitnessScheduler.instance.ReturnWorkoutSprite(this.data.workoutType);

        if(this.data.workoutType == 0) { return; }

        FitnessScheduler.instance.AddWorkoutData(this.data);
    }
}

public enum ScrollType
{
    Change,
    Empty
}
