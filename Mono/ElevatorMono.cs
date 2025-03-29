
using RedLoader;
using RedLoader.Unity.IL2CPP.Utils.Collections;
using Sons.Gui.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleElevator.Mono
{
    internal class ElevatorMono : MonoBehaviour
    {
        public bool isSetupPrefab = false;
        public GameObject MoveGo { get; private set; }
        public Text MoveText { get; private set; }
        public GameObject UPGo { get; private set; }
        public Text UPText { get; private set; }
        public GameObject DOWNGo { get; private set; }
        public Text DOWNText { get; private set; }
        public GameObject ErrorTextGo { get; private set; }
        public Text ErrorText { get; private set; }

        private string _upOrDown = "";
        public string UpOrDown
        {
            get
            {
                return _upOrDown;
            }
            set
            {
                _upOrDown = value;
                if (value == "UP" || value == "DOWN")
                {
                    if (UPGo != null) { UPGo.SetActive(value == "UP"); }
                    if (DOWNGo != null) { DOWNGo.SetActive(value == "DOWN"); }
                }
                else if (value == "NONE")
                {
                    if (UPGo != null) { UPGo.SetActive(false); }
                    if (DOWNGo != null) { DOWNGo.SetActive(false); }
                }
            }
        }

        private void Awake()
        {
            if (isSetupPrefab) { return; }
            if (transform.position == Vector3.zero) { isSetupPrefab = true; return; }
            if (MoveGo == null)
            {
                MoveGo = gameObject.transform.GetChild(3).FindChild("MoveGo").gameObject;
                if (MoveGo != null)
                {
                    MoveText = MoveGo.GetComponent<Text>();
                    MoveGo.SetActive(true);
                }
            }
            if (UPGo == null)
            {
                UPGo = gameObject.transform.GetChild(3).FindChild("UP").gameObject;
                if (UPGo != null)
                {
                    UPText = UPGo.GetComponent<Text>();
                    UPGo.SetActive(false);
                }
            }
            if (DOWNGo == null)
            {
                DOWNGo = gameObject.transform.GetChild(3).FindChild("DOWN").gameObject;
                if (DOWNGo != null)
                {
                    DOWNText = DOWNGo.GetComponent<Text>();
                    DOWNGo.SetActive(false);
                }
            }
            if (ErrorTextGo == null)
            {
                ErrorTextGo = gameObject.transform.GetChild(3).FindChild("Error").gameObject;
                if (ErrorTextGo != null)
                {
                    ErrorText = ErrorTextGo.GetComponent<Text>();
                    ErrorTextGo.SetActive(false);  // Hide By Default
                }
            }
        }

        public LinkUiElement LinkUi { get; private set; }


        private void Start()
        {
            if (isSetupPrefab) { return; }
            if (LinkUi == null)
            {
                LinkUi = Tools.LinkUi.CreateLinkUi(gameObject.transform.GetChild(4).gameObject, 2f, null, Assets.Instance.LinkUiIcon, null);
            }
            Destroy(GetComponent<Collider>());  // I Removed Here since it get added from something i dont have in unity
            Objects.Track.Elevators.Add(gameObject);
        }



        public void ShowError(string error, float seconds = 3f)
        {
            if (ErrorTextGo == null) { return; }
            if (ErrorText == null) { return; }
            showError(error, seconds);
        }

        private IEnumerator showError(string error, float seconds)
        {
            if (ErrorTextGo == null) { yield break; }
            if (ErrorText == null) { yield break; }
            ErrorText.text = error;
            if (MoveGo != null) { MoveGo.SetActive(false); }
            if (UPGo != null) { UPGo.SetActive(false); }
            if (DOWNGo != null) { DOWNGo.SetActive(false); }
            UpOrDown = "NONE";
            ErrorTextGo.SetActive(true);
            yield return new WaitForSeconds(seconds);
            ErrorTextGo.SetActive(false);
            if (MoveGo != null) { MoveGo.SetActive(true); }
        }

        // Debug visualization methods
        private IEnumerator VisualizeBoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, float maxDistance, Color color, float duration)
        {
            GameObject cubeOrigin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeOrigin.transform.position = center;
            cubeOrigin.transform.localScale = halfExtents * 2; // Convert half extents to full size
            cubeOrigin.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.3f);
            cubeOrigin.GetComponent<Collider>().enabled = false;

            GameObject cubeEnd = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeEnd.transform.position = center + (direction.normalized * maxDistance);
            cubeEnd.transform.localScale = halfExtents * 2;
            cubeEnd.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.3f);
            cubeEnd.GetComponent<Collider>().enabled = false;

            GameObject lineObj = new GameObject("DebugLine");
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = 2;
            line.SetPosition(0, center);
            line.SetPosition(1, center + (direction.normalized * maxDistance));
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = color;
            line.endColor = color;

            Misc.Msg($"[VisualizeBoxCast] Drawing box cast from {center} in direction {direction} for {maxDistance} units", true);

            yield return new WaitForSeconds(duration);

            Destroy(cubeOrigin);
            Destroy(cubeEnd);
            Destroy(lineObj);
        }

        private IEnumerator VisualizeHitPoint(Vector3 hitPoint, Color color, float duration)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = hitPoint;
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sphere.GetComponent<Renderer>().material.color = color;
            sphere.GetComponent<Collider>().enabled = false;

            Misc.Msg($"[VisualizeHitPoint] Drawing hit point at {hitPoint}", true);

            yield return new WaitForSeconds(duration);

            Destroy(sphere);
        }
        private float elevatorSpeed = 5f;
        private bool isMoving = false;
        private Vector3 targetPosition;
        private GameObject targetControlPanel;

        public void MoveUp(bool raiseNetwork = true)
        {
            if (isMoving)
            {
                ShowError("Elevator\nAlready\nMoving");
                return;
            }

            // If we're a client, just send the request to the server and return
            if (raiseNetwork && BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                BoltEntity entity = GetComponent<BoltEntity>();
                if (entity != null)
                {
                    if (MoveText != null) { MoveText.text = "Requesting\nMove Up"; }
                    Network.ElevatorSyncEvent.SendState(entity, Network.ElevatorSyncEvent.ElevatorSyncType.MoveUp);
                }
                return;
            }

            // Only server or single player proceeds with the actual movement

            // Check if there's a control panel above
            Vector3 boxCastCenter = transform.position;
            Vector3 boxCastHalfExtents = new Vector3(2f, 2f, 2f);
            float maxDistance = 20f;

            RaycastHit[] hits = Physics.BoxCastAll(
                boxCastCenter,
                boxCastHalfExtents,
                Vector3.up,
                Quaternion.identity,
                maxDistance
            );

            // Visualize the raycast if debug setting is enabled
            if (Settings.showRaycastElevator)
            {
                StartCoroutine(VisualizeBoxCast(boxCastCenter, boxCastHalfExtents, Vector3.up, maxDistance, Color.green, 5f).WrapToIl2Cpp());
            }

            GameObject closestControlPanel = null;
            float closestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.name.Contains("EControlPanel"))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance && hit.transform.position.y > transform.position.y)
                    {
                        closestDistance = distance;
                        closestControlPanel = hit.transform.gameObject;

                        // Visualize the hit if debug setting is enabled
                        if (Settings.showRaycastElevator)
                        {
                            StartCoroutine(VisualizeHitPoint(hit.transform.position, Color.red, 5f).WrapToIl2Cpp());
                        }
                    }
                }
            }

            if (closestControlPanel == null)
            {
                ShowError("No Control\nPanel Found\nAbove");
                return;
            }

            targetControlPanel = closestControlPanel;
            targetPosition = new Vector3(
                transform.position.x,
                targetControlPanel.transform.position.y - 1f,
                transform.position.z
            );

            isMoving = true;
            StartCoroutine(MoveElevator().WrapToIl2Cpp());

            // If we're the server, notify all clients about the movement
            if (raiseNetwork && BoltNetwork.isRunning && BoltNetwork.isServer)
            {
                BoltEntity entity = GetComponent<BoltEntity>();
                if (entity != null)
                {
                    Network.ElevatorSyncEvent.SendState(entity, Network.ElevatorSyncEvent.ElevatorSyncType.MoveUp);
                }
            }
        }

        public void MoveDown(bool raiseNetwork = true)
        {
            if (isMoving)
            {
                ShowError("Elevator\nAlready\nMoving");
                return;
            }

            // If we're a client, just send the request to the server and return
            if (raiseNetwork && BoltNetwork.isRunning && BoltNetwork.isClient)
            {
                BoltEntity entity = GetComponent<BoltEntity>();
                if (entity != null)
                {
                    if (MoveText != null) { MoveText.text = "Requesting\nMove Down"; }
                    Network.ElevatorSyncEvent.SendState(entity, Network.ElevatorSyncEvent.ElevatorSyncType.MoveDown);
                }
                return;
            }

            // Only server or single player proceeds with the actual movement

            // Check if there's a control panel below
            Vector3 boxCastCenter = transform.position;
            Vector3 boxCastHalfExtents = new Vector3(2f, 2f, 2f);
            float maxDistance = 20f;

            RaycastHit[] hits = Physics.BoxCastAll(
                boxCastCenter,
                boxCastHalfExtents,
                Vector3.down,
                Quaternion.identity,
                maxDistance
            );

            // Visualize the raycast if debug setting is enabled
            if (Settings.showRaycastElevator)
            {
                StartCoroutine(VisualizeBoxCast(boxCastCenter, boxCastHalfExtents, Vector3.down, maxDistance, Color.blue, 5f).WrapToIl2Cpp());
            }

            GameObject closestControlPanel = null;
            float closestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.name.Contains("EControlPanel"))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance && hit.transform.position.y < transform.position.y)
                    {
                        closestDistance = distance;
                        closestControlPanel = hit.transform.gameObject;

                        // Visualize the hit if debug setting is enabled
                        if (Settings.showRaycastElevator)
                        {
                            StartCoroutine(VisualizeHitPoint(hit.transform.position, Color.red, 5f).WrapToIl2Cpp());
                        }
                    }
                }
            }

            if (closestControlPanel == null)
            {
                ShowError("No Control\nPanel Found\nBelow");
                return;
            }

            targetControlPanel = closestControlPanel;
            targetPosition = new Vector3(
                transform.position.x,
                targetControlPanel.transform.position.y + 1f,
                transform.position.z
            );

            isMoving = true;
            StartCoroutine(MoveElevator().WrapToIl2Cpp());

            // If we're the server, notify all clients about the movement
            if (raiseNetwork && BoltNetwork.isRunning && BoltNetwork.isServer)
            {
                BoltEntity entity = GetComponent<BoltEntity>();
                if (entity != null)
                {
                    Network.ElevatorSyncEvent.SendState(entity, Network.ElevatorSyncEvent.ElevatorSyncType.MoveDown);
                }
            }
        }

        // Method for clients to update their local state without sending network events
        public void ClientSetTarget(GameObject controlPanel, Vector3 target)
        {
            if (isMoving)
            {
                // Don't interrupt an already moving elevator
                return;
            }

            targetControlPanel = controlPanel;
            targetPosition = target;
            isMoving = true;
            StartCoroutine(MoveElevator().WrapToIl2Cpp());
        }

        private IEnumerator MoveElevator()
        {
            // Set the text accordingly
            if (targetPosition.y > transform.position.y)
            {
                if (MoveText != null) { MoveText.text = "Moving Up"; }
            }
            else
            {
                if (MoveText != null) { MoveText.text = "Moving Down"; }
            }

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    elevatorSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Ensure exact position
            transform.position = targetPosition;
            isMoving = false;

            // Reset the text
            if (MoveText != null) { MoveText.text = "Select Direction"; }
            UpOrDown = "NONE";
        }

        public void InvokePrimaryAction()
        {
            if (UpOrDown == "UP")
            {
                MoveUp();
            }
            else if (UpOrDown == "DOWN")
            {
                MoveDown();
            }
            else
            {
                ShowError("No\nDirection\nSelected");
            }
        }

        public void OnScrollUp() { UpOrDown = "UP"; }
        public void OnScrollDown() { UpOrDown = "DOWN"; }

        private void OnDestroy()
        {
            Objects.Track.Elevators.Remove(gameObject);
        }
    }
}
