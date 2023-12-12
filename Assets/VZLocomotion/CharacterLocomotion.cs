using System;
using System.Collections.Generic;
using DefaultNamespace.VZLocomotion;
using UnityEngine;

public abstract class ICharacterLocomotionProcessor
{
    protected Transform _transform;
    protected GameObject _go;
    protected CharacterLocomotion _loco;

    public virtual void Setup(CharacterLocomotion locomotion)
    {
        _loco = locomotion;
        _transform = locomotion.transform;
        _go = locomotion.gameObject;
    }

    public abstract void Update();

    public virtual void OnActive(params System.Object[] parameters)
    {
    }

    public virtual void OnAnimatorIK(int layerIndex)
    {
    }
}

public class CharacterLocomotion : MonoBehaviour
{
    private Dictionary<Type, ICharacterLocomotionProcessor> _processorsMap =
        new Dictionary<Type, ICharacterLocomotionProcessor>();

    private ICharacterLocomotionProcessor _currentProcessor;

    private void OnAnimatorIK(int layerIndex)
    {
        if (_currentProcessor != null)
        {
            _currentProcessor.OnAnimatorIK(layerIndex);
        }
    }

    T GetProcessor<T>() where T : ICharacterLocomotionProcessor, new()
    {
        Type processorType = typeof(T);
        if (_processorsMap.ContainsKey(processorType))
        {
            return _processorsMap.GetValueOrDefault(processorType) as T;
        }

        T newProcessor = new T();
        newProcessor.Setup(this);
        _processorsMap.Add(processorType, newProcessor);

        return newProcessor;
    }

    void Start()
    {
        SetProcessor<OnGroundProcessor>();
    }

    public void Climb(Vector3 startPosition, GameObject dragonGO)
    {
        SetProcessor<ClimbDragonProcessor>(startPosition, dragonGO);
    }

    public bool SetProcessor<T>(params System.Object[] parameters) where T: ICharacterLocomotionProcessor, new()
    {
        T processor = GetProcessor<T>();
        if (processor != null)
        {
            _currentProcessor = processor;
            _currentProcessor.OnActive(parameters);            
        }
        return false;
    }

    void Update()
    {
        if (_currentProcessor != null)
        {
            _currentProcessor.Update();
        }
    }
}