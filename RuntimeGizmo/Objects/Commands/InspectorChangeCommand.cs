using System;
using SRLE.Components;
using SRLE.RuntimeGizmo.UndoRedo;

namespace SRLE.RuntimeGizmo.Objects.Commands
{
    public class InspectorChangeCommand : ICommand
    {
        private readonly InspectorBase.Setter m_Setter;
        private readonly object m_OldValue;
        private readonly object m_NewValue;
        private readonly Action m_OnApply;

        /// <param name="setter">The setter to call when executing or undoing.</param>
        /// <param name="oldValue">Value before the change (restored on undo).</param>
        /// <param name="newValue">Value after the change (applied on execute/redo).</param>
        /// <param name="onApply">Optional side-effect to run after both execute and undo (e.g. teleport re-registration).</param>
        public InspectorChangeCommand(InspectorBase.Setter setter, object oldValue, object newValue, Action onApply = null)
        {
            m_Setter = setter;
            m_OldValue = oldValue;
            m_NewValue = newValue;
            m_OnApply = onApply;
        }

        public void Execute()
        {
            m_Setter(m_NewValue);
            m_OnApply?.Invoke();
        }

        public void UnExecute()
        {
            m_Setter(m_OldValue);
            m_OnApply?.Invoke();
        }
    }
}
