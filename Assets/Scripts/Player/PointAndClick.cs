﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndClick : MonoBehaviour
{
    public LayerMask InteractableMask;

    public StringReference CurrentCamera;

    public FloatReference PlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if(Input.GetButtonDown("Fire1") && CursorManager.Instance.cursorState != CursorManager.CursorStates.deactivated)
        {
            // Fire ray
            GameObject camGO = GameObject.Find(CurrentCamera.Value);
            Camera cam = camGO.GetComponentInChildren<Camera>();

            if(cam == null || camGO == null)
            {
                Debug.LogError("PointAndClick::Update() -- Camera not found on " + CurrentCamera);
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, InteractableMask))
            {
                PlayerHealth.Value -= 5;

                Debug.LogWarning(hit.transform.name);
                if(hit.transform.root.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    Interactable i = hit.transform.GetComponent<Interactable>();
                    if (i.MustMoveToBeforeInteraction)
                    {
                        Debug.Log("Move to");
                        if(i.MoveToLoc != null)
                            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MoveToInteract(i.MoveToLoc.position, i);
                        else
                        {
                            Debug.LogWarning("Move to loc not found for interactable. Defaulting to basic activate.");
                            i.Activate();
                        }
                    }
                    else
                    {
                        Debug.Log("Interact");
                        i.Activate();
                    }
                }
            }
        }

        // Passive Raycast
        if (CursorManager.Instance.cursorState != CursorManager.CursorStates.deactivated)
        {
            // Fire passive ray
            GameObject camGO = GameObject.Find(CurrentCamera.Value);
            Camera cam = camGO.GetComponentInChildren<Camera>();

            if (cam == null || camGO == null)
            {
                Debug.LogError("PointAndClick::Update() -- Camera not found on " + CurrentCamera);
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, InteractableMask))
            {
                if (hit.transform.root.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    CursorManager.Instance.SetCurstor(CursorManager.CursorStates.interactable);
                    DisplayInteractionText.Instance.SetText(hit.transform.gameObject.GetComponent<Interactable>().Name);
                }
                else
                {
                    CursorManager.Instance.SetCurstor(CursorManager.CursorStates.normal);
                    DisplayInteractionText.Instance.SetText("");
                }
            }
        }        
    }
}
