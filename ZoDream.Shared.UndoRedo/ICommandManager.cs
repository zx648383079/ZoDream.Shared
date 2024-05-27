using System;

namespace ZoDream.Shared.UndoRedo
{
    public interface ICommandManager
    {
        /// <summary>
        /// 是否是正在执行命令中
        /// </summary>
        public bool IsExecuting { get; }
        /// <summary>
        /// 执行命令，同时添加
        /// </summary>
        /// <param name="command"></param>
        public void Execute(ICommand command);

        /// <summary>
        /// 只添加不运行
        /// </summary>
        /// <param name="command"></param>
        public void Add(ICommand command);
        public void Undo();
        public void ReverseUndo(); // 反撤销

        // 以下事件可用于控制撤销与反撤销图标的启用
        public event CommandStateChangedEventHandler? UndoStateChanged;  // bool参数表明当前是否有可撤销的操作
        public event CommandStateChangedEventHandler? ReverseUndoStateChanged;  // bool参数表明当前是否有可反撤销的操作
    }
}
