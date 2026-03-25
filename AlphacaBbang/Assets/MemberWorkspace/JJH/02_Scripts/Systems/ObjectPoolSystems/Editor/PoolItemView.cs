using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System;
using UnityEngine.UIElements;

namespace GameLib.ObjectPool.Editor
{
    public class PoolItemView
    {
        private Label _nameLabel;
        private Button _deleteBtn;
        private VisualElement _rootElement;
        private Label _warningLabel;

        public event Action<PoolItemView> OnDeleteEvent;
        public event Action<PoolItemView> OnSelectEvent;

        public string Name
        {
            get => _nameLabel.text;
            set => _nameLabel.text = value;
        }

        public PoolItemSO poolingItem;

        public bool IsActive
        {
            get => _rootElement.ClassListContains("active");
            set => _rootElement.EnableInClassList("active", value);
        }

        public bool IsEmpty
        {
            get => _warningLabel.ClassListContains("on");
            set => _warningLabel.EnableInClassList("on", value);
        }

        public PoolItemView(VisualElement root, PoolItemSO item)
        {
            poolingItem = item;
            _rootElement = root.Q("PoolItem");
            _nameLabel = _rootElement.Q<Label>("ItemName");
            _deleteBtn = _rootElement.Q<Button>("DeleteBtn");
            _warningLabel = _rootElement.Q<Label>("WarningLabel");

            _deleteBtn.RegisterCallback<ClickEvent>(evt =>
            {
                OnDeleteEvent?.Invoke(this);
                evt.StopPropagation(); //Stop event propagation to parent
            });

            _rootElement.RegisterCallback<ClickEvent>(evt =>
            {
                OnSelectEvent?.Invoke(this);
                evt.StopPropagation(); //Stop event propagation to parent
            });
        }
    }
}