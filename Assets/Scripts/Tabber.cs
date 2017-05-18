using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tabber : MonoBehaviour
{

    EventSystem system;
    private void Start()
    {
        system = EventSystem.current;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                GameObject currentSelected = system.currentSelectedGameObject;
                if (currentSelected != null)
                    next = currentSelected.GetComponent<Selectable>().FindSelectableOnUp();

            }
            else
            {
                GameObject currentSelected = system.currentSelectedGameObject;
                if (currentSelected != null)
                    next = currentSelected.GetComponent<Selectable>().FindSelectableOnDown();

            }

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }

}
