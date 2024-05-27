namespace ZoDream.Shared.UndoRedo
{
    public class PropertyChangedCommand(object item, string propertyName, object? oldValue, object? newValue) : IBackableCommand
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
            var propertyInfo = item.GetType().GetProperty(propertyName);
            propertyInfo?.SetValue(item, value);
        }
    }
}
