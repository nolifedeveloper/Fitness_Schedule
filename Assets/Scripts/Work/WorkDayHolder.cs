using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkDayHolder : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public Image dayImage => transform.GetChild(0).GetComponent<Image>();
    [HideInInspector] public TextMeshProUGUI dayCount => transform.GetChild(2).GetComponent<TextMeshProUGUI>();

    public WorkData data;

    private void Start()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        dayCount.text = data.day.ToString("00");
        dayImage.sprite = WorkScheduler.instance.ReturnWorkoutSprite(this.data.workoutType);
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
        WorkScheduler.instance.RemoveWorkoutData(this.data);

        if (type == ScrollType.Change)
        {
            int workoutType = (int)this.data.workoutType + 1;

            if (workoutType > 4) { workoutType = 1; }
            this.data.workoutType = (WorkType)workoutType;
        }
        else
        {
            this.data.workoutType = WorkType.None;
        }

        dayImage.sprite = WorkScheduler.instance.ReturnWorkoutSprite(this.data.workoutType);

        if (this.data.workoutType == 0) { return; }

        WorkScheduler.instance.AddWorkoutData(this.data);
    }
}
