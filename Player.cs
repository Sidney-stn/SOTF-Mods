
using TheForest.Utils;
using UnityEngine;

namespace Warps
{
    internal class Player
    {
        internal static void MoveLocalPlayer(Vector3 teleportPos)
        {
            LocalPlayer.Transform.position = teleportPos;
        }
    }
}
