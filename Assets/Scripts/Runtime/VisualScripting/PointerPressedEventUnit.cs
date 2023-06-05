#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.ARFoundation.Samples.VisualScripting
{
    [UnitTitle("On Input System Pointer Pressed")]
    [UnitCategory("Events/AR Foundation/Samples")]
    [UnitOrder(1)]
    public class PointerPressedEventUnit : EventUnit<Vector2>
    {
        const string k_PointerPressedEventHook = "Pointer Pressed";

        protected sealed override bool register => true;

        [DoNotSerialize]
        public ValueOutput pointerPosition { get; private set; }

        public override EventHook GetHook(GraphReference _) => k_PointerPressedEventHook;

        protected override void Definition()
        {
            base.Definition();
            pointerPosition = ValueOutput<Vector2>(nameof(pointerPosition));
        }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            PointerPressedListener.instance.Enable();
        }

        public override void StopListening(GraphStack stack)
        {
            base.StopListening(stack);
            PointerPressedListener.instance.Disable();
        }

        protected override void AssignArguments(Flow flow, Vector2 arg)
        {
            flow.SetValue(pointerPosition, arg);
        }

        class PointerPressedListener
        {
            public static PointerPressedListener instance => m_Instance ??= new PointerPressedListener();
            static PointerPressedListener m_Instance;

            InputAction s_PressAction;

            PointerPressedListener()
            {
                s_PressAction = new InputAction(k_PointerPressedEventHook, binding: "<Pointer>/press");
                s_PressAction.performed += ctx =>
                {
                    if (ctx.control.device is Pointer device)
                    {
                        EventBus.Trigger(k_PointerPressedEventHook, device.position.ReadValue());
                    }
                };
                s_PressAction.Enable();
            }

            public void Enable()
            {
                s_PressAction.Enable();
            }

            public void Disable()
            {
                s_PressAction.Disable();
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
