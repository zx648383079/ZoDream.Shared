namespace ZoDream.Shared.UndoRedo
{
    public interface IBackableCommand: ICommand
    {
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo();
    }
}
