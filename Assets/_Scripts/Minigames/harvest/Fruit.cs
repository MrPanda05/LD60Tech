using LD60Tec.Commons.Singletons;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD60Tec.Minigames.Harvest
{
    public class Fruit : MonoBehaviour
    {
        public SpriteRenderer Spriterender {  get; private set; }
        public event Action<Fruit> OnFruitClick;
        [SerializeField]
        private GameObject _audio;

        private void Awake()
        {
            Spriterender = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePos);

                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    SoundPool.PlayAt(_audio, transform.position);
                    OnFruitClick?.Invoke(this);
                    gameObject.SetActive(false);
                }
            }
        }

    }
}
