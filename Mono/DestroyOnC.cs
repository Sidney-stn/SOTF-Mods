using RedLoader;
using TheForest.Utils;
using UnityEngine;
using System.Collections;
using SonsSdk;

namespace Shops.Mono
{
    [RegisterTypeInIl2Cpp]
    public class DestroyOnC : MonoBehaviour
    {
        private Quaternion initialRotation;
        private bool isRotating = false;
        private Coroutines.CoroutineToken rotateCoroutine = null;
        private GameObject shakeObject = null;

        private void Start()
        {
            //if (Config.PrintMethodCallPrinting.Value) { Misc.Msg($"[DestroyOnCMono][Start()] initialRotation: {gameObject.transform.rotation}"); }
            initialRotation = gameObject.transform.rotation;
        }

        private void Update()
        {
            float distance = Vector3.Distance(LocalPlayer.Transform.position, gameObject.transform.position);
            if (UnityEngine.Input.GetKeyDown(KeyCode.C) && distance < 10f)
            {
                Transform cameraTransform = LocalPlayer._instance._mainCam.transform;
                RaycastHit raycastHit;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out raycastHit, 1.5f, LayerMask.GetMask(new string[] { "Default" })))
                {
                    shakeObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    if (shakeObject.name.Contains("Shop") || shakeObject.name.Contains("Shop(Clone)"))
                    {
                        GameObject topParent = raycastHit.collider.gameObject.transform.root.gameObject;
                        if (topParent != null)
                        {
                            shakeObject = topParent;

                        }
                    }

                    if (shakeObject != null)
                    {
                        if (shakeObject.name.Contains(gameObject.name))
                        {
                            Misc.Msg("Found Destroyable Object!");
                            if (!isRotating)
                            {
                                isRotating = true;
                                rotateCoroutine = ShakeObject().RunCoro();
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator ShakeObject(float shakeDuration = 2f, float shakeMagnitude = 2.5f)
        {
            float elapsedTime = 0f;
            if (shakeObject == null)
            {
                rotateCoroutine = null;
                yield break;
            }

            while (elapsedTime < shakeDuration)
            {
                if (!UnityEngine.Input.GetKey(KeyCode.C))
                {
                    shakeObject.transform.rotation = initialRotation;
                    //Misc.Msg($"[DestroyOnCMono][Start()] Rotation Set Back To: {shakeObject.transform.rotation}");
                    rotateCoroutine = null;
                    isRotating = false;
                    yield break;
                }

                float offsetX = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
                float offsetY = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
                float offsetZ = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
                shakeObject.transform.rotation = initialRotation * Quaternion.Euler(offsetX, offsetY, offsetZ);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            shakeObject.transform.rotation = initialRotation;
            //Misc.Msg($"[DestroyOnCMono][ShakeObject()] Rotation Set Back To: {shakeObject.transform.rotation}");
            isRotating = false;
            OnShakeComplete();
        }

        private void OnShakeComplete()
        {
            if (shakeObject.name.Contains(gameObject.name))
            {
                Shop mono = shakeObject.GetComponent<Shop>();
                if (mono != null)
                {
                    // Make So Only The Owner Can Delete The Shop
                    if (mono.OwnerId != Banking.API.GetLocalPlayerId()) 
                    {
                        Misc.Msg("You Are Not The Owner Of This Shop!");
                        SonsTools.ShowMessage("You Are Not The Owner Of This Shop!", 3f);
                        return; 
                    }
                    string uniqueId = mono.UniqueId;
                    if (uniqueId != null) { Prefab.SingleShop.spawnedShops.Remove(uniqueId); }
                    Saving.Load.ModdedShops.Remove(shakeObject);
                    Destroy(shakeObject);

                    SimpleNetworkEvents.EventDispatcher.RaiseEvent(new Network.RemoveShop
                    {
                        UniqueId = uniqueId
                    });
                }
                

            }
        }

    }
}
