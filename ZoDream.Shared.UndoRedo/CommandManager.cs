using System.Collections.Generic;

namespace ZoDream.Shared.UndoRedo
{
    public class CommandManager : ICommandManager
    {
        private readonly Stack<IBackableCommand> _undoItems = new();
        private readonly Stack<IBackableCommand> _reverseItems = new();
        public bool IsExecuting { get; private set; }
        public event CommandStateChangedEventHandler? UndoStateChanged;
        public event CommandStateChangedEventHandler? ReverseUndoStateChanged;

        public void Execute(ICommand command)
        {
            IsExecuting = true;
            if (!command.Execute())
            {
                IsExecuting = false;
                return;
            }
            Add(command);
            IsExecuting = false;
        }

        public void Add(ICommand command)
        {
            _reverseItems.Clear();
            if (command is IBackableCommand backableCommand)
            {
                _undoItems.Push(backableCommand);
            }
            else
            {
                _undoItems.Clear();
            }
            UndoStateChanged?.Invoke(_undoItems.Count > 0);
        }

        public void ReverseUndo()
        {
            IsExecuting = true;
            var command = _reverseItems.Pop();
            if (command == null)
            {
                IsExecuting = false;
                return;
            }
            command.Execute();
            _undoItems.Push(command);
            UndoStateChanged?.Invoke(_undoItems.Count > 0);
            ReverseUndoStateChanged?.Invoke(_reverseItems.Count > 0);
            IsExecuting = false;
        }

        public void Undo()
        {
            IsExecuting = true;
            var command = _undoItems.Pop();
            if (command == null)
            {
                IsExecuting = false;
                return;
            }
            command.Undo();
            _reverseItems.Push(command);
            UndoStateChanged?.Invoke(_undoItems.Count > 0);
            ReverseUndoStateChanged?.Invoke(_reverseItems.Count > 0);
            IsExecuting = false;
        }
    }
}
