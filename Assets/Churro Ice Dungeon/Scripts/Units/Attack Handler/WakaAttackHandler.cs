using Bremsengine;
using Core.Extensions;
using Core.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChurroIceDungeon
{
    public class WakaAttackHandler : AttackHandler
    {
        [SerializeField] Transform shotTypeAnchor;
        List<GameObject> spawnedShots = new List<GameObject>();
        int iteration = 0;
        private void SetShottype(ItemData item)
        {
            Debug.Log("T");
            if (item is ShottypeItem shot)
            {
                SpawnItem(shot.containedAttackPrefab);
            }
        }
        private void Update()
        {
            if (isAttackPressed && TryAttack(Owner.CurrentPosition + Vector2.up))
            {

            }
        }
        private void SpawnItem(ChurroBaseAttack attack)
        {
            DestroySpawnedItems();
            OnAttack = null;
            ChurroBaseAttack spawn = Instantiate(attack, shotTypeAnchor);
            spawnedShots.Add(spawn.gameObject);
            spawn.gameObject.SetActive(true);
            spawn.SetOwner(Owner);
            spawn.SetHandler(this);
        }
        public void RegisterWeapon(ChurroBaseAttack attack)
        {
            spawnedShots.AddIfDoesntExist(attack.gameObject);
        }
        private void DestroySpawnedItems()
        {
            for (int i = 0; i < spawnedShots.Count; i++)
            {
                if (spawnedShots[i] == null)
                {
                    spawnedShots.RemoveAt(i);
                    i--;
                }
                Destroy(spawnedShots[i].gameObject);
                spawnedShots.RemoveAt(i);
                i--;
            }
        }
        bool isAttackPressed;
        void PressAttackInput(InputAction.CallbackContext c)
        {
            isAttackPressed = true;
        }
        void ReleaseAttackInput(InputAction.CallbackContext c)
        {
            isAttackPressed = false;
        }
        protected override void WhenStart()
        {
            ItemData.BindEvent(SetShottype, ItemActionKey.Shottype);
            PlayerInputController.actions.Shmup.Fire.performed += PressAttackInput;
            PlayerInputController.actions.Shmup.Fire.canceled += ReleaseAttackInput;
        }
        protected override void WhenDestroy()
        {
            ItemData.ReleaseEvent(SetShottype, ItemActionKey.Shottype);
            PlayerInputController.actions.Shmup.Fire.performed -= PressAttackInput;
            PlayerInputController.actions.Shmup.Fire.canceled -= ReleaseAttackInput;
        }
    }
}
