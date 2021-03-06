using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Purgatory
{
    public abstract class CharacterStateManager : StateManager
    {
        [Header("States")]
        public bool isGrounded;
        public bool isOnAir;
        public bool isInteracting;
        public bool useRootMotion;
        public bool lockOn;
        public bool isTwoHanded;
        public bool canDoCombo;
        public bool canRotate;
        public bool canMove;
        public float groundRayForDistance = .2f;
        public float groundDownDistanceOnAir = .4f;
        public Transform target;

        [Header("References")]
        public Animator anim;
        public new Rigidbody rigidbody;
        public AnimatorHook animHook;
        public InventoryManager weaponHolderManager;
        public NavMeshAgent agent;

        [Header("Controller Values")]
        public float vertical;
        public float horizontal;
        public float delta;
        public Vector3 rootMovement;

        [Header("Item Actions")]
        protected ItemActionContainer[] itemActions;  //4 item actions for weapon
        public ItemActionContainer[] defaultItemActions = new ItemActionContainer[4]; 


        [Header("Runtime References")]
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        protected WeaponItem currentWeaponInUse;
        protected ItemActionContainer currentItemAction;

        
        public override void Init()
        {
            anim = GetComponentInChildren<Animator>();
            animHook = GetComponentInChildren<AnimatorHook>();
            rigidbody = GetComponentInChildren<Rigidbody>();
            weaponHolderManager = GetComponentInChildren<InventoryManager>();
            agent = GetComponentInChildren<NavMeshAgent>();
            agent.updateRotation = false;
            anim.applyRootMotion = false;

            itemActions = new ItemActionContainer[4];
            PopulateListWithDefaultItemActions();
        }

        void PopulateListWithDefaultItemActions()
        {
            for (int i = 0; i < defaultItemActions.Length; i++)
            {
                itemActions[i] = defaultItemActions[i];
            }
        }

        protected ItemActionContainer GetItemActionContainer(AttackInputs ai, ItemActionContainer[] l)
        {
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i].attackInput == ai)
                {
                    return l[i];
                }
            }
            return null;
        }
        

        public void PlayTargetAnimation(string targetAnim,bool isInteracting,bool isMirror = false)
        {
            anim.SetBool("isMirror", isMirror);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }

        public virtual bool PlayTargetItemAction(AttackInputs attackInput)
        {
            return true;
        }

        public virtual void OnAssignLookOverride(Transform target)
        {
            this.target = target;
            if (target != null)
            {
                lockOn = true;
            }
        }

        public virtual void OnClearLookOverride()
        {
            lockOn = false;
        }

        public virtual void UpdateItemActionsWithCurrent()
        {
            ItemActionContainer[] newItemActions = new ItemActionContainer[4];

            for (int i = 0; i < newItemActions.Length; i++)
            {   //if there is no new item stay at dafault
                newItemActions[i] = new ItemActionContainer();
                newItemActions[i].animName = defaultItemActions[i].animName;
                newItemActions[i].attackInput = defaultItemActions[i].attackInput;
                newItemActions[i].itemActual = defaultItemActions[i].itemActual;

            }

            bool canLoadLeftHandActions = false;
            if (!isTwoHanded)
            {
                canLoadLeftHandActions = true;
            }

            //if (weaponHolderManager.rightItem != null)
            //{
            //    if (isTwoHanded)
            //    {
            //        anim.CrossFade(weaponHolderManager.rightItem.twoHanded_anim, 0.2f);
            //        anim.SetBool("isMirror", false);
            //    }
            //    else
            //    {
            //        anim.CrossFade("R_" + weaponHolderManager.rightItem.oneHanded_anim, 0.2f);
            //    }

            //    for (int i = 0; i < weaponHolderManager.rightItem.itemActions.Length; i++)
            //    {
            //        if (isTwoHanded)
            //        {
            //            if (!weaponHolderManager.rightItem.itemActions[i].isTwoHanded)
            //                continue;
            //        }
            //        else
            //        {
            //            if (weaponHolderManager.rightItem.itemActions[i].isTwoHanded)
            //                continue;
            //        }
            //        ItemActionContainer iac = GetItemActionContainer(weaponHolderManager.rightItem.itemActions[i].attackInput, newItemActions); //copying from a list and raplacing with the item list, 
                 

            //        iac.animName = weaponHolderManager.rightItem.itemActions[i].animName;
            //        //iac.attackInput = weaponHolderManager.rightItem.itemActions[i].attackInput;
            //        iac.isTwoHanded = weaponHolderManager.rightItem.itemActions[i].isTwoHanded;
            //        iac.itemActual = weaponHolderManager.rightItem;
            //    }
            //}
            //else
            //{
            //    anim.CrossFade("R_Empty", 0.2f);
            //    canLoadLeftHandActions = true;
            //}

            //if (isTwoHanded && canLoadLeftHandActions)
            //{
            //    canLoadLeftHandActions = false;
            //}

            if (!canLoadLeftHandActions)
            {
                itemActions = newItemActions;
                return;
            }
                


            //if (weaponHolderManager.leftItem != null)
            //{   //go trough all item actions on youer item, and will find a container of this list
            //    if (isTwoHanded)
            //    {
            //        anim.CrossFade(weaponHolderManager.leftItem.twoHanded_anim, 0.2f);
            //        anim.SetBool("isMirror", true);
            //    }
            //    else
            //    {
            //        anim.CrossFade("L_" + weaponHolderManager.leftItem.oneHanded_anim, 0.2f);
            //    }

            //    for (int i = 0; i < weaponHolderManager.leftItem.itemActions.Length; i++)
            //    {  
            //        if (isTwoHanded)
            //        {
            //            if (!weaponHolderManager.leftItem.itemActions[i].isTwoHanded)
            //                continue;
            //        }
            //        else
            //        {
            //            if (weaponHolderManager.leftItem.itemActions[i].isTwoHanded)
            //                continue;
            //        }

            //        ItemActionContainer weaponAction = weaponHolderManager.leftItem.itemActions[i];  //copying from a list and raplacing with the item list, 
                  
            //        if (isTwoHanded)
            //        {                     
            //            ItemActionContainer iac = GetItemActionContainer(weaponHolderManager.leftItem.itemActions[i].attackInput, newItemActions);
            //            iac.animName = weaponHolderManager.leftItem.itemActions[i].animName;
            //            iac.isTwoHanded = weaponHolderManager.leftItem.itemActions[i].isTwoHanded;
            //            iac.itemActual = weaponHolderManager.leftItem;
            //        }
            //        else
            //        {
            //            AttackInputs ai = AttackInputs.lb;
            //            if (weaponAction.attackInput == AttackInputs.rb)
            //                ai = AttackInputs.lb;
            //            if (weaponAction.attackInput == AttackInputs.rt)
            //                ai = AttackInputs.lt;

            //            ItemActionContainer iac = GetItemActionContainer(ai, newItemActions);
            //            iac.animName = weaponHolderManager.leftItem.itemActions[i].animName;
            //            iac.isTwoHanded = weaponHolderManager.leftItem.itemActions[i].isTwoHanded;
            //            iac.itemActual = weaponHolderManager.leftItem;
            //        }
            //    }
            //}
            //else
            //{
            //    anim.CrossFade("L_Empty", 0.2f);
            //}

            itemActions = newItemActions;
        }

        public void HandleTwoHanded()
        {
            if (isTwoHanded)
            {
                isTwoHanded = false;
                anim.Play("B_Empty");
            }
            else
            {
                isTwoHanded = true;
            }
            UpdateItemActionsWithCurrent();

        }

        public void AssignCurrentWeaponAndAction(WeaponItem weapon,ItemActionContainer iac)
        {
            currentWeaponInUse = weapon;
            currentItemAction = iac;
        }

   

        public Transform FindLockableTarget()
        {
            List<Transform> lockablesList = new List<Transform>();

            LayerMask lm = (1 << 9);
            Collider[] colliders = Physics.OverlapSphere(mTransform.position, 20,lm);
            foreach (Collider c in colliders)
            {
                ILockable iLock = c.GetComponentInChildren<ILockable>();
                if(iLock != null)
                {
                    Transform t = iLock.GetLockOnTarget(mTransform);
                    if(t != null)
                        lockablesList.Add(t);
                }
            }
            float minDis = float.MaxValue;
            Transform target = null;

            for (int i = 0; i < lockablesList.Count; i++)
            {
                float tempDis = Vector3.Distance(lockablesList[i].position,mTransform.position);
                if (tempDis < minDis)
                {
                    minDis = tempDis;
                    target = lockablesList[i];
                }
            }
            return target;
        }

        public Transform GetLockOnTarget(Transform from)
        {
            if(from == this.transform)
                return null;

            return mTransform;
        }

     
    }

    
}