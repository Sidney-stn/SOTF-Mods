

using UnityEngine;

namespace WirelessSignals.Prefab
{
    internal interface IPrefabSetup
    {
        void SetupPrefab();
        GameObject Spawn();
        GameObject FindByUniqueId(string uniqueId);
        bool DoesUniqueIdExist(string uniqueId);
    }
}
