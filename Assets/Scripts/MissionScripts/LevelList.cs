using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

/* Copyright (c) [2023] [Lizhneghe.Chen https://github.com/Lizhenghe-Chen]
* Please do not use these code directly without permission.
*/
public class LevelList : MonoBehaviour
{
    [System.Serializable]
    public struct Level
    {
        public string levelName;
        public int levelIndex;
    }
    public Level StartMenu;//start menu
    public Level[] List;

    public Level Level_1;
    public Level Level_2;
    public Level Level_3;
    public Level Level_4;


}
