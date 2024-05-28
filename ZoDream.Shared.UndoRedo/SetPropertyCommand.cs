namespace ZoDream.Shared.UndoRedo
{
    public class SetPropertyCommand(object target, string propertyName, object? oldValue, object? newValue) : IBackableCommand
    {
        public void Undo()
        {
            SetPropertyValue(oldValue);
        }

        public bool Execute()
        {
            SetPropertyValue(newValue);
            return true;
        }

        private void SetPropertyValue(object? value)
        {
            var propertyInfo = target.GetType().GetProperty(propertyName);
            propertyInfo?.SetValue(target, value);
        }
    }
}
