using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private string initState; // e.g., "PatrolState"
    [SerializeField] private FSMState[] states;

    public FSMState CurrentState { get; private set; }
    public Transform Player { get; set; }

    private bool isFrozen = false;

    private void Start()
    {
        ChangeState(initState);
    }

    private void Update()
    {
        if (isFrozen) return;
        CurrentState?.UpdateState(this);
    }

    public void ChangeState(string newStateID)
    {
        FSMState newState = GetState(newStateID);
        if (newState == null) return;
        CurrentState = newState;
    }

    private FSMState GetState(string stateID)
    {
        foreach (var state in states)
        {
            if (state.ID == stateID)
                return state;
        }
        return null;
    }

    // === Freeze Control ===
    public void Freeze()
    {
        isFrozen = true;
    }

    public void Unfreeze()
    {
        isFrozen = false;
    }

    public bool IsFrozen => isFrozen;
}
