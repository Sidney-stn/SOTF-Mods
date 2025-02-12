
using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal class WirelessTransmitterSwitch : PrefabBase
    {
        internal GameObject gameObjectWithComps;
        public Dictionary<string, GameObject> spawnedGameObjects = new Dictionary<string, GameObject>();  // UniqueId, GameObject

        internal override void ConfigureComponents(GameObject obj)
        {
            throw new NotImplementedException();
        }

        internal override void Spawn(Vector3 pos, Quaternion rot)
        {
            throw new NotImplementedException();
        }

        //public void SetupPrefab()
        //{
        //    throw new NotImplementedException();
        //}

        //public GameObject Spawn()
        //{
        //    throw new NotImplementedException();
        //}

        //public bool DoesUniqueIdExist(string uniqueId)
        //{
        //    if (spawnedGameObjects.ContainsKey(uniqueId))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        Misc.Msg($"GameObject with unique ID {uniqueId} not found.");
        //        return false;
        //    }
        //}

        //public GameObject FindByUniqueId(string uniqueId)
        //{
        //    if (spawnedGameObjects.TryGetValue(uniqueId, out GameObject sign))
        //    {
        //        return sign;
        //    }
        //    else
        //    {
        //        Misc.Msg($"GameObject with unique ID {uniqueId} not found.");
        //        return null;
        //    }
        //}
    }
}
