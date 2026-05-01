using System.Collections.Generic;
using MemberWorkspace.CHG._02_Scripts.TextBoxSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ColorUtility = UnityEngine.ColorUtility;

namespace MemberWorkspace.CHG._02_Scripts.TalkSystem.TalkSystem
{
    [CustomEditor(typeof(DialogueNodeSO))]
    public class DialogueNodeSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset editorView = default;

        private Button _colorSetButton;
        private ColorField _textColorField;
        private Button _effectSetButton;
        private EnumField _textEffectTypeField;
        private EnumFlagsField _textEffectSettingValueTypeField;
        private FloatField _textEffectSettingFieldA;
        private FloatField _textEffectSettingFieldS;
        private TextEffectType _textEffectType;
        private TextEffectSettingType _textEffectSettingType;
    
        private DialogueNodeSO _dialogueNodeSO;
        private TextField _lastUsingTextField;

        private Dictionary<TextEffectSettingType, int> _effectSettingValues = new();
        private int _selectedBoxStart;
        private int _selectedBoxEnd;
    
        public override VisualElement CreateInspectorGUI()
        {
            //TextSettingSO Inspector Draw
            _dialogueNodeSO = (DialogueNodeSO)target;
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            editorView.CloneTree(root);
        
            //Editor Content Injection, Setting
            _colorSetButton = root.Q<Button>("ColorSetButton");
            _effectSetButton = root.Q<Button>("EffectSetButton");
            _textColorField = root.Q<ColorField>("TextColor");
            _textEffectTypeField = root.Q<EnumField>("TextEffectType");
            _textEffectSettingValueTypeField = root.Q<EnumFlagsField>("TextEffectSettingType");
            _textEffectSettingFieldA = root.Q<FloatField>("TextEffectSettingValueA");
            _textEffectSettingFieldS = root.Q<FloatField>("TextEffectSettingValueS");

            _colorSetButton.clicked += HandleColorBtnClick;
            _effectSetButton.clicked += HandleEffectBtnClick; 
            _colorSetButton.focusable = false;
            _effectSetButton.focusable = false;
        
            _colorSetButton.RegisterCallback<MouseDownEvent>(evt => CaptureSelection(), TrickleDown.TrickleDown);
            _effectSetButton.RegisterCallback<MouseDownEvent>(evt => CaptureSelection(), TrickleDown.TrickleDown);

        
            #region ValueChageCallBack
        
            _textEffectSettingValueTypeField.RegisterValueChangedCallback(evt =>
            {
                _textEffectSettingType = (TextEffectSettingType)evt.newValue;
            });
        

            _textEffectTypeField.RegisterValueChangedCallback(evt =>
            {
                _textEffectType = (TextEffectType)evt.newValue;
            });

            #endregion
        
            //focus change play
            root.RegisterCallback<FocusInEvent>(OnElementFocused);
            //play 0.05second
            root.schedule.Execute(TrackSelection).Every(50);
            return root;
        }

        private void CaptureSelection()
        {
            if (_lastUsingTextField == null) return;
            _selectedBoxStart = _lastUsingTextField.cursorIndex;
            _selectedBoxEnd = _lastUsingTextField.selectIndex;
        }

        private void OnElementFocused(FocusInEvent evt)
        {
            VisualElement targetElement = evt.target as VisualElement;
        
            TextField parentTextField = targetElement?.GetFirstAncestorOfType<TextField>();
        
            if (targetElement is TextField tf)
                _lastUsingTextField = tf;
            else if (parentTextField != null)
                _lastUsingTextField = parentTextField;
        }

        private void TrackSelection()
        {
            if (_lastUsingTextField == null) return;

            //what is get focus
            var focusController = _lastUsingTextField.panel?.focusController;
            var focusedElem = _lastUsingTextField?.panel?.focusController?.focusedElement as VisualElement;
            if (focusedElem == null) return;
            // if focusedElem get textField
            var tf = (focusedElem as TextField)
                     ?? focusedElem.GetFirstAncestorOfType<TextField>();

            if (tf != null)
            {
                _lastUsingTextField = tf;
                _selectedBoxStart = tf.cursorIndex;
                _selectedBoxEnd = tf.selectIndex;
            }
        }

        private void HandleColorBtnClick()
        {
            if (GetSelectText(out var start, out var end, out var originText)) return;
        
            Color color = _textColorField.value;
            string hex = ColorUtility.ToHtmlStringRGB(color);

            string selectedText = originText.Substring(start, end - start);
            string newText = originText.Remove(start, end - start);
            newText = newText.Insert(start, $"<color=#{hex}>{selectedText}</color>");

            _lastUsingTextField.value = newText;
            EditorUtility.SetDirty(_dialogueNodeSO);

            _lastUsingTextField.Focus();
        }

        private void HandleEffectBtnClick()
        {
            if (GetSelectText(out var start, out var end, out var originText)) return;


            string effectString = _textEffectType.ToString();
            string selectedText = originText.Substring(start, end - start);
            string effectSettingString = "";
            if (_textEffectSettingType != TextEffectSettingType.none)
            {
                if (_textEffectSettingType == TextEffectSettingType.everything)
                {
                    effectSettingString = effectSettingString.Insert(0,$" a={_textEffectSettingFieldA.value}");
                    effectSettingString = effectSettingString.Insert(0,$" s={_textEffectSettingFieldS.value}");
                }
                else
                {
                    Debug.Log(_textEffectSettingFieldA.value);
                    Debug.Log(_textEffectSettingFieldS.value);
                    
                    if ((_textEffectSettingType & TextEffectSettingType.a) != 0)
                        effectSettingString = effectSettingString.Insert(0,$" a={_textEffectSettingFieldA.value}");

                    if ((_textEffectSettingType & TextEffectSettingType.s) != 0)
                        effectSettingString = effectSettingString.Insert(0,$" s={_textEffectSettingFieldS.value}");
                }
            }
            
            string newText = originText.Remove(start, end - start);
            newText = newText.Insert(start, $"<{effectString}{(string.IsNullOrEmpty(effectSettingString) ? "" : effectSettingString)}>{selectedText}</{effectString}>");
        
            _lastUsingTextField.value = newText;
            EditorUtility.SetDirty(_dialogueNodeSO);
        
            _lastUsingTextField.Focus();
        }

        private bool GetSelectText(out int start, out int end, out string originText)
        {
            if (_lastUsingTextField == null)
            {
                start = 0;
                end = 0;
                originText = null;
                return true;
            }
        
            start = _selectedBoxStart;
            end = _selectedBoxEnd;
        
            if (start > end)
                (end, start) = (start, end);

            originText = _lastUsingTextField.value;
            if (string.IsNullOrEmpty(originText)) return true;
            if (start == end) return true;
            return false;
        }
        
        //Setting Value Save
        private void SaveEditorPrefs()
        {
            EditorPrefs.SetInt("DialogueNodeSOEditor_EffectType", (int)_textEffectType);
            EditorPrefs.SetInt("DialogueNodeSOEditor_EffectSettingType", (int)_textEffectSettingType);
            EditorPrefs.SetFloat("DialogueNodeSOEditor_FieldA", _textEffectSettingFieldA.value);
            EditorPrefs.SetFloat("DialogueNodeSOEditor_FieldS", _textEffectSettingFieldS.value);

            EditorPrefs.SetString("DialogueNodeSOEditor_Color", 
                ColorUtility.ToHtmlStringRGBA(_textColorField.value));
        }

        // Load
        private void LoadEditorPrefs()
        {
            _textEffectType = (TextEffectType)EditorPrefs.GetInt("DialogueNodeSOEditor_EffectType", 0);
            _textEffectSettingType = (TextEffectSettingType)EditorPrefs.GetInt("DialogueNodeSOEditor_EffectSettingType", 0);
    
            _textEffectTypeField.value = _textEffectType;
            _textEffectSettingValueTypeField.value = _textEffectSettingType;
            _textEffectSettingFieldA.value = EditorPrefs.GetFloat("DialogueNodeSOEditor_FieldA", 1f);
            _textEffectSettingFieldS.value = EditorPrefs.GetFloat("DialogueNodeSOEditor_FieldS", 1f);
    
            if (ColorUtility.TryParseHtmlString(
                    "#" + EditorPrefs.GetString("DialogueNodeSOEditor_Color", "FFFFFFFF"), out Color c))
                _textColorField.value = c;
        }
    }
}
