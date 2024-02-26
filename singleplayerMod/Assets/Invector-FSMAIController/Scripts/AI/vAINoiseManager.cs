using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    public class vAINoiseManager : MonoBehaviour
    {
        static vAINoiseManager _instance;
        public static vAINoiseManager Instance
        {
            get
            {
              
                if (_instance == null) _instance = FindObjectOfType<vAINoiseManager>();
                if (_instance == null)
                {
                    var noiseManager = new GameObject("AI Noise Manager");
                    var newInstance = noiseManager.AddComponent<vAINoiseManager>();
                    newInstance.noises = new List<vNoise>();
                    _instance = newInstance;
                }              
                return _instance;
            }
        }

        public delegate void NoiseOperator(vNoise noise);

        public event NoiseOperator OnAddNoise;
        public event NoiseOperator OnRemoveNoise;
        /// <summary>
        /// List of all noises that is listening
        /// </summary>
        public  List<vNoise> noises;// { get; protected set; }

        public List<vAINoiseListener> noiseListeners = new List<vAINoiseListener>();

        public void AddListener(vAINoiseListener listener)
        {
            if (!noiseListeners.Contains(listener))
            {
                OnAddNoise += listener.AddNoise;
                OnRemoveNoise += listener.RemoveNoise;
                noiseListeners.Add(listener);
            }
        }

        public void RemoveListener(vAINoiseListener listener)
        {
            if(noiseListeners.Contains(listener))
            {
                OnAddNoise -= listener.AddNoise;
                OnRemoveNoise -= listener.RemoveNoise;
                noiseListeners.Remove(listener);
            }
        }

        public void AddNoise(vNoise noise)
        {
            if (noises==null) noises = new List<vNoise>();
            
            if (noises.Contains(noise))
            {               
                noises[noises.IndexOf(noise)].AddDuration(noise.duration);
               
            }
            else
            {              
                noise.onFinishNoise.AddListener(RemoveNoise);
                noises.Add(noise);
                OnAddNoise?.Invoke(noise);
            }
            if (!noise.isPlaying) StartCoroutine(noise.Play());
        }

        public void RemoveNoise(vNoise noise)
        {
            if (noises == null) noises = new List<vNoise>();
            if (noises.Contains(noise))
            {
                OnRemoveNoise?.Invoke(noise);
                noises.Remove(noise);
            }         
        }     

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}