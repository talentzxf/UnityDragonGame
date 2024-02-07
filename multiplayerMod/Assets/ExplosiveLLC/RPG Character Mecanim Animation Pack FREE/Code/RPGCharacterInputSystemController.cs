// To switch your project to using the new InputSystem:
// Edit>Project Settings>Player>Active Input Handling change to "Input System Package (New)".

using UnityEngine;
using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Lookups;

// Requires installing the InputSystem Package from the Package Manager: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/Installation.html
using UnityEngine.InputSystem;

namespace RPGCharacterAnims
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html")]

	public class RPGCharacterInputSystemController : MonoBehaviour
    {
        RPGCharacterController rpgCharacterController;

		//InputSystem
		public @RPGInputs rpgInputs;

		// Inputs.
		private bool inputJump;
        private bool inputLightHit;
        private bool inputKnockdown;
        private bool inputAttackL;
        private bool inputAttackR;
		private bool inputRoll;
		private bool inputAim;
		private Vector2 inputMovement;
		private bool inputFace;
		private Vector2 inputFacing;
		private bool inputSwitchUp;
		private bool inputSwitchDown;

		// Variables.
		private Vector3 moveInput;
		private Vector3 currentAim;
		private float inputPauseTimeout = 0;
		private bool inputPaused = false;

		private void Awake()
        {
            rpgCharacterController = GetComponent<RPGCharacterController>();
			rpgInputs = new @RPGInputs();
			currentAim = Vector3.zero;
        }

		private void OnEnable()
		{ rpgInputs.Enable(); }

		private void OnDisable()
		{ rpgInputs.Disable(); }

		public bool HasMoveInput() => moveInput.magnitude > 0.1f;

		public bool HasAimInput() => inputAim;

		public bool HasFacingInput() => inputFacing != Vector2.zero || inputFace;

		private void Update()
		{
			// Pause input for other external input.
			if (inputPaused) {
				if (Time.time > inputPauseTimeout) { inputPaused = false; }
				else { return; }
			}

			if (!inputPaused) { Inputs(); }

			Moving();
			Jumping();
			Damage();
			SwitchWeapons();
			Strafing();
			Facing();
			Aiming();
			Rolling();
			Attacking();
		}

		/// <summary>
		/// Pause input for a number of seconds.
		/// </summary>
		/// <param name="timeout">The amount of time in seconds to ignore input.</param>
		public void PauseInput(float timeout)
		{
			inputPaused = true;
			inputPauseTimeout = Time.time + timeout;
		}

		/// <summary>
		/// Input abstraction for easier asset updates using outside control schemes.
		/// </summary>
		private void Inputs()
        {
            try {
				inputAttackL = rpgInputs.RPGCharacter.AttackL.WasPressedThisFrame();
				inputAttackR = rpgInputs.RPGCharacter.AttackR.WasPressedThisFrame();
				inputKnockdown = rpgInputs.RPGCharacter.Knockdown.WasPressedThisFrame();
				inputFace = rpgInputs.RPGCharacter.Face.IsPressed();
				inputFacing = rpgInputs.RPGCharacter.Facing.ReadValue<Vector2>();
				inputJump = rpgInputs.RPGCharacter.Jump.IsPressed();
				inputLightHit = rpgInputs.RPGCharacter.LightHit.WasPressedThisFrame();
				inputMovement = rpgInputs.RPGCharacter.Move.ReadValue<Vector2>();
				inputRoll = rpgInputs.RPGCharacter.Roll.WasPressedThisFrame();
				inputAim = rpgInputs.RPGCharacter.Aim.IsPressed();
				inputSwitchDown = rpgInputs.RPGCharacter.WeaponDown.WasPressedThisFrame();
				inputSwitchUp = rpgInputs.RPGCharacter.WeaponUp.WasPressedThisFrame();

                // Slow time toggle.
                if (Keyboard.current.tKey.wasPressedThisFrame) {
                    if (rpgCharacterController.CanStartAction("SlowTime"))
					{ rpgCharacterController.StartAction("SlowTime", 0.125f); }
					else if (rpgCharacterController.CanEndAction("SlowTime"))
					{ rpgCharacterController.EndAction("SlowTime"); }
                }
                // Pause toggle.
                if (Keyboard.current.pKey.wasPressedThisFrame) {
                    if (rpgCharacterController.CanStartAction("SlowTime"))
					{ rpgCharacterController.StartAction("SlowTime", 0f); }
					else if (rpgCharacterController.CanEndAction("SlowTime"))
					{ rpgCharacterController.EndAction("SlowTime"); }
                }
            }
			catch (System.Exception) { Debug.LogError("Inputs not found!  " +
				"Make sure your project is using the new InputSystem: Edit>Project Settings>Player>Active Input Handling  - change to 'Input System Package (New)'."); }
			}

			public void Moving()
		{
			moveInput = new Vector3(inputMovement.x, inputMovement.y, 0f);

			// Filter the 0.1 threshold of HasMoveInput.
			if (HasMoveInput()) { rpgCharacterController.SetMoveInput(moveInput); }
			else { rpgCharacterController.SetMoveInput(Vector3.zero); }
		}

		private void Jumping()
		{
			// Set the input on the jump axis every frame.
			Vector3 jumpInput = inputJump ? Vector3.up : Vector3.zero;
			rpgCharacterController.SetJumpInput(jumpInput);

			// If we pressed jump button this frame, jump.
			if (inputJump && rpgCharacterController.CanStartAction("Jump")) { rpgCharacterController.StartAction("Jump"); }
			else if (inputJump && rpgCharacterController.CanStartAction("DoubleJump")) { rpgCharacterController.StartAction("DoubleJump"); }
		}

		public void Rolling()
		{
			if (!inputRoll) { return; }
			if (!rpgCharacterController.CanStartAction("DiveRoll")) { return; }

			rpgCharacterController.StartAction("DiveRoll", 1);
		}

		private void Aiming()
		{ Strafing(); }

		private void Strafing()
		{
			if (rpgCharacterController.canStrafe) {
				if (inputAim) {
					if (rpgCharacterController.CanStartAction("Strafe")) { rpgCharacterController.StartAction("Strafe"); }
				}
				else {
					if (rpgCharacterController.CanEndAction("Strafe")) { rpgCharacterController.EndAction("Strafe"); }
				}
			}
		}

		private void Facing()
		{
			if (rpgCharacterController.canFace) {
				if (HasFacingInput()) {
					if (inputFace) {

						// Get world position from mouse position on screen and convert to direction from character.
						Plane playerPlane = new Plane(Vector3.up, transform.position);
						Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
						float hitdist = 0.0f;
						if (playerPlane.Raycast(ray, out hitdist)) {
							Vector3 targetPoint = ray.GetPoint(hitdist);
							Vector3 lookTarget = new Vector3(targetPoint.x - transform.position.x, transform.position.z - targetPoint.z, 0);
							rpgCharacterController.SetFaceInput(lookTarget);
						}
					}
					else { rpgCharacterController.SetFaceInput(new Vector3(inputFacing.x, inputFacing.y, 0)); }

					if (rpgCharacterController.CanStartAction("Face")) { rpgCharacterController.StartAction("Face"); }
				}
				else {
					if (rpgCharacterController.CanEndAction("Face")) { rpgCharacterController.EndAction("Face"); }
				}
			}
		}

		private void Attacking()
		{
			// Check to make sure Attack Action exists.
			if (!rpgCharacterController.HandlerExists(HandlerTypes.Attack)) { return; }

			// Check to make character can Attack.
			if (!rpgCharacterController.CanStartAction(HandlerTypes.Attack)) { return; }

			if (inputAttackL)
			{ rpgCharacterController.StartAction(HandlerTypes.Attack, new AttackContext(HandlerTypes.Attack, Side.Left)); }
			else if (inputAttackR)
			{ rpgCharacterController.StartAction(HandlerTypes.Attack, new AttackContext(HandlerTypes.Attack, Side.Right)); }
		}

		private void Damage()
		{
			// Hit.
			if (rpgCharacterController.HandlerExists(HandlerTypes.GetHit)) {
				if (inputLightHit) { rpgCharacterController.StartAction(HandlerTypes.GetHit, new HitContext()); }
			}

			// Knockdown.
			if (rpgCharacterController.HandlerExists(HandlerTypes.Knockdown)) {
				if (inputKnockdown && rpgCharacterController.CanStartAction(HandlerTypes.Knockdown))
				{ rpgCharacterController.StartAction(HandlerTypes.Knockdown, new HitContext(( int )KnockdownType.Knockdown1, Vector3.back)); }
			}
		}

		/// <summary>
		/// Cycle weapons using directional pad input. Up and Down cycle forward and backward through
		/// the list of two handed weapons. Left cycles through the left hand weapons. Right cycles through
		/// the right hand weapons.
		/// </summary>
		private void SwitchWeapons()
		{
			// Check to make sure SwitchWeapon Action exists.
			if (!rpgCharacterController.HandlerExists(HandlerTypes.SwitchWeapon)) { return; }

			// Bail out if we can't switch weapons.
			if (!rpgCharacterController.CanStartAction(HandlerTypes.SwitchWeapon)) { return; }

			var doSwitch = false;
			var context = new SwitchWeaponContext();
			var weaponNumber = Weapon.Unarmed;

			// Cycle through 2Handed weapons if any input happens on the up-down axis.
			if (inputSwitchUp || inputSwitchDown) {
				var twoHandedWeapons = new Weapon[] {
					Weapon.TwoHandSword
				};
				// If we're not wielding 2Handed weapon already, just switch to the first one in the list.
				if (System.Array.IndexOf(twoHandedWeapons, rpgCharacterController.rightWeapon) == -1)
				{ weaponNumber = twoHandedWeapons[0]; }

				// Otherwise, we should loop through them.
				else {
					var index = System.Array.IndexOf(twoHandedWeapons, rpgCharacterController.rightWeapon);
					if (inputSwitchUp) { index = (index - 1 + twoHandedWeapons.Length) % twoHandedWeapons.Length; }
					else if (inputSwitchDown) { index = (index + 1) % twoHandedWeapons.Length; }
					weaponNumber = twoHandedWeapons[index];
				}
				// Set up the context and flag that we actually want to perform the switch.
				doSwitch = true;
				context.type = HandlerTypes.Switch;
				context.side = "None";
				context.leftWeapon = Weapon.Unarmed;
				context.rightWeapon = weaponNumber;
			}

			// If we've received input, then "doSwitch" is true, and the context is filled out,
			// so start the SwitchWeapon action.
			if (doSwitch) { rpgCharacterController.StartAction(HandlerTypes.SwitchWeapon, context); }
		}
	}

	/// <summary>
	/// Extension Method to allow checking InputSystem without Action Callbacks.
	/// </summary>
	public static class InputActionExtensions
	{
		public static bool IsPressed(this InputAction inputAction) => inputAction.ReadValue<float>() > 0f;

		public static bool WasPressedThisFrame(this InputAction inputAction) => inputAction.triggered && inputAction.ReadValue<float>() > 0f;

		public static bool WasReleasedThisFrame(this InputAction inputAction) => inputAction.triggered && inputAction.ReadValue<float>() == 0f;
	}
}