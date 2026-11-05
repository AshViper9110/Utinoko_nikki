using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualProgramming
{
    public enum VisualBlockType
    {
        Move,
        Rotate,
        SetPosition,
        SetVisible,
        SetScale,
        Wait,
        DebugLog
    }

    [Serializable]
    public class VisualBlockData
    {
        public VisualBlockType type;
        public float x;
        public float y;
        public float z;
        public float duration = 1f;
        public string message = "Hello Unity!";
        public bool visible = true;
    }

    [Serializable]
    public class VisualProgramData
    {
        public string targetObjectName = "Cube";
        public List<VisualBlockData> blocks = new List<VisualBlockData>();
    }
}
