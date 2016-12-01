using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CrackerBarrel
{
    public class InputManager : MonoBehaviour
    {
        public event Action<bool, GameObject> OnHighlightObject;
        public event Action<GameObject> OnActivateObject;

        public GameObject CurrentHighlightObject { get; set; }
        public GameObject CurrentActivateObject { get; set; }
        
        void Update()
        {
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
                        OnHighlightObject?.Invoke(false, CurrentHighlightObject);
                    
                    CurrentHighlightObject = newMouseOverObject;

                    OnHighlightObject?.Invoke(true, CurrentHighlightObject);
                }

                // Check for mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    
                    CurrentActivateObject = newMouseOverObject;
                    OnActivateObject.Invoke(newMouseOverObject);
                }
            }
            else
            {
                // If mouse clicked on something other than a game piece, make sure we clear the previous select
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentActivateObject = null;
                    OnActivateObject?.Invoke(null);
                }

                // If mouse hover over something other than a game piece, make sure we clear the previous highlight
                if (CurrentHighlightObject != null)
                {
                    OnHighlightObject?.Invoke(false, CurrentHighlightObject);
                    CurrentHighlightObject = null;
                }
            }
        }

    }
}