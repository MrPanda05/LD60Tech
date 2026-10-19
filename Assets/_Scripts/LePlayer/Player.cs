using LD60Tec.Commons.Components;
using LD60Tec.Inventorys;
using LD60Tec.Minigames;
using LD60Tec.Requests;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace LD60Tec.LePlayer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float _speed;
        private Rigidbody2D _rg;
        [SerializeField] private InputActionAsset _playerActions;
        private InputAction _move;
        private InputAction _interact;
        private InputAction _jump;
        public Vector2 Movement { get; private set; }
        [field: SerializeField]
        public InteractorComponent Interactror { get; private set; }
        [field: SerializeField]
        public InventorySystem Inventory { get; private set; }
        private bool _isGrounded;
        [SerializeField]
        private float _jumpForce = 100;
        private bool _isOnOneWay;

        private void Awake()
        {
            _rg = GetComponent<Rigidbody2D>();
            _move = _playerActions.FindAction("Move");
            _interact = _playerActions.FindAction("Interact");
            _jump = _playerActions.FindAction("Jump");
            _playerActions.Enable();

        }
        private void Start()
        {
            _playerActions.Enable();
            RequestManagers.Instance.OnGameLost += DisableMovement;
        }
        public void DisableMovement()
        {
            _playerActions.Disable();
        }
        public void EnableMovement()
        {
            _playerActions.Enable();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            int targetLayer = LayerMask.NameToLayer("Ground");
            if (collision.gameObject.layer == targetLayer)
            {
                //print("I'm ground");
                _isGrounded = true;
            }
            if (collision.gameObject.CompareTag("oneway"))
            {
                _isOnOneWay = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            int targetLayer = LayerMask.NameToLayer("Ground");
            if (collision.gameObject.layer == targetLayer)
            {
                //print("I'm not ground");
                _isGrounded = false;
            }
            if (collision.gameObject.CompareTag("oneway"))
            {
                _isOnOneWay = false;
            }
        }
        private void OnEnable()
        {
            _interact.started += OnInteract;
            _jump.started += Jump;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (!_isGrounded) return;
            _rg.AddForceY(_jumpForce, ForceMode2D.Impulse);
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log($"[1] Interact pressed, frame {Time.frameCount}");
            Interactror.UseInteraction(this);
        }

        private void OnDisable()
        {
            _interact.started -= OnInteract;
            _jump.started -= Jump;
        }
        public void Move()
        {
            if (_rg == null) return;
            _rg.linearVelocity = new Vector2(Movement.x * _speed, _rg.linearVelocityY);
        }
        private void Update()
        {
            Movement = _move.ReadValue<Vector2>().normalized;
        }
        private void FixedUpdate()
        {
            Move();
            if(Movement.y  < 0 && _isOnOneWay)
            {
                transform.position -= new Vector3(0, 1);
            }
        }
        private void OnDestroy() { _playerActions.Disable(); }
    }
}
