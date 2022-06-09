using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{
    public class BasicMountController : MonoBehaviour
    {
        public CharacterController Controller;
        private float MoveSpeed = 5f;

        private Character MountedCharacter;
        private Animator Animator;

        public void Awake()
        {
            Animator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
        }

        public void SetMountedCharacter(Character mountTarget)
        {
            MountedCharacter = mountTarget;
        }

        public void Update()
        {
            if (MountedCharacter == null)
            {
                return;
            }

            FaceTransformTowardsCamera();
            UpdateMoveInput();
            UpdateAnimator();
        }

        private void FaceTransformTowardsCamera()
        {
            Vector3 YOnly = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
            transform.eulerAngles = YOnly;
        }

        private void UpdateAnimator()
        {
            Animator.SetFloat("Move X", Controller.velocity.x);
            Animator.SetFloat("Move Y", Controller.velocity.z);
        }

        private void UpdateMoveInput()
        {
            if (CustomKeybindings.GetKeyDown("TEST_DISMOUNT_BUTTON"))
            {
                MountedCharacter.StatusEffectMngr.RemoveStatusWithIdentifierName("MountedStatusEffect");
            }

            Vector3 Input = Camera.main.transform.TransformDirection(new Vector3(ControlsInput.MoveHorizontal(MountedCharacter.OwnerPlayerSys.PlayerID), 0, ControlsInput.MoveVertical(MountedCharacter.OwnerPlayerSys.PlayerID)));
            Controller.Move(Input * MoveSpeed * Time.deltaTime);
        }

        public void SetMoveSpeed(float newSpeed)
        {
            MoveSpeed = newSpeed;
        }
    }

    public class CharacterMountController : MonoBehaviour
    {
        public Character ParentCharacter;

        public EmoMountControllerState State = EmoMountControllerState.UNMOUNTED;


        public void Awake()
        {
            ParentCharacter = GetComponent<Character>();
        }

        public void EnableCharacterComponents(Character _affectedCharacter)
        {
            _affectedCharacter.CharMoveBlockCollider.enabled = true;
            _affectedCharacter.CharacterController.enabled = true;
            _affectedCharacter.CharacterControl.enabled = true;
            _affectedCharacter.Animator.enabled = true;
            _affectedCharacter.Animator.Update(Time.deltaTime);
        }

        public void DisableCharacterComponents(Character _affectedCharacter)
        {
            _affectedCharacter.CharMoveBlockCollider.enabled = false;
            _affectedCharacter.CharacterController.enabled = false;
            _affectedCharacter.CharacterControl.enabled = false;
            _affectedCharacter.Animator.StopPlayback();
            _affectedCharacter.Animator.enabled = false;
        }

        public void UpdateCharacterCamera(Character _affectedCharacter, Vector3 NewOffset)
        {
            _affectedCharacter.CharacterCamera.Offset = NewOffset;
        }

        public void SetMounted(EmoMountControllerState isMounted)
        {
            State = isMounted;

            switch (State)
            {
                case EmoMountControllerState.UNMOUNTED:
                    EnableCharacterComponents(ParentCharacter);
                break;
                case EmoMountControllerState.MOUNTED:
                    DisableCharacterComponents(ParentCharacter);
                break;
            }
        }

        
    }

    public enum EmoMountControllerState
    {
        UNMOUNTED,
        MOUNTED
    }
}
