using RedLoader;
using TheForest.Utils;
using UnityEngine;
using System.Collections;
using Signs.Network;
using SonsSdk;
using Sons.Items.Core;
using Bolt;
using Sons.Inventory;

namespace Signs.Mono
{
    //[RegisterTypeInIl2Cpp]
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.C) && distance < 1f)
            {
                Transform cameraTransform = LocalPlayer._instance._mainCam.transform;
                RaycastHit raycastHit;
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out raycastHit, 1.5f, LayerMask.GetMask(new string[] { "Default" })))
                {
                    shakeObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    if (shakeObject.name.Contains("Sign") || shakeObject.name.Contains("Sign(Clone)"))
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
                var boltEntity = shakeObject.GetComponent<BoltEntity>();
                if (boltEntity != null)
                {

                    //TheForest.Items.Utils.ItemUtils.SpawnItemPickup

                    //var config1 = new Construction.SpawnPickupConfig(gameObject.transform, ItemDatabaseManager.ItemById(576));
                    //Construction.ItemUtils.SpawnPickup(config1);
                    //var config2 = new Construction.SpawnPickupConfig(gameObject.transform, ItemDatabaseManager.ItemById(392));
                    //Construction.ItemUtils.SpawnPickup(config2);
                    //Construction.ItemUtils.SpawnPickup(config2);
                    //Construction.ItemUtils.SpawnPickup(config2);
                    //Construction.ItemUtils.SpawnPickup(config2);
                    //Construction.ItemUtils.SpawnPickup(config2);

                    //try
                    //{
                    //    TheForest.Items.Utils.ItemUtils.SpawnItemPickup(ItemDatabaseManager.ItemById(576), gameObject.transform.position, gameObject.transform.rotation, new Vector3(0, 0, 0), false, false, true, false, null, null, 1);
                    //    TheForest.Items.Utils.ItemUtils.SpawnItemPickup(ItemDatabaseManager.ItemById(392), gameObject.transform.position, gameObject.transform.rotation, new Vector3(0, 0, 0), false, true, true, false, null, null, 5);
                    //}
                    //catch (System.Exception e)
                    //{
                    //    Misc.Msg($"[DestroyOnCMono][OnShakeComplete()] Exception: {e}");
                    //}

                    try
                    {
                        DropItem dropItem = DropItem.Create(GlobalTargets.OnlyServer);
                        dropItem.PrefabId = ItemDatabaseManager.ItemById(576).PickupPrefab.gameObject.GetComponent<BoltEntity>().prefabId;
                        dropItem.ItemInstance = null;
                        dropItem.Position = gameObject.transform.position + (Vector3.up * 1);
                        dropItem.Rotation = gameObject.transform.rotation;
                        dropItem.PreSpawned = null;
                        dropItem.AvoidImpacts = false;
                        dropItem.Velocity = new Vector3(0, 0, 0);
                        dropItem.IsKinematic = false;
                        dropItem.ShouldDespawn = true;
                        dropItem.Send();

                        // Loop 5 times to drop sticks
                        for (int i = 0; i < 5; i++)
                        {
                            DropItem dropItem2 = DropItem.Create(GlobalTargets.OnlyServer);
                            dropItem2.PrefabId = ItemDatabaseManager.ItemById(392).PickupPrefab.gameObject.GetComponent<BoltEntity>().prefabId;
                            dropItem2.ItemInstance = null;
                            dropItem2.Position = gameObject.transform.position + (Vector3.up * 1);
                            dropItem2.Rotation = gameObject.transform.rotation;
                            dropItem2.PreSpawned = null;
                            dropItem2.AvoidImpacts = false;
                            dropItem2.Velocity = new Vector3(0, 0, 0);
                            dropItem2.IsKinematic = false;
                            dropItem2.ShouldDespawn = true;
                            dropItem2.Send();
                        }
                    }
                    catch (System.Exception e)
                    {
                        Misc.Msg($"[DestroyOnCMono][OnShakeComplete()] Exception: {e}");
                    }
                    SignSyncEvent.SendState(boltEntity, SignSyncEvent.SignSyncType.Destroy);
                    if (BoltNetwork.isRunning && BoltNetwork.isServer)
                    {
                        BoltNetwork.Destroy(gameObject);
                    }
                    else if (BoltNetwork.isRunning && BoltNetwork.isClient)
                    {
                        BoltNetwork.Destroy(gameObject);
                        SignSyncEvent.SendState(boltEntity, SignSyncEvent.SignSyncType.Destroy);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                    Misc.Msg($"[DestroyOnCMono][OnShakeComplete()] Destroyed: {boltEntity.name}");
                }
                else
                {
                    Misc.Msg($"[DestroyOnCMono][OnShakeComplete()] BoltEntity is null for: {shakeObject.name}");
                }
            }
        }

    }
}
