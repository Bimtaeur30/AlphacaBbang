using MemberWorkspace.CHG._02_Scripts.TextBoxSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[CustomEditor(typeof(TextSettingSO))]
public class TextSettingSOEditor : UnityEditor.Editor
{
    [SerializeField] private VisualTreeAsset editorView = default;

    private Button _colorSetButton;
    private ColorField _textColorField;
    private Button _effectSetButton;
    private EnumField _textEffectTypeField;
    private TextEffectType _textEffectType;
    
    private TextSettingSO textSettingSO;
    private TextField _lastUsingTextField;

    private int _seceltedBoxStart;
    private int _seceltedBoxEnd;
    
    public override VisualElement CreateInspectorGUI()
    {
        //TextSettingSO Inspector Draw
        textSettingSO = (TextSettingSO)target;
        VisualElement root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);
        editorView.CloneTree(root);
        
        //Editor Content Injection, Setting
        _colorSetButton = root.Q<Button>("ColorSetButton");
        _effectSetButton = root.Q<Button>("EffectSetButton");
        _textColorField = root.Q<ColorField>("TextColor");
        _textEffectTypeField = root.Q<EnumField>("TextEffectType");

        _colorSetButton.clicked += HandleColorBtnClick;
        _effectSetButton.clicked += HandleEffectBtnClick; 
        
        _colorSetButton.focusable = false;
        _effectSetButton.focusable = false;
        
        //focus change play
        root.RegisterCallback<FocusInEvent>(OnElementFocused);
        //play 0.05second
        root.schedule.Execute(TrackSelection).Every(50);
        return root;
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
            _seceltedBoxStart = tf.cursorIndex;
            _seceltedBoxEnd = tf.selectIndex;
        }
    }

    private void HandleColorBtnClick()
    {
        
    }

    private void HandleEffectBtnClick()
    {
        if (_lastUsingTextField == null) return;
        Debug.Log(_lastUsingTextField.cursorIndex);
        _effectSetButton.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (_lastUsingTextField != null)
            {
                _seceltedBoxStart = _lastUsingTextField.cursorIndex;
                _seceltedBoxEnd = _lastUsingTextField.selectIndex;
            }
        }, TrickleDown.TrickleDown);
        
        int start = _seceltedBoxStart;
        int end = _seceltedBoxEnd;
        
        if (start > end)
            (end, start) = (start, end);
        
        Debug.Log(start + " " + end);

        string originText = _lastUsingTextField.value;
        if (string.IsNullOrEmpty(originText)) return;
        if (start == end) return;
        
        //string targetTag = _textEffectType.ToString();
        _textEffectTypeField.RegisterValueChangedCallback(evt =>
        {
            _textEffectType = (TextEffectType)evt.newValue;
        });
        
        string selectedText = originText.Substring(start, end - start);
        Debug.Log(selectedText);
        
        string newText = originText.Remove(start, end - start);
        newText = newText.Insert(start, $"<{_textEffectType.ToString()}>{selectedText}</{_textEffectType.ToString()}>");
        
        _lastUsingTextField.value = newText;
        EditorUtility.SetDirty(textSettingSO);
        
        _lastUsingTextField.Focus();
    }
}
