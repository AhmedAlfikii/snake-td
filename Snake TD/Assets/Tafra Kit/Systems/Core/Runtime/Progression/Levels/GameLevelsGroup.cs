using System.Collections.Generic;
using TafraKit.Internal;
using UnityEngine;

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Progression/Level/Game Levels Group", fileName = "Game Levels Group")]
    public class GameLevelsGroup : GameLevelBase
    {
        [SerializeField] private List<GameLevel> levels = new List<GameLevel>();

        public List<GameLevel> Levels => levels;

        /// <summary>
        /// Returns the latest level the player reached but haven't yet cleared. If all levels are cleared, will return null.
        /// </summary>
        /// <returns></returns>
        public GameLevel GetLatestLevel()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];

                if(!level.IsCleared)
                    return level;
            }

            return null;
        }
        /// <summary>
        /// Returns the sum of the clears for every level in this group.
        /// </summary>
        /// <returns></returns>
        public int GetTotalClears()
        { 
            int totalClears = 0;

            for(int i = 0; i < levels.Count; i++)
            {
                totalClears += levels[i].ClearsCount;
            }

            return totalClears;
        }
    }
}