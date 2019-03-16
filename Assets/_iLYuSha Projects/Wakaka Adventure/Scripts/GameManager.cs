/* * * * * * * * * * * * * * * * * * * * *
 * 
 *    Title: "哇咔咔大冒險"
 * 
 *    Dsecription:
 *                  功能: 遊戲管理器
 *                   1. 遊戲模式管理（章節、自定義、密室逃脫）
 *                   2. 遊戲場景列表管理
 *                   3. 遊戲難度列表管理
 * 
 *     Author: iLYuSha
 *     
 *     Date: 2018.03.24
 *     
 *     Modify:
 *                  03.24 修改: 
 *                   1. 繼承自Singleton
 *     
 * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;

namespace WakakaAdventureSpace
{
    public enum GameMode
    {
        Ready = 0, Campaign = 1, Custom = 2, Escape = 3,
    }
    public enum Theme
    {
        Inca = 1, Gobi = 2, KocmocA = 3,
    }
    public enum ThemeCht
    {
        印加秘境 = 1, 戈壁秘境 = 2, 卡斯摩沙 = 3,
    }
    public enum Difficulty
    {
        Doctor = 0, Newbie = 1, Trainee = 2, Elite = 3, Expert = 4, Master = 5, Crazy = 6, Wakaka = 7,
    }
    public enum DifficultyCht
    {
        博士 = 0, 萌新 = 1, 學徒 = 2, 菁英 = 3, 磚家 = 4, 大師 = 5, 瘋王 = 6, 哇咔咔 = 7,
    }

    public partial class GameManager : Singleton<GameManager>
    {
        public GameMode gameMode;

        public void SetGameMode(int indexGameMode)
        {
            gameMode = (GameMode)indexGameMode;
        }
    }
}