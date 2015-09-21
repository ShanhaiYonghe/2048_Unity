using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventTriggerListener : EventTrigger
{
    public Dictionary<EventTriggerType, UnityAction<PointerEventData>> Listeners = new Dictionary<EventTriggerType, UnityAction<PointerEventData>>();

    public static EventTriggerListener GetListener(GameObject go)
    {
        var comp = go.GetComponent<EventTriggerListener>();
        if (comp == null)
        {
            comp = go.AddComponent<EventTriggerListener>();
        }
        return comp;
    }

    public static void SetListener(GameObject go, EventTriggerType eventTriggerType, UnityAction<PointerEventData> eventData)
    {
        GetListener(go).SetListener(eventTriggerType, eventData);
    }

    public void SetListener(EventTriggerType eventTriggerType, UnityAction<PointerEventData> callback)
    {
        Listeners[eventTriggerType] = callback;

        var entry = new EventTrigger.Entry();
        entry.eventID = eventTriggerType;
        entry.callback.AddListener(
            delegate(BaseEventData data)
            {
                UnityAction<PointerEventData> action = null;
                if (Listeners.TryGetValue(eventTriggerType, out action))
                    if (action != null)
                        action(data as PointerEventData);
            });

        if (delegates == null)
            delegates = new List<Entry>();
        delegates.Add(entry);
    }

    #region Old

    //public delegate void VoidDelegate(GameObject go);
    //public delegate void VectorDelegate(GameObject go, Vector2 delta);

    //public VoidDelegate onClick;
    //public VectorDelegate onDrag;

    //public event VoidDelegate EventOnClick
    //{
    //    add
    //    {
    //        EventTriggerListener.GetListener(gameObject).onClick += value;
    //    }
    //    remove
    //    {
    //        EventTriggerListener.GetListener(gameObject).onClick -= value;
    //    }
    //}

    //public override void OnPointerClick(PointerEventData eventData)
    //{
    //    if (onClick != null)
    //        onClick(gameObject);
    //}
    //public override void OnDrag(PointerEventData eventData)
    //{
    //    if (onDrag != null)
    //        onDrag(gameObject, eventData.delta);
    //}

    #endregion
}
