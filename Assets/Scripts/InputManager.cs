﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CrackerBarrel
{
    public class InputManager : MonoBehaviour
    {
        public event Action<bool, GameObject> OnObjectHighlightChanged;
        public event Action<GameObject> OnSelectObject;

        public GameObject CurrentHighlightObject { get; set; }
        public GameObject CurrentSelectedObject { get; set; }

        public bool DisableInput { get; set; } = false;
        
        void Update()
        {
            if (DisableInput)
            {
                return;
            }

            // Bail out if mouse is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // TODO: Unhighlight here?? or keep highlight while over UI? Not sure yet.
                return;
            }

            // Raycast to find mouse hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hitInfos = new RaycastHit2D[1];

            // If mouse button clicks a game object
            if (Physics2D.RaycastNonAlloc(ray.origin, ray.direction, hitInfos) == 1)
            {
                var hitInfo = hitInfos[0];

                // Raise mouse-over events
                var newMouseOverObject = hitInfo.collider.gameObject;
                if (newMouseOverObject != CurrentHighlightObject)
                {
                    if (CurrentHighlightObject != null)
                        OnObjectHighlightChanged?.Invoke(false, CurrentHighlightObject);
                    
                    CurrentHighlightObject = newMouseOverObject;

                    OnObjectHighlightChanged?.Invoke(true, CurrentHighlightObject);
                }

                // Check for mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentSelectedObject = newMouseOverObject;
                    OnSelectObject.Invoke(newMouseOverObject);
                }
            }
            else
            {
                // If mouse clicked on something other than a game piece, make sure we clear the previous select
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentSelectedObject = null;
                    OnSelectObject?.Invoke(null);
                }

                // If mouse hover over something other than a game piece, make sure we clear the previous highlight
                if (CurrentHighlightObject != null)
                {
                    OnObjectHighlightChanged?.Invoke(false, CurrentHighlightObject);
                    CurrentHighlightObject = null;
                }
            }
        }

    }
}