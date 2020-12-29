//ActionCache 缓存并重用 Action，重用时可修改调用参数，避免每次用(含上值的) lamdba 创建新 action
//action 调用完成时，将回收 action，因此生成的 action 不可调用多次，同时尽量保证生成 action 会被调用一次
//
// 例：
// 运用 ActionCache 前：
// Int32 context;
// //lamdba 有上值，每次调用需生成新的 action 对象
// callAsync((String arg)=>onFinish(arg, context));
//
// 运用 ActionCache 后：
// struct Context { public Int32 context; }
// Int32 context;
// //lamdba 无上值，每次调用不会生成新的 action 对象
// callAsync(ActionCache<String>.Generate(new Context{context=context}, (arg, extraArg)=>onFinish(arg, extraArg.context));
//
using System;
using System.Collections.Generic;

namespace Best
{
    /// <summary>
    /// 对应 Action
    /// </summary>
    public class ActionCache
    {
        /// <summary>
        /// 生成 Action
        /// </summary>
        /// <typeparam name="TExtra">额外参数类型，额外参数是重用时可修改的参数</typeparam>
        /// <param name="argExtra">额外参数</param>
        /// <param name="executor">完成实际操作</param>
        /// <returns></returns>
        public static Action Generate<TExtra>(TExtra argExtra, Action<TExtra> executor)
        {
            ActionState<TExtra> actionState;
            if (ActionState<TExtra>.stack.Count == 0)   //无缓存，新建 action
            {
                actionState = new ActionState<TExtra>();
                ActionState<TExtra> state = actionState;    //避免 lamdba 引用公用局部变量
                state.action = () =>
                {
                    state.executor(state.argExtra);

                //调用完成，回收 action 以重用
                ActionState<TExtra>.stack.Push(state);
                    return;
                };
            }
            else
            {
                actionState = ActionState<TExtra>.stack.Pop();
            }

            //更新参数
            actionState.executor = executor;
            actionState.argExtra = argExtra;

            return actionState.action;
        }

        private class ActionState<TExtra>
        {
            public Action action;   //保存生成的 action 自身
            public Action<TExtra> executor;
            public TExtra argExtra;
            public static Stack<ActionState<TExtra>> stack = new Stack<ActionState<TExtra>>();
        }
    }

    /// <summary>
    /// 对应 Action<T1>
    /// </summary>
    public class ActionCache<T1>
    {
        /// <summary>
        /// 生成 Action
        /// </summary>
        /// <typeparam name="TExtra">额外参数类型，额外参数是重用时可修改的参数</typeparam>
        /// <param name="argExtra">额外参数</param>
        /// <param name="executor">完成实际操作</param>
        /// <returns></returns>
        public static Action<T1> Generate<TExtra>(TExtra argExtra, Action<T1, TExtra> executor)
        {
            ActionState<TExtra> actionState;
            if (ActionState<TExtra>.stack.Count == 0)   //无缓存，新建 action
            {
                actionState = new ActionState<TExtra>();
                ActionState<TExtra> state = actionState;    //避免 lamdba 引用公用局部变量
                state.action = (arg1) =>
                {
                    state.executor(arg1, state.argExtra);

                //调用完成，回收 action 以重用
                ActionState<TExtra>.stack.Push(state);
                    return;
                };
            }
            else
            {
                actionState = ActionState<TExtra>.stack.Pop();
            }

            //更新参数
            actionState.executor = executor;
            actionState.argExtra = argExtra;

            return actionState.action;
        }

        private class ActionState<TExtra>
        {
            public Action<T1> action;   //保存生成的 action 自身
            public Action<T1, TExtra> executor;
            public TExtra argExtra;
            public static Stack<ActionState<TExtra>> stack = new Stack<ActionState<TExtra>>();
        }
    }

    /// <summary>
    /// 对应 Action<T1, T2>
    /// </summary>
    public class ActionCache<T1, T2>
    {
        /// <summary>
        /// 生成 Action
        /// </summary>
        /// <typeparam name="TExtra">额外参数类型，额外参数是重用时可修改的参数</typeparam>
        /// <param name="argExtra">额外参数</param>
        /// <param name="executor">完成实际操作</param>
        /// <returns></returns>
        public static Action<T1, T2> Generate<TExtra>(TExtra argExtra, Action<T1, T2, TExtra> executor)
        {
            ActionState<TExtra> actionState;
            if (ActionState<TExtra>.stack.Count == 0)   //无缓存，新建 action
            {
                actionState = new ActionState<TExtra>();
                ActionState<TExtra> state = actionState;    //避免 lamdba 引用公用局部变量
                state.action = (arg1, arg2) =>
                {
                    state.executor(arg1, arg2, state.argExtra);

                //调用完成，回收 action 以重用
                ActionState<TExtra>.stack.Push(state);
                    return;
                };
            }
            else
            {
                actionState = ActionState<TExtra>.stack.Pop();
            }

            //更新参数
            actionState.executor = executor;
            actionState.argExtra = argExtra;

            return actionState.action;
        }

        private class ActionState<TExtra>
        {
            public Action<T1, T2> action;   //保存生成的 action 自身
            public Action<T1, T2, TExtra> executor;
            public TExtra argExtra;
            public static Stack<ActionState<TExtra>> stack = new Stack<ActionState<TExtra>>();
        }
    }
    /// <summary>
    /// 对应 Action<T1, T2, T3>
    /// </summary>
    public class ActionCache<T1, T2, T3>
    {
        /// <summary>
        /// 生成 Action
        /// </summary>
        /// <typeparam name="TExtra">额外参数类型，额外参数是重用时可修改的参数</typeparam>
        /// <param name="argExtra">额外参数</param>
        /// <param name="executor">完成实际操作</param>
        /// <returns></returns>
        public static Action<T1, T2, T3> Generate<TExtra>(TExtra argExtra, Action<T1, T2, T3, TExtra> executor)
        {
            ActionState<TExtra> actionState;
            if (ActionState<TExtra>.stack.Count == 0)   //无缓存，新建 action
            {
                actionState = new ActionState<TExtra>();
                ActionState<TExtra> state = actionState;    //避免 lamdba 引用公用局部变量
                state.action = (arg1, arg2, arg3) =>
                {
                    state.executor(arg1, arg2, arg3, state.argExtra);

                //调用完成，回收 action 以重用
                ActionState<TExtra>.stack.Push(state);
                    return;
                };
            }
            else
            {
                actionState = ActionState<TExtra>.stack.Pop();
            }

            //更新参数
            actionState.executor = executor;
            actionState.argExtra = argExtra;

            return actionState.action;
        }

        private class ActionState<TExtra>
        {
            public Action<T1, T2, T3> action;   //保存生成的 action 自身
            public Action<T1, T2, T3, TExtra> executor;
            public TExtra argExtra;
            public static Stack<ActionState<TExtra>> stack = new Stack<ActionState<TExtra>>();
        }
    }
}
