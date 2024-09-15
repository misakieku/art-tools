using UnityEngine;

namespace Misaki.ArtToolEditor
{
    public class SourcePrefab
    {
        public bool Enabled = true;
        public Texture2D Icon;
        public GameObject Prefab;

        public float Frequency = 1.0f;
        public AlignmentType Alignment;
        public ParentType Parent;
        public GameObject ParentObject;
        public float ObjectSpacing = 1.0f;
        public ReferencePointType ReferencePoint;
        public float SlopeFilter = 45;

        public RandomnessType Randomness;
        public Vector3 PositionMin;
        public Vector3 PositionMax;
        public Vector3 RotationMin;
        public Vector3 RotationMax;
        public Vector3 ScaleMin = Vector3.one;
        public Vector3 ScaleMax = Vector3.one;
    }
}