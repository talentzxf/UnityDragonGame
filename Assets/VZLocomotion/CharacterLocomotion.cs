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
    private OnGroundProcessor _onGroundProcessor;
    private ClimbDragonProcessor _climbDragonProcessor;

    private List<ICharacterLocomotionProcessor> _processors;
    private ICharacterLocomotionProcessor _currentProcessor;

    private void OnAnimatorIK(int layerIndex)
    {
        if (_currentProcessor != null)
        {
            _currentProcessor.OnAnimatorIK(layerIndex);
        }
    }

    void SetupProcessor<T>(T processor) where T : ICharacterLocomotionProcessor
    {
        processor.Setup(this);
        _processors.Add(processor);
    }

    void Start()
    {
        SetupProcessor(_onGroundProcessor);
        SetupProcessor(_climbDragonProcessor);

        SetProcessor(typeof(OnGroundProcessor));
    }

    public void Climb(Vector3 startPosition, GameObject dragonGO)
    {
        SetProcessor(typeof(ClimbDragonProcessor), startPosition, dragonGO);
    }

    public bool SetProcessor(Type processorType, params System.Object[] parameters)
    {
        foreach (var _processor in _processors)
        {
            if (_processor.GetType() == processorType)
            {
                _currentProcessor = _processor;
                _currentProcessor.OnActive(parameters);
                return true;
            }
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