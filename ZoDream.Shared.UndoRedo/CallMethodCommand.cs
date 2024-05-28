using System.Linq;

namespace ZoDream.Shared.UndoRedo
{
    public class CallMethodCommand(object target, string method, object[] doParameters, object[] undoParameters) : IBackableCommand
    {
        public CallMethodCommand(object target, string method): this(target, method, [], [])
        {
            
        }

        public bool Execute()
        {
            CallMethod(doParameters);
            return true;
        }

        public void Undo()
        {
            CallMethod(undoParameters);
        }

        private void CallMethod(object[] parameters)
        {
            var methodInfo = target.GetType().GetMethod(method, parameters.Select(item => item.GetType()).ToArray());
            methodInfo?.Invoke(target, parameters);
        }
    }
}
