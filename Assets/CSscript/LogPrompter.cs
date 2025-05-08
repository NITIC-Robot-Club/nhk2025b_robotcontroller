using UnityEngine;
using System;
using System.Text;
using TMPro;

namespace Udonba
{
    /// <summary>
    /// Debug.LogをTextMeshProに表示
    /// </summary>
    public class LogPrompter : MonoBehaviour
    {
        private TMP_Text _tmpText = null;
        public TMP_Text TmpText
        {
            get
            {
                if (_tmpText == null)
                    _tmpText = this.GetComponent<TextMeshProUGUI>();
                if (_tmpText == null)
                    _tmpText = this.GetComponent<TextMeshPro>(); // TextMeshProUGUIじゃなかったらTextMeshPro
                return _tmpText;
            }
        }

        private StringBuilder _builder = new StringBuilder();

        [SerializeField, Tooltip("テキストの先頭に時刻を表示する")]
        private bool _useTimeStamp = true;

        [SerializeField, Tooltip("ログの種別に応じて色を付ける")]
        private bool _coloredByLogType = true;

        [SerializeField, Tooltip("特定の文字列を含むログは表示しない")]
        private string[] _ignorePhrases = new string[] { };

        [SerializeField, Tooltip("表示可能な最大行数")]
        private int _maxVisibleLines = 10;

        private void Awake()
        {
            if (TmpText == null)
            {
                this.enabled = false;
                throw new NullReferenceException("No text component found.");
            }
            
            Application.logMessageReceived -= HandleLog;
            Application.logMessageReceived += HandleLog;

            _builder = new StringBuilder();

            TmpText.overflowMode = TextOverflowModes.Overflow;

            if (_coloredByLogType)
                TmpText.richText = true;

            TmpText.text = string.Empty;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logText, string stackTrace, LogType logType)
        {
            _builder.Clear();

            if (0 < _ignorePhrases.Length)
            {
                for (int i = 0; i < _ignorePhrases.Length; i++)
                {
                    if (_ignorePhrases[i] != string.Empty && logText.IndexOf(_ignorePhrases[i]) != -1)
                    {
                        return;
                    }
                }
            }

            if (_useTimeStamp)
            {
                _builder.Append($"[{DateTime.Now.ToLongTimeString()}:{DateTime.Now.Millisecond:D3}] ");
            }

            if (_coloredByLogType)
            {
                switch (logType)
                {
                    case LogType.Assert:
                    case LogType.Warning:
                        logText = GetColoredString(logText, "yellow");
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        logText = GetColoredString(logText, "red");
                        break;
                    default:
                        break;
                }
            }

            _builder.AppendLine(logText);
            TmpText.text += _builder.ToString();

            AdjustTextToMaxLines();
        }

        /// <summary>
        /// 色付き文字列に変換
        /// </summary>
        /// <param name="src"></param>
        /// <param name="colorString"></param>
        /// <returns></returns>
        private string GetColoredString(string src, string colorString)
        {
            return $"<color={colorString}>{src}</color>";
        }

        /// <summary>
        /// 最大行数を超えた場合に文字列を調整
        /// </summary>
        private void AdjustTextToMaxLines()
        {
            TmpText.ForceMeshUpdate();

            int currentLineCount = TmpText.textInfo.lineCount;

            if (currentLineCount > _maxVisibleLines)
            {
                int overflowLines = currentLineCount - _maxVisibleLines;

                int deleteLength = 0;
                int foundIdx = 0;
                for (int i = 0; i < overflowLines; i++)
                {
                    foundIdx = TmpText.text.IndexOf('\n', foundIdx + 1);
                }
                deleteLength = foundIdx + 1;

                TmpText.text = TmpText.text.Remove(0, deleteLength);
            }
        }
    }
}
