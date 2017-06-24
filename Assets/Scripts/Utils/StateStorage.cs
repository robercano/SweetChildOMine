using System.Collections;
using System.Collections.Generic;

namespace SCOM.Utils
{
    public class StateStorage
    {

        private Dictionary<string, object> _state = null;

        public StateStorage()
        {
            _state = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            set
            {
                _state[key] = value;
            }
            get
            {
                return _state[key];
            }
        }

        public string getValueAsString(string key)
        {
            return _state[key] as string;
        }

        public int? getValueAsInt(string key)
        {
            return _state[key] as int?;
        }

        public bool? getValueAsBool(string key)
        {
            return _state[key] as bool?;
        }
    }
}