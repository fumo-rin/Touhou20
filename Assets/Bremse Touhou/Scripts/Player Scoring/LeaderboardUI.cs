using UnityEngine;
using TMPro;
using Bremsengine;
using Dan.Main;
using System.Text.RegularExpressions;
using Core.Extensions;
using UnityEngine.UIElements;

namespace BremseTouhou
{
    #region Leaderboard
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField] TMP_Text[] entryTextObjects;
        [SerializeField] TMP_InputField userNameInput;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] GameObject LeaderboardContainer;
        const string PublicKey = "9cff76cd79e78501788160362aded20e4b342c591cc9dadc056116073d61d30b";
        public static int LeaderboardScore => (int)GeneralManager.VisibleScore;
        private void Start()
        {
            HideLeaderboard();
        }
        public void ToggleLeaderboard()
        {
            SetLeaderboardVisibility(!LeaderboardContainer.activeInHierarchy);
        }
        public void ShowLeaderboard()
        {
            SetLeaderboardVisibility(true);
        }
        public void HideLeaderboard()
        {
            SetLeaderboardVisibility(false);
        }
        public void SetLeaderboardVisibility(bool state)
        {
            scoreText.text = GeneralManager.VisibleScore.ToString("F0");
            LeaderboardContainer.SetActive(state);
            if (state == true)
            {
                LoadEntries();
            }
        }
        public void SubmitLeaderboardEntry()
        {
            string name = userNameInput.text;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            userNameInput.text = "";
            int score = LeaderboardScore;
            Debug.Log("Attempting to send Leaderboard Entry");
            if (GeneralManager.IsScoreLegit())
            {
                Debug.Log("Score seems legit");
                //string extra = $"{TouhouManager.GetDifficultyName(TouhouManager.CurrentDifficulty)} {TouhouManager.missCount}miss";
                if (score > 0)
                {
                    UploadEntry(name, score);
                }
            }
        }
        void LoadEntries()
        {
            Debug.Log("Loading Leaderboard Entries");
            LeaderboardCreator.GetLeaderboard(PublicKey, ((msg) =>
            {
                for (int i = 0; i < entryTextObjects.Length && i < msg.Length; i++)
                {
                    Debug.Log("T");
                    entryTextObjects[i].text = msg[i].Username + " - " +msg[i].Score.ToString("F0");
                }
            }));
            /*Leaderboards.Sauna_Quest_Ultra.GetEntries(entries =>
            {
                foreach (var entry in entryTextObjects)
                    entry.text = "";

                var length = Mathf.Min(entryTextObjects.Length, entries.Length);
                for (int i = 0; i < length; i++)
                {
                    entryTextObjects[i].text = $"{entries[i].Rank}. {entries[i].Username} - {entries[i].Score}";
                }
            });*/
        }
        public static string UseRegex(string strIn)
        {
            // Replace invalid characters with empty strings.
            return Regex.Replace(strIn, @"[^\w\.@-]", "");
        }
        private void UploadEntry(string name, int score)
        {
            name = name.Substring(0, 8.Clamp(0, name.Length));
            LeaderboardCreator.UploadNewEntry(PublicKey, name, score, ((msg) =>
            {
                LoadEntries();
                GeneralManager.ResetScore();
            }));
            /*Leaderboards.Sauna_Quest_Ultra.UploadNewEntry(userNameInput.text, LeaderboardScore, IsSuccessful =>
            {
                if (IsSuccessful)
                {
                    ShowLeaderboard();
                    GeneralManager.ResetScore();
                }
            });*/
        }
    }
    #endregion
}
