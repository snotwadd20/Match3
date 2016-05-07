using UnityEngine;
using System.Collections;

namespace Useless.Match3
{
    public class ScoreKeeper : MonoBehaviour
    {
        public float multiplierTimerLength = 5.0f;
        
        private static int _score = 0;
        public static int score { get { return _score; } }

        private static int _globalMultiplier = 1;
        public static int globalMultiplier { get { return _globalMultiplier; } }

        private static float _globalMultiplierTimer = 0;
        private static float multiplierTimeLeft { get { return Mathf.Max(0, _globalMultiplierTimer); } }

        public static void AddPoints(int pointsToAdd)
        {
            _score += pointsToAdd * _globalMultiplier;
        }//AddPoints

        public static void IncrementMultiplier()
        {
            _globalMultiplier++;
            _globalMultiplierTimer = self.multiplierTimerLength; //reset the timer
            print("MULT= " + _globalMultiplier + "x");
        }//IncrementMultiplier

        public static void SetMultiplier(int newMult)
        {
            _globalMultiplier = newMult;
            _globalMultiplierTimer = self.multiplierTimerLength; //reset the timer
        }//IncrementMultiplier

        void Start()
        {
            if (_self == null)
                _self = this;

        }//Start
        //int i = 0;
        void Update()
        {
            
            _globalMultiplierTimer -= Time.deltaTime;
            if (_globalMultiplierTimer <= 0)
            {
                _globalMultiplierTimer = 0;
                _globalMultiplier = 1;
            }//if

            //int scoreToAdd = Mathf.RoundToInt((i - 3) * (1.2f * i - 4) * 30);
            //scoreToAdd += 10 - (scoreToAdd % 10); //round up to ten
            //ScoreKeeper.AddPoints(scoreToAdd);
            //print(i + " match: " + scoreToAdd + " points");
            //i++;
        }//Update

        //SINGLETON
        private static ScoreKeeper _self = null;

        public static ScoreKeeper self
        {
            get
            {
                if (_self == null)
                    InitSingleton();

                return _self;
            }//get
        }//self

        private static void InitSingleton()
        {
            _self = new GameObject("ScoreKeeper").AddComponent<ScoreKeeper>();
        }//InitSingleton
    }//ScoreKeeper
}//namespace
