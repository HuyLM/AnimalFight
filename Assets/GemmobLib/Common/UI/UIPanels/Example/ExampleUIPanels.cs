using Gemmob.Common.UI;
using UnityEngine;

namespace Gemmob.Common.UI.Example {
    /**<summary> Remove name space if using this script ingame </summary> */

    public class ExampleUIPanels : UIPanels<ExampleUIPanels> {
        public ExamplePanel ExamplePanel { get; private set; }

        /**<summary> Show with No Data </summary> */
        public void ShowExamplePanel() {
            this.Show<ExamplePanel>(null);
        }

        /**<summary> Show with a value type parameter </summary> */
        public void ShowExamplePanel(int value) {
            this.Show<ExamplePanel>(value);
        }

        /**<summary> Show with a callback </summary> */
        public void ShowExamplePanel(System.Action callback = null) {
            this.Show<ExamplePanel>(callback);
        }

        /**<summary> Show with callback use parameter </summary> */
        public void ShowExamplePanel(System.Action<int> callback = null) {
            this.Show<ExamplePanel>(callback);
        }

        /**<summary> Show with more than one parameter </summary> */
        public void ShowExamplePanel(ExamplePanel.Params parameters) {
            this.Show<ExamplePanel>(parameters);
        }

    }
}