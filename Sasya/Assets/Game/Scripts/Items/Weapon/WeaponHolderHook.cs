using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Purgatory
{
    public class WeaponHolderHook : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHook;

        GameObject currentModel;
        
        //reset the item action (defaultItemAction)
        public void UnloadWeapon()
        {
            if (currentModel != null)
            {
                currentModel.SetActive(false);
                //Destroy(currentModel);
            }
        }

        public void UnloadWeaponAndDestroy()
        {
            if (currentModel != null)
            {
                Destroy(currentModel);
            }
        }

        public WeaponHook LoadWeaponModel(WeaponItem weaponItem)
        {
            if (currentModel != null)
            {
                UnloadWeapon();
            }
            if (weaponItem == null)
            {
                UnloadWeapon();
                return null;
            }

            GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject;
            weaponItem.weaponHook = model.GetComponent<WeaponHook>();
            if (model != null)
            {
                if (parentOverride != null)
                {
                    model.transform.parent = parentOverride;
                }
                else
                {
                    model.transform.parent = this.transform;
                }

                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;             
            }
            currentModel = model;
            return weaponItem.weaponHook;
        }
    }

}
