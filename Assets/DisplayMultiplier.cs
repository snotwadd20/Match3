using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Useless.Match3
{
    public class DisplayMultiplier : MonoBehaviour
    {
        public Text textObject = null;

        // Use this for initialization
        void Start()
        {
            if (textObject == null)
                textObject = gameObject.GetComponent<Text>();

            if (textObject == null)
                textObject = gameObject.GetComponentInParent<Text>();

            if (textObject == null)
            {
                enabled = false;
                Debug.LogError(name + "'s script " + GetType() + " requires a Text object be linked, on the same object, or on a parent. Disabling");
            }//if
        }//Start

        // Update is called once per frame
        void Update()
        {
            textObject.enabled = (ScoreKeeper.globalMultiplier > 1);
            textObject.text = ScoreKeeper.globalMultiplier.ToString() + "x";
        }//Update
    }//DisplayMultiplier
}//namespace
