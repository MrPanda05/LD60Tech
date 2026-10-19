using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Commons.Singletons
{
    public class SoundPool : MonoBehaviour
    {
        #region Types

        /// <summary>Um objeto de som do pool e quando ele deve voltar para a fila.</summary>
        private sealed class PooledSound
        {
            public GameObject Prefab;
            public GameObject Instance;
            public AudioSource Source;
            public float ReleaseTime;
        }

        #endregion

        #region Constants

        private const string HostName = "SoundPool";

        /// <summary>Segundos até devolver ao pool um som cujo AudioSource não tem clipe para medir a duração.</summary>
        private const float DefaultLifetime = 3f;

        /// <summary>Valor de <c>AudioSource.spatialBlend</c> para som totalmente 3D (posicional).</summary>
        private const float FullySpatialBlend = 1f;

        #endregion

        #region State

        private static SoundPool _instance;

        private readonly Dictionary<GameObject, Stack<PooledSound>> _free = new();
        private readonly List<PooledSound> _playing = new();

        #endregion

        #region API

        /// <summary>
        /// Toca o som de um prefab na posição informada, com as configurações de áudio do próprio prefab.
        /// </summary>
        /// <param name="prefab">Prefab com um AudioSource e o clipe a tocar. Nulo não faz nada.</param>
        /// <param name="position">Onde o som é ouvido, no mundo.</param>
        public static void PlayAt(GameObject prefab, Vector3 position)
        {
            if (prefab == null || !prefab.TryGetComponent(out AudioSource _)) return;

            GetInstance().Play(prefab, position);
        }

        /// <summary>
        /// Toca o som de um prefab na posição informada, como som 3D com o alcance dado.
        /// </summary>
        /// <param name="prefab">Prefab com um AudioSource e o clipe a tocar. Nulo não faz nada.</param>
        /// <param name="position">Onde o som é ouvido, no mundo.</param>
        /// <param name="maxDistance">Distância máxima, em metros, em que o som é ouvido.</param>
        public static void PlayAt(GameObject prefab, Vector3 position, float maxDistance)
        {
            if (prefab == null || !prefab.TryGetComponent(out AudioSource _)) return;

            PooledSound sound = GetInstance().Play(prefab, position);

            sound.Source.spatialBlend = FullySpatialBlend;
            sound.Source.maxDistance = maxDistance;
        }

        #endregion

        #region Lifecycle

        /// <summary>Devolve ao pool os sons que já terminaram.</summary>
        private void Update()
        {
            for (int i = _playing.Count - 1; i >= 0; i--)
            {
                PooledSound sound = _playing[i];

                if (sound.Instance != null && Time.unscaledTime < sound.ReleaseTime) continue;

                _playing.RemoveAt(i);

                if (sound.Instance != null) Release(sound);
            }
        }

        #endregion

        #region Pool

        /// <summary>O pool da cena atual, criado na primeira chamada e recriado se a cena anterior o levou embora.</summary>
        private static SoundPool GetInstance()
        {
            if (_instance == null) _instance = new GameObject(HostName).AddComponent<SoundPool>();

            return _instance;
        }

        /// <summary>
        /// Pega um objeto de som livre (ou cria um), posiciona e toca. O alcance 3D, se pedido, é aplicado por quem chama.
        /// </summary>
        private PooledSound Play(GameObject prefab, Vector3 position)
        {
            PooledSound sound = TakeFree(prefab) ?? Create(prefab);

            sound.Instance.transform.position = position;
            sound.Instance.SetActive(true);
            sound.Source.Play();

            AudioClip clip = sound.Source.clip;
            sound.ReleaseTime = Time.unscaledTime + (clip != null ? clip.length : DefaultLifetime);

            _playing.Add(sound);

            return sound;
        }

        /// <summary>Próximo objeto livre do prefab, ou nulo se não há. Descarta os que foram destruídos por fora.</summary>
        private PooledSound TakeFree(GameObject prefab)
        {
            if (!_free.TryGetValue(prefab, out Stack<PooledSound> stack)) return null;

            while (stack.Count > 0)
            {
                PooledSound sound = stack.Pop();

                if (sound.Instance != null) return sound;
            }

            return null;
        }

        private PooledSound Create(GameObject prefab)
        {
            GameObject instance = Instantiate(prefab, transform);

            return new PooledSound { Prefab = prefab, Instance = instance, Source = instance.GetComponent<AudioSource>() };
        }

        /// <summary>Para o som, desativa o objeto e o devolve à fila do prefab.</summary>
        private void Release(PooledSound sound)
        {
            sound.Source.Stop();
            sound.Instance.SetActive(false);

            if (!_free.TryGetValue(sound.Prefab, out Stack<PooledSound> stack))
            {
                stack = new Stack<PooledSound>();
                _free[sound.Prefab] = stack;
            }

            stack.Push(sound);
        }

        #endregion
    }
}
