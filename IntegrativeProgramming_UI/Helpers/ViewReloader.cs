using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeProgramming_UI
{
    internal class ViewReloader
    {
        private readonly Dictionary<string, Action> _reloadActions;

        public ViewReloader()
        {
            _reloadActions = new Dictionary<string, Action>();
        }

        public void Register(string key, Action reloadAction)
        {
            if (!_reloadActions.ContainsKey(key))
                _reloadActions[key] = reloadAction;
        }

        public void Reload(string key)
        {
            if (_reloadActions.TryGetValue(key, out var action))
            {
                action?.Invoke();
            }

        }


    }
}
