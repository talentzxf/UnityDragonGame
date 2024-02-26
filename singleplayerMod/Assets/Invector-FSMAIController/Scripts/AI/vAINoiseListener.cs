using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Invector.vCharacterController.AI
{
    /// <summary>
    /// Is a <seealso cref="vIAIComponent"/> use with the vNoiseObject to hear a noise, you can also create custom noises 
    /// </summary>
    [DisallowMultipleComponent]
    [vClassHeader("AI Noise Listener")]
    public class vAINoiseListener : vMonoBehaviour, vIAIComponent
    {
        [vHelpBox("The noise has a radius effect and the noise volume decreases depending on the distance, 'Listener Power'  will applify the distance of the noise to listener"), Range(0f, 10f)]
        public float listenerPower = 1;

        public bool debugMode;

        public List<string> ignoreNoiseType;

        public virtual Type ComponentType
        {
            get
            {
                return typeof(vAINoiseListener);
            }
        }

        protected virtual void OnEnable()
        {
            if (vAINoiseManager.Instance != null) vAINoiseManager.Instance.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            try
            {
                if (!this.gameObject.scene.isLoaded) return;
                if (vAINoiseManager.Instance != null) vAINoiseManager.Instance.RemoveListener(this);
            }
            catch { }
        }

        protected virtual void OnDestroy()
        {
            try
            {
                if (!this.gameObject.scene.isLoaded) return;
                if (vAINoiseManager.Instance != null) vAINoiseManager.Instance.RemoveListener(this);
            }
            catch { }
        }

        protected virtual bool IsInListenerPower(vNoise noise)
        {
            if (ignoreNoiseType.Contains(noise.noiseType)) return false;
            return NoiseVolume(noise) > 0;
        }        
      
        protected virtual List<vNoise> SortByDistance()
        {          
            if (ListenerdNoises.Count > 1)
                ListenerdNoises.Sort(delegate (vNoise noiseA, vNoise noiseB)
                {
                    return Vector3.Distance(transform.position, noiseA.position).CompareTo
                                ((Vector3.Distance(transform.position, noiseB.position)));
                });
            if (ListenerdNoises.Count > 0)
                LastListenedNoise = ListenerdNoises[0];
            return ListenerdNoises;
        }

        protected virtual List<vNoise> GetNoiseByType(string noiseType)
        {        
            var noisesFromType = ListenerdNoises.FindAll(n => n.noiseType.Equals(noiseType));
           
            if (noisesFromType.Count > 0)
                LastListenedNoise = noisesFromType[0];
            return noisesFromType;
        }

        protected virtual List<vNoise> GetNoiseByType(List<string> noiseTypes)
        {         
            var noisesFromType = ListenerdNoises.FindAll(n =>noiseTypes.Contains(n.noiseType));          
            if (noisesFromType.Count > 0)
                LastListenedNoise = noisesFromType[0];
            return noisesFromType;
        }

        public virtual List<vNoise> ListenerdNoises { get { if (_ListenerdNoises == null) _ListenerdNoises = new List<vNoise>();return _ListenerdNoises; } protected set { _ListenerdNoises = value; } }

        protected List<vNoise> _ListenerdNoises;

        /// <summary>
        /// Target Noise is automatically when use Any Get noise Methods"/>
        /// </summary>
        public virtual vNoise LastListenedNoise { get; protected set; }

        /// <summary>
        /// Get noise volume relative to <seealso cref="listenerPower"/>
        /// </summary>
        /// <param name="noise">noise to check</param>
        /// <returns></returns>
        public virtual float NoiseVolume(vNoise noise)
        {
            var progress = 0f;
            if (listenerPower > 0)
            {
                var minDistance = noise.minDistance * listenerPower;
                var maxDistance = noise.maxDistance * listenerPower;
                var relativeDistance = Vector3.Distance(noise.position, transform.position) - minDistance;
                progress = 1f - (relativeDistance / (minDistance == maxDistance ? maxDistance : minDistance > maxDistance ? minDistance - maxDistance : maxDistance - minDistance));
            }
            return noise.volume * progress;
        }

        /// <summary>
        /// Check if is listening any noise
        /// </summary>    
        /// <returns>Return true if is listening any noise</returns>
        public virtual bool IsListeningNoise()
        {
            if (ListenerdNoises == null) ListenerdNoises = new List<vNoise>();
            return ListenerdNoises.Count > 0;
        }

        /// <summary>
        /// Check if is listening any noise
        /// </summary>    
        /// <returns>Return true if is listening any noise</returns>
        public virtual bool IsListeningNoise(out vNoise noise)
        {
            var noises = SortByDistance();
            if (noises.Count > 0)
            {
                noise = noises[0];
                return true;
            }
            noise = null;
            return false;           
        }

        /// <summary>
        /// Check if is listening  specific noises
        /// </summary>      
        /// <param name="noiseTypes">types of noises to check</param>
        /// <returns>Return true if is listening any noise in the list of types</returns>
        public virtual bool IsListeningSpecificNoises(List<string> noiseTypes)
        {
            return GetNoiseByType(noiseTypes).Count > 0;
        }

        /// <summary>
        /// Check if is listening  specific noises
        /// </summary>      
        /// <param name="noiseTypes">types of noises to check</param>
        /// <returns>Return true if is listening any noise in the list of types</returns>
        public virtual bool IsListeningSpecificNoises(List<string> noiseTypes,out vNoise noise)
        {
            var noises = GetNoiseByType(noiseTypes);
            if(noises.Count>0)
            {
                noise = noises[0];
                return true;
            }
            noise = null;
            return false;
        }

        /// <summary>
        /// Get near noise if is listening any noise"/>
        /// </summary>      
        /// <returns>Ner Noise</returns>
        public virtual vNoise GetNearNoise()
        {
            var noisesByDistance = SortByDistance();
            if (noisesByDistance.Count > 0)
                return ListenerdNoises[0];
            else return null;
        }

        /// <summary>
        /// Get noise by type if is listening a specific noise"/>
        /// </summary>       
        /// <param name="noiseType">type of noise to get</param>
        /// <returns>Near Noise </returns>
        public virtual vNoise GetNearNoiseByType(string noiseType)
        {
            var noisesByType = GetNoiseByType(noiseType);
            if (noisesByType.Count > 0)
                return ListenerdNoises[0];
            else return null;
        }
        /// <summary>
        /// Get near noise by types if is listening a specific noise"/>
        /// </summary>      
        /// <param name="noiseTypes">types of noises to get</param>
        /// <returns>Near noise</returns>
        public virtual vNoise GetNearNoiseByTypes(List<string> noiseTypes)
        {
            var noisesByType = GetNoiseByType(noiseTypes);
            if (noisesByType.Count > 0)
                return ListenerdNoises[0];
            else return null;
        }

        /// <summary>
        /// Get noises by type if is listening  specific noises"/>
        /// </summary>       
        /// <param name="noiseTypes">types of noises to get</param>
        /// <param name="sortByDistance">sort list by noises distance</param>
        /// <returns>List of Noises that can be Listener</returns>
        public virtual List<vNoise> GetNoiseByTypes(List<string> noiseTypes)
        {
            var noisesByType = GetNoiseByType(noiseTypes);
            if (noisesByType.Count > 0)
            {
                return ListenerdNoises;
            }
               
            else return null;
        }

        /// <summary>
        /// Check if The Listener is the closest listener hearing a noise
        /// </summary>
        /// <param name="noise">Target noise</param>
        /// <returns></returns>
        public virtual bool IsClosestListenerToNoise(vNoise noise)
        {
            if (vAINoiseManager.Instance == null) return false;
            var listenersWithNoise = vAINoiseManager.Instance.noiseListeners.FindAll(l => l.ListenerdNoises.Contains(noise));

            listenersWithNoise = listenersWithNoise.OrderBy(l => (l.transform.position - noise.position).magnitude).ToList();
            return listenersWithNoise[0].Equals(this);
        }

        /// <summary>
        /// Add Noise
        /// </summary>
        /// <param name="noise"></param>
        public virtual void AddNoise(vNoise noise)
        {
            if (ListenerdNoises == null) ListenerdNoises = new List<vNoise>();
            if (! IsInListenerPower(noise)) return;
            if (ListenerdNoises.Contains(noise) )
            {
                ListenerdNoises[ListenerdNoises.IndexOf(noise)].AddDuration(noise.duration);
                ListenerdNoises = SortByDistance();
            }
            else
            {
                noise.onFinishNoise.AddListener(RemoveNoise);
                ListenerdNoises.Add(noise);
                ListenerdNoises = SortByDistance();
            }          
        }

        /// <summary>
        /// Remove noise
        /// </summary>
        /// <param name="noise"></param>
        public virtual void RemoveNoise(vNoise noise)
        {
            if (ListenerdNoises == null) ListenerdNoises = new List<vNoise>();
            if (ListenerdNoises.Contains(noise))
            {
                ListenerdNoises.Remove(noise);
                ListenerdNoises = SortByDistance();
            }
        }
    }
}