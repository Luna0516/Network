using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    public System.Action<PointerEventData> OnPointerClickHandler = null;
    public System.Action<PointerEventData> OnDragHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnPointerClickHandler != null)
            OnPointerClickHandler.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(OnDragHandler != null) 
            OnDragHandler.Invoke(eventData);
    }
}
