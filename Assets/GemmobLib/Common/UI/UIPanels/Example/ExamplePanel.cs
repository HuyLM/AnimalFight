using Gemmob.Common.UI;

namespace Gemmob.Common.UI.Example {
    /**<summary> Remove name space if using this script ingame </summary> */

    public class ExamplePanel : Panel {
        public override void Show(object data = null, bool animated = false) {
            base.Show(data, animated);

            // if no data
            ShowWithNoData();

            //if data is a value type, example data is int type
            ShowWithParam(data);

            // if data is delegate / anomyous function
            ShowWithCallback(data);

            // if data is delegate / anomyous function with a parameter
            ShowWithCallbackParam(data);

            // if need to send more than one parameter
            ShowWithMultiParams(data);
        }

        private void ShowWithNoData() {
            // do something
        }

        private void ShowWithParam(object data) {
            int a = (int)data;
            // do something
        }

        private void ShowWithCallback(object data) {
            // do something
            var callback = data as System.Action;
            if (callback != null) callback.Invoke();
        }

        private void ShowWithCallbackParam(object data) {
            // do something
            int value = 0;
            var callback = data as System.Action<int>;
            if (callback != null) callback.Invoke(value);
        }

        public void ShowWithMultiParams(object data) {
            Params param = data as Params;
            // do something
        }

        /** <summary> Declare this class if need more than one params as more value type </summary> */
        public class Params {
            public int variables;
            public string msg;
            public System.Action callback;

            public Params(int param, string msg, System.Action callback) {
                this.variables = param;
                this.msg = msg;
                this.callback = callback;
            }
        }
    }
}