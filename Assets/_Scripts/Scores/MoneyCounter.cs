using TMPro;
using UnityEngine;

namespace LD60Tec.Score
{
    public class MoneyCounter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;


        private void Update()
        {
            if (_text != null)
            {
                _text.text = $"{Moneymanager.Instance.CurrentMoney}$";
            }
        }
    }
}
