using UnityEngine;

namespace Survivor
{
    public enum MENU_STATE { NONE, IN_GAME, GAME_OVER };

    public class GameData
    {
        public Vector2[] EnemyPosition;
        public Vector2[] EnemyDirection;
        public float[] EnemyVelocity;

        public Vector2 PlayerPosition;
        public Vector2 PlayerDirection;

        public Vector2 BoardBounds;

        public float GameTime;
        public float BestTime;

        public MENU_STATE GameState = MENU_STATE.NONE;
    }
}