using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    void Start()
    {
        var ss = new GameObject().GetComponent<SpriteRenderer>().sprite;
        
        
        
        
        var btn = transform.FindChild("Button").GetComponent<Button>();
        var lbl = transform.FindChild("Button/Text").GetComponent<Text>();

        //自动添加EventTriggerListener
        //EventTriggerListener.GetListener(lbl.gameObject).onDrag = ActionDrag;
        //EventTriggerListener.GetListener(btn.gameObject).EventOnClick += ActionClick;
        //EventTriggerListener.GetListener(btn.gameObject).onClick = ActionClick;
        //3
        
        EventTriggerListener.SetListener(btn.gameObject, EventTriggerType.PointerClick, EntryClick);

        //需要EventTrigger
        //1
        //btn.GetComponent<Button>().onClick.AddListener(ActionClick);

        //2
        //var eventTrigger = btn.GetComponent<EventTrigger>();
        //var entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerClick;
        //entry.callback.AddListener(EntryClick);
        //eventTrigger.delegates.Add(entry);

    }

    private void EntryClick(PointerEventData eventData)
    {
        Debug.Log("EntryClick");
    }
    private void ActionClick(GameObject go)
    {
        Debug.Log("ActionClick");
    }
    private void ActionDrag(GameObject go, Vector2 delta)
    {
        Debug.Log("ActionDrag");
    }
}