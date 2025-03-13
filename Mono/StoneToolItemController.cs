using Sons.Animation;
using Sons.Inventory;
using Sons.Items;
using Sons.Weapon;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;


namespace StoneGate.Mono
{
    internal class StoneToolItemController : GenericMeleeWeaponController
    {
        private void Awake()
        {
            Misc.Msg("[StoneToolItemController] [Awake] Called");
            GameObject _pickAxeHeld = ItemTools.GetHeldPrefab(663).gameObject;
            GameObject.Instantiate(_pickAxeHeld.transform.GetChild(1), transform);
            GameObject.Instantiate(_pickAxeHeld.transform.GetChild(2), transform);
            GameObject.Instantiate(_pickAxeHeld.transform.GetChild(3), transform);

            // Modify WeaponInfo
            weaponInfo info = gameObject.GetComponentInChildren<weaponInfo>();
            info._itemID = 751152;
            BoxCollider collider = gameObject.transform.FindDeepChild("LowerColliders").GetComponent<BoxCollider>();
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Collider> colliders = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Collider>(1);
            colliders.AddFirst(collider);
            info._lowWeaponColliders = colliders;

            Misc.Msg("[StoneToolItemController] [Awake] Done");
        }

        private void Start()
        {
            Misc.Msg("[StoneToolItemController] [Start] Called");
            _attackActive = false;
            _attackInputState = GenericMeleeWeaponController.AttackInputState.Idle;
            _attackStateStartTime = -1f;
            _buttonPressTime = -1f;
            _buttonPressTimeout = 0.1f;
            _cancelGroundComboAttackTime = 0.7f;
            _cancelQueuedGroundAttackTime = 0.4f;
            _canChopTrees = false;
            _canComboGroundAttack = true;
            _chargeAttackType = weaponInfo.SwingType.ChargeAttack;
            _checkCombo = false;
            _comboEnderActive = false;
            _comboHitSwings = 0;
            _curve = ItemTools.GetHeldPrefab(663).gameObject.GetComponent<PickaxeItemController>()._curve;
            _curveDuration = 0.18f;
            // Skipped _fullBodyAttackActivators
            _groundAttackAnimTagActive = false;
            _groundAttackOptions = GenericMeleeWeaponController.GroundAttackOptions.Hold;
            _groundAttackQueueFrame = 0;
            _groundAttackQueueTime = 0f;
            _groundAttackState = GenericMeleeWeaponController.GroundAttackState.Idle;
            _groundAttackType = GenericMeleeWeaponController.GroundAttackType.Smash;
            _hasChargeAttack = true;
            _hitDirection = 0;
            _lastAttackStartTime = -1f;
            _lastHitTime = -1f;
            _movementSlowed = false;
            _onHitDelay = 0.05f;
            _onHitMinSpeed = 0.1f;
            _swingHit = false;
            // Skipped _treeAttackActivators
            _treeAttackWaitTimeout = 0.6f;
            _treeChopActive = false;
            _treeCutMaxDistance = 1.75f;
            _treeCutPlayerMoveSpeed = 0.8f;
            _treeCutPlayerRotationSpeed = 1f;
            _treeHitFrequency = 0f;
            _treeMaxSwingHits = 0;
            _treeSwingStaminaCost = 8f;
            _useOnHitCurve = false;

            // MeleeWeponController
            _allowBlocking = true;
            _attackHoldTime = 0f;
            _blockImpactTime = -1f;
            _currentSwingSpeed = 1f;
            _damageController = null;
            _groundAttackAngleActive = false;
            _groundAttackAngleBase = 0.25f;
            _isBlockDamageActive = 0;
            _isHoldingAttack = false;
            _isHoldingBlocking = false;
            _itemRenderable = null;
            _lastAttackFrame = -1;
            _lastAttackHeldFrame = -1;
            _lastAttackHeldTime = -1;
            _lastAttackTime = -1;
            _meleeWeaponData = null;
            _playerStats = null;
            _treeCutDamageMultiplierMethod = null;
            _twoArmBlock = false;
            _weaponInfo = gameObject.GetComponentInChildren<weaponInfo>();

            // HeldControllerBase
        }

        public override bool IsItemIdle()
        {
            if (this._playerAnimatorControl == null)
            {
                return true;
            }
            if (!base.CanStashFullBody())
            {
                return false;
            }
            int shortNameHash = this._playerAnimatorControl._playerUpperBodyState.shortNameHash;
            return shortNameHash == AnimationHashes.IdlePickaxeHash || shortNameHash == AnimationHashes.MovePickaxeHash;
        }
    }
}
