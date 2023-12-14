using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

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

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnActive(params Object[] parameters)
    {
    }

    public virtual void OnAnimatorIK(int layerIndex)
    {
    }
}

public abstract class AnimatedCharactorLocomotionProcessor : ICharacterLocomotionProcessor
{
    protected Animator _animator;

    public override void Setup(CharacterLocomotion locomotion)
    {
        base.Setup(locomotion);
        _animator = _go.GetComponent<Animator>();
    }
}

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private String startProcessorName = "OnGroundProcessor";

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
        Type type = Type.GetType(startProcessorName);
        var setProcessorMethod = GetType().GetMethod("SetProcessor");
        var genericMethod = setProcessorMethod.MakeGenericMethod(type);
        genericMethod.Invoke(this, new object[] {null});
    }

    public bool SetProcessor<T>(params Object[] parameters) where T : ICharacterLocomotionProcessor, new()
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

    private void FixedUpdate()
    {
        if (_currentProcessor != null)
        {
            _currentProcessor.FixedUpdate();
        }
    }
}