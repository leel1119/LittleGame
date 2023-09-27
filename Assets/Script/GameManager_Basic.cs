using BasicCode;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace BasicCode
{
    public class GameManager_Basic : MonoBehaviour
    {
        [SerializeField] private int maxLife = 100;
        private int currentLife = 0;
        public int CurrentLife => currentLife;
        public int gems = 0;
        public int Gems => gems;
        private int maxGemAmount = 9;
        private int score = 0;
        [SerializeField] private Text Text_Life, Text_Gems, Text_Score, Text_GameOver_Score;

        private float ResetTime = 5f;

        private PlayerHealth playerHealth;
        private Vector3 RevivalPos;
        // Start is called before the first frame update
        void Start()
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
            playerHealth.HitReceived += PlayerOnHit;
            playerHealth.DeadReceived += PlayerOnDead; 
            playerHealth.RevivalReceived += PlayerOnRevival;
            playerHealth.HealingReceived += PlayerOnHealthing;
            Text_GameOver_Score.transform.parent.gameObject.SetActive(false);

            currentLife = maxLife;
            Text_Life.text = currentLife.ToString();
            Text_Gems.text = Text_Score.text = "0";
        }

        private void PlayerOnHealthing(object sender, EventArgs e)
        {
            currentLife += 30;
            if (currentLife > 100)
            {
                currentLife = 100;
            }
            Text_Life.text = currentLife.ToString();
        }

        public void SetRevivalPos(Vector3 pos)
        {
            RevivalPos = pos;
        }
        private void PlayerOnDead(object sender, EventArgs e) { SubtractLife(); }

        public void SubtractLife()
        {
            currentLife -= 10;
            if (currentLife <= 0)
            {
                currentLife = 0;
                GameOver();
            }
            Text_Life.text = currentLife.ToString();
        }
        public void GameOver()
        {
            Text_GameOver_Score.transform.parent.gameObject.SetActive(true);
            Text_GameOver_Score.text = score.ToString();
            Text_Life.transform.parent.gameObject.SetActive(false);
            Text_Gems.transform.parent.gameObject.SetActive(false);
            Text_Score.transform.parent.gameObject.SetActive(false);

            StartCoroutine(AfterReset());
        }

        IEnumerator AfterReset()
        {
            yield return new WaitForSeconds(ResetTime);
            SceneManager.LoadScene(0);
        }

        private void PlayerOnRevival(object sender, EventArgs e)
        {
            playerHealth.gameObject.transform.position = RevivalPos;
        }

        private void PlayerOnHit(object sender, EventArgs e) 
        {
            //SubGems();
            SubtractLife();
        }
        public void AddGems()
        {
            SettingGems(1);
            AddScore(100);
        }
        //public void SubGems() { SettingGems(-1); }

        private void SettingGems(int amount)
        {
            print("Gems");
            gems += amount;
            if (gems > maxGemAmount) gems = maxGemAmount;
            if(gems < 0) { gems = 0; }
            Text_Gems.text = gems.ToString();
        }
        public void AddScore(int amount)
        {
            score += gems * amount;
            Text_Score.text = score.ToString();

            Text_GameOver_Score.text = score.ToString();
        }
    }
}