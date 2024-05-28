using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace ZoDream.Shared.UndoRedo
{
    public class PropertyChangeListener<T>(ICommandManager manager) where T : class, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private readonly List<T> _items = [];

        private string? _currentItemPropertyName;
        private object? _currentItemChangingPropertyValue;
        private object? _currentItemChanging;

        public void Subscribe(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.PropertyChanging += Item_PropertyChanging;
            item.PropertyChanged += Item_PropertyChanged;

            _items.Add(item);
        }

        public void Unsubscribe(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_items.Remove(item))
            {
                item.PropertyChanging -= Item_PropertyChanging;
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            Debug.Assert(_items.Contains((T)sender));

            if (!manager.IsExecuting)
            {
                _currentItemChanging = sender;
                _currentItemPropertyName = e.PropertyName;
                _currentItemChangingPropertyValue = GetPropertyValue(sender, e.PropertyName);
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Expect the property whose value is changing and the object it is changing on to be the
            // same between PropertyChanging and PropertyChanged events
            var isValid = _currentItemChanging == sender && _currentItemPropertyName == e.PropertyName;
            if (!isValid)
            {
                var exception = new InvalidOperationException(
                    $"The {nameof(PropertyChangeListener<T>)} handled a PropertyChanged event without first receiving a " +
                    $"PropertyChanging event for the property '{e.PropertyName}' on '{sender.GetType().FullName}'");
                Debug.WriteLine(exception.Message);
                // Don't thrown an exception in the event handler, pass the exception to the UndoRedoService
                // manager.OnError(exception);
            }
            else if (!manager.IsExecuting)
            {
                var newValue = GetPropertyValue(sender, e.PropertyName);
                manager.Add(new SetPropertyCommand(sender, e.PropertyName, _currentItemChangingPropertyValue, newValue));
            }

            _currentItemChanging = default;
            _currentItemPropertyName = default;
            _currentItemChangingPropertyValue = default;
        }

        private static object? GetPropertyValue(object sender, string propertyName)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            var propertyInfo = sender.GetType().GetProperty(propertyName);
            return propertyInfo?.GetValue(sender, null);
        }
    }
}
