
using UnityEngine;

namespace SimpleElevator.Mono
{
    internal class ScrollMono : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                // Scroll up
                Objects.Actions.OnScrollUp();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                // Scroll down
                Objects.Actions.OnScrollDown();
            }
        }
    }
}
