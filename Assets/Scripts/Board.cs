using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Survivor
{
    public class Board : MonoBehaviour
    {
        public GameObject Player;
        public GameObject EnemyPrefab;
        public Transform EnemyParent;

        GameObject[] m_enemyPool;
        Camera m_mainCamera;
        Vector2 m_mouseDownPosition;

        public GameObject UI;
        public TextMeshProUGUI GameTimeText;

        // Start is called before the first frame update
        public void Init(Balance balance)
        {
            m_enemyPool = new GameObject[balance.NumEnemies];
            for (int i = 0; i < balance.NumEnemies; i++)
            {
                m_enemyPool[i] = Instantiate(EnemyPrefab, EnemyParent);
                m_enemyPool[i].SetActive(false);
            }

            Player.SetActive(false);

            UI.SetActive(false);
        }

        public void Show(
            GameData gameData,
            Balance balance,
            Camera mainCamera,
            float screenRatio)
        {
            m_mainCamera = mainCamera;
            Logic.StartGame(gameData, balance, mainCamera.orthographicSize, screenRatio);

            for (int i = 0; i < balance.NumEnemies; i++)
            {
                m_enemyPool[i].transform.localPosition = gameData.EnemyPosition[i];
                m_enemyPool[i].SetActive(true);
            }
            Player.SetActive(true);

            UI.SetActive(true);
        }

        public void Hide(Balance balance)
        {
            for (int i = 0; i < balance.NumEnemies; i++)
                m_enemyPool[i].SetActive(false);
            Player.SetActive(false);

            UI.SetActive(false);
        }

        // Update is called once per frame
        public void Tick(GameData gameData, Balance balance, float dt)
        {
            handleInput(gameData);

            bool gameOver;
            Logic.Tick(gameData, balance, dt, out gameOver);

            for (int i = 0; i < balance.NumEnemies; i++)
                m_enemyPool[i].transform.localPosition = gameData.EnemyPosition[i];

            Player.transform.localPosition = gameData.PlayerPosition;

            GameTimeText.text = getTimeElapsedString(gameData.GameTime);

            if (gameOver)
                Game.Instance.GameOver();
        }

        string getTimeElapsedString(float time)
        {
            string timeString = "";
            int m = Mathf.FloorToInt(time / 60.0f);
            int s = Mathf.FloorToInt(time - m * 60.0f);
            if (m >= 10)
                timeString += m;
            else
                timeString += "0" + m;
            timeString += ":";
            if (s >= 10)
                timeString += s;
            else
                timeString += "0" + s;

            return timeString;
        }

        void handleInput(GameData gameData)
        {
#if UNITY_EDITOR
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseMove = Input.GetMouseButton(0);
            bool mouseUp = Input.GetMouseButtonUp(0);
            Vector3 mousePosition = Input.mousePosition;
#else
bool mouseDown = (Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Began;
bool mouseMove = (Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Moved;
bool mouseUp = (Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled);
Vector3 mousePosition = Vector3.zero;
if (Input.touchCount > 0)
mousePosition = Input.GetTouch(0).position;
#endif
            Vector3 worldPosition = m_mainCamera.ScreenToWorldPoint(mousePosition);
            Vector2 localPos = EnemyParent.InverseTransformPoint(worldPosition);

    if (mouseDown)
        m_mouseDownPosition = localPos;
    if (mouseMove)
        Logic.MouseMove(gameData, localPos, m_mouseDownPosition);
    if (mouseUp)
        Logic.MouseUp(gameData);
        }
    }
}