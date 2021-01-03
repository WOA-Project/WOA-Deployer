using System;
using System.Runtime.CompilerServices;
using Deployer.Core.DeploymentLibrary.Utils.LazyTask;

namespace Deployer.Core.DeploymentLibrary.Utils
{
    public class LazyTaskMethodBuilder<T>
    {
        public LazyTaskMethodBuilder() => this.Task = new LazyTask<T>();

        public static LazyTaskMethodBuilder<T> Create() => new LazyTaskMethodBuilder<T>();

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            //instead of stateMachine.MoveNext();
            this.Task.SetStateMachine(stateMachine);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine) { }

        public void SetException(Exception exception) => this.Task.SetException(exception);

        public void SetResult(T result) => this.Task.SetResult(result);

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
            =>
                this.GenericAwaitOnCompleted(ref awaiter, ref stateMachine);

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
            =>
                this.GenericAwaitOnCompleted(ref awaiter, ref stateMachine);

        public void GenericAwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public LazyTask<T> Task { get; }
    }
}