using RedLoader;
using Sons.Gui.Input;
using UnityEngine;

namespace StoneGate.Mono
{
    internal class StoneGateStoreMono : MonoBehaviour
    {

        private GameObject _rotateGo;  // Rotate GameObject
        private Objects.CreateGateParent.RotateMode _rotateMode;  // Mode For Rotation of Rotate GameObject
        private GameObject _floorBeam;
        private GameObject _topBeam;
        private GameObject _rockWall;
        private GameObject _extraPillar;

        private HashSet<GameObject> objectsToRotate = new HashSet<GameObject>(new Objects.GameObjectInstanceIDComparer());
        private Dictionary<string, GameObject> namesOfAllGo = new Dictionary<string, GameObject>();

        public LinkUiElement LinkUiElement { get; private set; }

        public void Init(GameObject rotationGo,
            Objects.CreateGateParent.RotateMode mode,
            GameObject floorBeam = null,
            GameObject topBeam = null,
            GameObject rockWall = null,
            GameObject extraPillar = null
            )
        {
            if (rotationGo == null) { RLog.Error("[StoneGate] [StoneGateStoreMono] [Init] rotationGo is null"); Destroy(gameObject); }
            if (mode == Objects.CreateGateParent.RotateMode.None) { RLog.Error("[StoneGate] [StoneGateStoreMono] [Init] mode is None"); Destroy(gameObject); }
            if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] Rotation GameObject: {rotationGo.name} Mode: {mode}"); }
            if (floorBeam == null && topBeam == null && rockWall == null && extraPillar == null) { RLog.Error("[StoneGate] [StoneGateStoreMono] [Init] All GameObjects are null"); Destroy(gameObject); }
            _rotateGo = rotationGo;
            _rotateMode = mode;
            _floorBeam = floorBeam;
            _topBeam = topBeam;
            _rockWall = rockWall;
            _extraPillar = extraPillar;
            if (floorBeam != null)
            {
                objectsToRotate.Add(floorBeam);
                namesOfAllGo.Add(floorBeam.name, floorBeam);
            }
            if (topBeam != null)
            {
                objectsToRotate.Add(topBeam);
                namesOfAllGo.Add(topBeam.name, topBeam);
            }
            if (rockWall != null)
            {
                objectsToRotate.Add(rockWall);
                namesOfAllGo.Add(rockWall.name, rockWall);
            }
            if (extraPillar != null)
            {
                objectsToRotate.Add(extraPillar);
                namesOfAllGo.Add(extraPillar.name, extraPillar);
            }
                
            namesOfAllGo.Add(rotationGo.name, rotationGo);

            if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] Added {objectsToRotate.Count} GameObjects to objectsToRotate"); }

            if (LinkUiElement == null)
            {
                if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] Creating LinkUI"); }
                LinkUiElement = Tools.LinkUi.CreateLinkUi(rotationGo, 2f, null, StoneGate.stoneGateOpenCloseIcon, null);
            }
            else
            {
                if (Testing.Settings.logExtraStoneGateStoreMono) { Misc.Msg($"[StoneGate] [StoneGateStoreMono] [Init] LinkUI already exists"); }
            }
        }

    }
}
