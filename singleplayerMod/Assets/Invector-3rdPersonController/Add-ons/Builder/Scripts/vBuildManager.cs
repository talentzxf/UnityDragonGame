using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using System.Collections.Generic;
using UnityEngine;

[vClassHeader("Build Manager", "This component requires a ShooterController, ItemManager and a GenericAction. Make sure to add this as a child of the controller.", openClose = false)]
public class vBuildManager : vMonoBehaviour
{
    [vReadOnly]
    public List<vBuildObject> builds;
    [vReadOnly]
    public vBuildObject currentBuild;
    public vItemManager itemManager;
    public vGenericAction genericAction;

    [vHelpBox("Optional - You can spawn a item instantly or use the TriggerGenericAction to play a animation first and spawn after")]
    public GenericInput createBuildInput = new GenericInput("E", "X", "X");
    [Space]
    public GenericInput enterBuildMode = new GenericInput("B", "A", "A");
    public GenericInput rotateLeft = new GenericInput("Alpha1", "LB", "LB");
    public GenericInput rotateRight = new GenericInput("Alpha2", "RB", "RB");
    public bool rotateToCameraFwdWhenBuild = true;
    public UnityEngine.Events.UnityEvent onIsItemEquipped, onIsItemUnequipped, onEnterBuildMode, onExitBuildMode;

    public bool inBuildMode;

    protected vThirdPersonInput tpInput;

    void Start()
    {
        tpInput = GetComponentInParent<vThirdPersonInput>();
        if (tpInput != null)
        {
            tpInput.onFixedUpdate += UpdateBuilderBehavior;
            tpInput.onUpdate += CheckInput;
        }
        itemManager = GetComponentInParent<vItemManager>();
        genericAction = GetComponentInParent<vGenericAction>();

        var _builds = GetComponentsInChildren<vBuildObject>();
        builds.Clear();
        for (int i = 0; i < _builds.Length; i++)
        {
            var item = itemManager.itemListData.items.Find(_item => _item.id.Equals(_builds[i].id));
            if (item != null)
            {
                builds.Add(_builds[i]);
                _builds[i].onSpawnBuild.AddListener(OnSpawnBuildObject);
                _builds[i].spawnPrefab = item.originalObject;
            }
        }
    }

    public virtual void UpdateBuilderBehavior()
    {
        CheckBuilds();
        HandleBuild();
        ItemEquippedEvents();
    }

    public virtual void SetLockAllInput(bool value)
    {
        tpInput.SetLockAllInput(value);
        itemManager.LockInventoryInput(value);
    }

    public virtual void ItemEquippedEvents()
    {
        if (!currentBuild) return;

        if (currentBuild.canCreate)
            onIsItemEquipped.Invoke();
        else
            onIsItemUnequipped.Invoke();
    }

    public virtual void HandleBuildRotation()
    {
        if (rotateLeft.GetButton())
            currentBuild.rotationY -= Time.deltaTime * currentBuild.rotateSpeed;
        else if (rotateRight.GetButton())
            currentBuild.rotationY += Time.deltaTime * currentBuild.rotateSpeed;
    }

    public virtual void HandleBuild()
    {
        if (!inBuildMode || !currentBuild)
            return;
        if (!string.IsNullOrEmpty(currentBuild.customCameraState))
            tpInput.ChangeCameraState(currentBuild.customCameraState, true);

        tpInput.MoveInput();

        currentBuild.HandleBuild(this);
        HandleBuildRotation();
    }

    public virtual void CheckBuilds()
    {
        for (int i = 0; i < builds.Count; i++)
        {
            if (itemManager.ItemIsEquipped(builds[i].id, out builds[i].equipedItemInfo))
            {
                currentBuild = builds[i];
                break;
            }
            else builds[i].equipedItemInfo = null;
        }
    }

    public virtual void CheckInput()
    {
        if (currentBuild && currentBuild.canCreate)
        {
            if (enterBuildMode.GetButtonDown())
            {
                if (!inBuildMode)
                {
                    EnterBuildMode();
                }
                else
                {
                    ExitBuildMode();
                }
            }
            if (inBuildMode)
                currentBuild.CreateObjectInput(this);
        }
        else if (inBuildMode)
        {
            ExitBuildMode();
        }
    }

    public virtual void EnterBuildMode()
    {
        inBuildMode = true;
        SetLockAllInput(true);
        if (currentBuild) currentBuild.EnterBuild();
        if (currentBuild && currentBuild.strafeWhileCreate && tpInput.cc.locomotionType != vThirdPersonMotor.LocomotionType.OnlyStrafe)
            tpInput.SetStrafeLocomotion(true);
        onEnterBuildMode.Invoke();
        genericAction.SetLockTriggerEvents(true);
    }

    public virtual void ExitBuildMode()
    {
        SetLockAllInput(false);
        if (currentBuild) currentBuild.ExitBuild();
        if (currentBuild && currentBuild.strafeWhileCreate && tpInput.cc.locomotionType != vThirdPersonMotor.LocomotionType.OnlyStrafe)
            tpInput.SetStrafeLocomotion(false);
        tpInput.ResetCameraState();
        onExitBuildMode.Invoke();
        genericAction.SetLockTriggerEvents(false);
        inBuildMode = false;
    }

    public virtual void OnSpawnBuildObject()
    {
        SetLockAllInput(false);
        tpInput.ResetCameraState();
        onExitBuildMode.Invoke();
        genericAction.SetLockTriggerEvents(false);
        inBuildMode = false;

        if (!currentBuild) return;
        if (currentBuild.strafeWhileCreate && tpInput.cc.locomotionType != vThirdPersonMotor.LocomotionType.OnlyStrafe)
            tpInput.SetStrafeLocomotion(false);

        if (itemManager.ItemIsEquipped(currentBuild.id, out currentBuild.equipedItemInfo) && currentBuild.equipedItemInfo.item.canBeDestroyed && currentBuild.equipedItemInfo.item.destroyAfterUse)
        {
            itemManager.DestroyItem(currentBuild.equipedItemInfo.item, 1);
        }
    }

}