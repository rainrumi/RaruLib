using UnityEngine;

/***************************************************************
* 
* 使い方
* 前提,このクラスはパネルの切り替えを行います。シーンが切り替わると消滅します。
* 
* １,クラスを任意シーンのオブジェにアタッチ
* ２,パネル分のデータをインスペクタ上でpanelDatasにアタッチ
* ３,Button等のクリック時に、panelDatasの配列番号を入れた引数で呼び出しメソッド(コードの一番下)を呼び出す
* 
* 追記,イベント発生時のパネル切り替えをカスタムする場合は任意配列番号のPanelEventにサブスクライブしてください
* 
* ************************************************************/

namespace RaruLib
{
    public enum PanelKind
    {
        Main,
        Start,
        End,
        Config,
        StageSelect,
        Menu,
        MAX
    }

    public class ScenePanel : MonoBehaviour
    {
        public static ScenePanel instance;
        public delegate void PanelEvent();  // パネルイベント

        [System.Serializable]
        public class PanelData                      // パネルデータ
        {
            public GameObject panel;                // パネルオブジェクト
            public event PanelEvent onOpenPanel;    // パネルを表示
            public event PanelEvent onClosePanel;   // パネルを非表示

            public void CallOpenPanel() { onOpenPanel?.Invoke(); }
            public void CallClosePanel() { onClosePanel?.Invoke(); }

            public void OpenPanel() { if (panel == null) { return; } panel.SetActive(true); }
            public void ClosePanel() { if (panel == null){ return; } panel.SetActive(false); }
        }

        [SerializeField,Header("内部enumの順に入れる")]
        public PanelData[] panelDatas = new PanelData[(int)PanelKind.MAX];  // イベント量に応じたイベント配列

        /***************************************************************
         * 初期化
         * ************************************************************/
        private void Awake()
        {
            if (instance == null) instance = this;
            else { Debug.Log($"重複した{instance}を削除します"); Destroy(instance.gameObject); }

            //イベントの設定
            for (int i = 0; i < panelDatas.Length; i++)
            {
                for (int j = 0; j < panelDatas.Length; j++)
                {
                    // 自身のパネルを表示したとき、他のパネルを非表示にする
                    if (i != j) { panelDatas[i].onOpenPanel += panelDatas[j].ClosePanel; continue; }
                    panelDatas[i].onOpenPanel += panelDatas[j].OpenPanel;
                    panelDatas[i].onClosePanel += panelDatas[j].ClosePanel;
                }
            }
        }

        /***************************************************************
         * 呼び出しメソッド
         * ************************************************************/
        public void DataCallOpenPanel(PanelKind panel)
        {
            if ((int)panel >= panelDatas.Length) { Debug.Log("存在しない要素はアクセスできません"); return; }
            panelDatas[(int)panel].CallOpenPanel();
        }
    }
}