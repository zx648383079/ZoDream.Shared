using System;
using System.Collections.Generic;

namespace ZoDream.Shared.UndoRedo
{
    /// <summary>
    /// 批量执行事务
    /// </summary>
    public class TransactionCommand(IList<IBackableCommand> commandItems) : IBackableCommand
    {

        private readonly List<int> _failureIndex = [];
        public bool IsExecuting { get; private set; }

        public void Commit()
        {
            IsExecuting = true;
            _failureIndex.Clear();
            for (var i = 0; i < commandItems.Count; i++)
            {

                if (!commandItems[i].Execute())
                {
                    _failureIndex.Add(i);
                }
            }
            IsExecuting = false;
        }

        public void Rollback()
        {
            IsExecuting = true;
            for (var i = 0; i < commandItems.Count; i++)
            {

                if (_failureIndex.Contains(i))
                {
                    continue;
                }
                commandItems[i].Undo();
            }
            IsExecuting = false;
        }

        public bool Execute()
        {
            try
            {
                Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Undo()
        {
            Rollback();
        }
    }
}
