using System;
using UnityEditor;
using UnityEngine;

namespace DragonGameNetworkProject.Gems
{
    public class CylinderGemGenerator: AbstractGemGenerator
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(CylinderGemGenerator))]
        class CylinderGemGeneratorEditor: GemGeneratorEditor{}

        [SerializeField] private float rMin = 10.0f;
        [SerializeField] private float rMax = 20.0f;
        [SerializeField] private float hMin = 10.0f;
        [SerializeField] private float hMax = 20.0f;
        
        public override void GenerateGems()
        {
            
        }

        private void OnDrawGizmos()
        {
            
        }
#endif
    }
}