using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Sentient
{
    public NavMeshAgent agent;
    public Animator animator;

    public bool reachedDestination;

    public NPCActionController actionController;
    public NPCActionGraph startingGraph;

    protected override void InternalAwake()
    {
        base.InternalAwake();
        actionController.Init(this);
        actionController.ExecuteGraph(startingGraph);
    }

    protected override void InternalUpdate()
    {
        base.InternalUpdate();
        reachedDestination = agent.remainingDistance < 0.1f;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}

[Serializable]
public class NPCActionController
{
    private NPCActionGraph currentGraph;
    private NPC npc;

    private Coroutine executionRoutine = null;
    private CancellationTokenSource tokenSource;

    public void Init(NPC _npc)
    {
        npc = _npc;
    }
    
    public void ExecuteGraph(NPCActionGraph graph)
    {
        currentGraph = graph;
        
        if (executionRoutine != null)
        {
            tokenSource.Cancel();
            npc.StopCoroutine(executionRoutine);
        }

        executionRoutine = npc.StartCoroutine(ExecutionRoutine());
    }

    private IEnumerator ExecutionRoutine()
    {
        tokenSource = new();
        NPCAction.ExecutionContext context = new NPCAction.ExecutionContext(npc, tokenSource.Token);
        foreach (var action in currentGraph.actions)
        {
            action.Execute(context);
            yield return new WaitUntil(() => action.State == NPCAction.ActionState.Complete);
        }
        yield break;
    }
}

[Serializable]
public class NPCActionGraph
{
    [SerializeReference]
    public List<NPCAction> actions = new();
}

[Serializable]
public abstract class NPCAction
{
    public class ExecutionContext
    {
        public NPC npc;
        public CancellationToken cancellationToken;
        
        public ExecutionContext(NPC _npc, CancellationToken _cancellationToken)
        {
            npc = _npc;
            cancellationToken = _cancellationToken;
        }
    }
    
    public enum ActionState
    {
        Idle,
        Running,
        Complete
    }

    public ActionState State;
    public ExecutionContext Context;
    
    public void Execute(ExecutionContext context)
    {
        Context = context;
        if (State == ActionState.Running)
        {
            Debug.LogError($"Action {GetType().FullName} is already running. Aborting execution");
        }
        InternalExecute();
    }

    protected virtual void InternalExecute()
    {
        State = ActionState.Running;
    }

    protected void Complete()
    {
        State = ActionState.Complete;
    }

}

[Serializable]
public class NA_MoveToPosition : NPCAction
{
    public Vector3 position;
    public float stopDistance;
    public bool stopOnArrival;
    
    protected override void InternalExecute()
    {
        base.InternalExecute();
        Context.npc.StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        var agent = Context.npc.agent;
        agent.SetDestination(position);
        yield return new WaitUntil(() => agent.remainingDistance < stopDistance ||
                                   Context.cancellationToken.IsCancellationRequested);
        if (stopOnArrival)
        {
            agent.ResetPath();
            Context.npc.animator.SetFloat("Speed", 0);
        }
        Complete();
    }
}

[Serializable]
public class NA_MoveToTransform : NPCAction
{
    public Transform targetTransform;
    public float stopDistance;
    public bool stopOnArrival;
    
    protected override void InternalExecute()
    {
        base.InternalExecute();
        Context.npc.StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        var agent = Context.npc.agent;
        agent.SetDestination(targetTransform.position);
        yield return new WaitUntil(() => agent.remainingDistance < stopDistance ||
                                         Context.cancellationToken.IsCancellationRequested);
        if (stopOnArrival)
        {
            agent.ResetPath();
            Context.npc.animator.SetFloat("Speed", 0);
        }
        Complete();
    }
}
