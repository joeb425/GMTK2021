// // GENERATED AUTOMATICALLY FROM 'Assets/Scripts/GameInputs.inputactions'
//
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Utilities;
//
// public class @GameInputs : IInputActionCollection, IDisposable
// {
//     public InputActionAsset asset { get; }
//     public @GameInputs()
//     {
//         asset = InputActionAsset.FromJson(@"{
//     ""name"": ""GameInputs"",
//     ""maps"": [
//         {
//             ""name"": ""Mouse"",
//             ""id"": ""ac0e7b5f-8159-4a38-bcdd-f3d3dfed7e57"",
//             ""actions"": [
//                 {
//                     ""name"": ""MouseClick"",
//                     ""type"": ""Button"",
//                     ""id"": ""b0166acb-c483-4416-a43a-f9f4f853d96d"",
//                     ""expectedControlType"": ""Button"",
//                     ""processors"": """",
//                     ""interactions"": """"
//                 },
//                 {
//                     ""name"": ""MousePosition"",
//                     ""type"": ""Value"",
//                     ""id"": ""84eab8f5-cf15-4062-914d-1fbc4b835575"",
//                     ""expectedControlType"": ""Vector2"",
//                     ""processors"": """",
//                     ""interactions"": """"
//                 },
//                 {
//                     ""name"": ""TouchDelta"",
//                     ""type"": ""Value"",
//                     ""id"": ""5329e858-4252-46e5-ad3d-9a42e1aaa9e1"",
//                     ""expectedControlType"": ""Vector2"",
//                     ""processors"": """",
//                     ""interactions"": """"
//                 },
//                 {
//                     ""name"": ""MouseWheel"",
//                     ""type"": ""Value"",
//                     ""id"": ""3d03b7dd-4690-4e76-9c98-0750d78de3d2"",
//                     ""expectedControlType"": ""Axis"",
//                     ""processors"": ""Clamp(min=-1,max=1)"",
//                     ""interactions"": """"
//                 }
//             ],
//             ""bindings"": [
//                 {
//                     ""name"": """",
//                     ""id"": ""6921eafa-d36f-48ef-a53c-4498ddd5684f"",
//                     ""path"": ""<Mouse>/leftButton"",
//                     ""interactions"": ""Press"",
//                     ""processors"": """",
//                     ""groups"": """",
//                     ""action"": ""MouseClick"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": """",
//                     ""id"": ""d89ce630-9a4c-47f2-a2b9-12d4b7048c48"",
//                     ""path"": ""<Mouse>/position"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": """",
//                     ""action"": ""MousePosition"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": """",
//                     ""id"": ""d16e327d-113b-42e7-a6a1-b2981261040c"",
//                     ""path"": ""<Touchscreen>/touch0/delta"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": """",
//                     ""action"": ""TouchDelta"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 },
//                 {
//                     ""name"": """",
//                     ""id"": ""c0d2209a-8e7f-4650-983f-0fff3447dc71"",
//                     ""path"": ""<Mouse>/scroll/y"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": """",
//                     ""action"": ""MouseWheel"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 }
//             ]
//         },
//         {
//             ""name"": ""Keyboard"",
//             ""id"": ""d8b6c4e1-43d7-4af9-84e2-8fccd663a769"",
//             ""actions"": [
//                 {
//                     ""name"": ""FastForward"",
//                     ""type"": ""Button"",
//                     ""id"": ""61c8201e-281f-416d-bef9-36dfe8245fd2"",
//                     ""expectedControlType"": ""Button"",
//                     ""processors"": """",
//                     ""interactions"": ""Press""
//                 }
//             ],
//             ""bindings"": [
//                 {
//                     ""name"": """",
//                     ""id"": ""2de10c1a-7d95-4e6b-9641-f2de5a2881a0"",
//                     ""path"": ""<Keyboard>/f1"",
//                     ""interactions"": """",
//                     ""processors"": """",
//                     ""groups"": """",
//                     ""action"": ""FastForward"",
//                     ""isComposite"": false,
//                     ""isPartOfComposite"": false
//                 }
//             ]
//         }
//     ],
//     ""controlSchemes"": []
// }");
//         // Mouse
//         m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
//         m_Mouse_MouseClick = m_Mouse.FindAction("MouseClick", throwIfNotFound: true);
//         m_Mouse_MousePosition = m_Mouse.FindAction("MousePosition", throwIfNotFound: true);
//         m_Mouse_TouchDelta = m_Mouse.FindAction("TouchDelta", throwIfNotFound: true);
//         m_Mouse_MouseWheel = m_Mouse.FindAction("MouseWheel", throwIfNotFound: true);
//         // Keyboard
//         m_Keyboard = asset.FindActionMap("Keyboard", throwIfNotFound: true);
//         m_Keyboard_FastForward = m_Keyboard.FindAction("FastForward", throwIfNotFound: true);
//     }
//
//     public void Dispose()
//     {
//         UnityEngine.Object.Destroy(asset);
//     }
//
//     public InputBinding? bindingMask
//     {
//         get => asset.bindingMask;
//         set => asset.bindingMask = value;
//     }
//
//     public ReadOnlyArray<InputDevice>? devices
//     {
//         get => asset.devices;
//         set => asset.devices = value;
//     }
//
//     public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;
//
//     public bool Contains(InputAction action)
//     {
//         return asset.Contains(action);
//     }
//
//     public IEnumerator<InputAction> GetEnumerator()
//     {
//         return asset.GetEnumerator();
//     }
//
//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return GetEnumerator();
//     }
//
//     public void Enable()
//     {
//         asset.Enable();
//     }
//
//     public void Disable()
//     {
//         asset.Disable();
//     }
//
//     // Mouse
//     private readonly InputActionMap m_Mouse;
//     private IMouseActions m_MouseActionsCallbackInterface;
//     private readonly InputAction m_Mouse_MouseClick;
//     private readonly InputAction m_Mouse_MousePosition;
//     private readonly InputAction m_Mouse_TouchDelta;
//     private readonly InputAction m_Mouse_MouseWheel;
//     public struct MouseActions
//     {
//         private @GameInputs m_Wrapper;
//         public MouseActions(@GameInputs wrapper) { m_Wrapper = wrapper; }
//         public InputAction @MouseClick => m_Wrapper.m_Mouse_MouseClick;
//         public InputAction @MousePosition => m_Wrapper.m_Mouse_MousePosition;
//         public InputAction @TouchDelta => m_Wrapper.m_Mouse_TouchDelta;
//         public InputAction @MouseWheel => m_Wrapper.m_Mouse_MouseWheel;
//         public InputActionMap Get() { return m_Wrapper.m_Mouse; }
//         public void Enable() { Get().Enable(); }
//         public void Disable() { Get().Disable(); }
//         public bool enabled => Get().enabled;
//         public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
//         public void SetCallbacks(IMouseActions instance)
//         {
//             if (m_Wrapper.m_MouseActionsCallbackInterface != null)
//             {
//                 @MouseClick.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseClick;
//                 @MouseClick.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseClick;
//                 @MouseClick.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseClick;
//                 @MousePosition.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
//                 @MousePosition.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
//                 @MousePosition.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
//                 @TouchDelta.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnTouchDelta;
//                 @TouchDelta.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnTouchDelta;
//                 @TouchDelta.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnTouchDelta;
//                 @MouseWheel.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseWheel;
//                 @MouseWheel.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseWheel;
//                 @MouseWheel.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseWheel;
//             }
//             m_Wrapper.m_MouseActionsCallbackInterface = instance;
//             if (instance != null)
//             {
//                 @MouseClick.started += instance.OnMouseClick;
//                 @MouseClick.performed += instance.OnMouseClick;
//                 @MouseClick.canceled += instance.OnMouseClick;
//                 @MousePosition.started += instance.OnMousePosition;
//                 @MousePosition.performed += instance.OnMousePosition;
//                 @MousePosition.canceled += instance.OnMousePosition;
//                 @TouchDelta.started += instance.OnTouchDelta;
//                 @TouchDelta.performed += instance.OnTouchDelta;
//                 @TouchDelta.canceled += instance.OnTouchDelta;
//                 @MouseWheel.started += instance.OnMouseWheel;
//                 @MouseWheel.performed += instance.OnMouseWheel;
//                 @MouseWheel.canceled += instance.OnMouseWheel;
//             }
//         }
//     }
//     public MouseActions @Mouse => new MouseActions(this);
//
//     // Keyboard
//     private readonly InputActionMap m_Keyboard;
//     private IKeyboardActions m_KeyboardActionsCallbackInterface;
//     private readonly InputAction m_Keyboard_FastForward;
//     public struct KeyboardActions
//     {
//         private @GameInputs m_Wrapper;
//         public KeyboardActions(@GameInputs wrapper) { m_Wrapper = wrapper; }
//         public InputAction @FastForward => m_Wrapper.m_Keyboard_FastForward;
//         public InputActionMap Get() { return m_Wrapper.m_Keyboard; }
//         public void Enable() { Get().Enable(); }
//         public void Disable() { Get().Disable(); }
//         public bool enabled => Get().enabled;
//         public static implicit operator InputActionMap(KeyboardActions set) { return set.Get(); }
//         public void SetCallbacks(IKeyboardActions instance)
//         {
//             if (m_Wrapper.m_KeyboardActionsCallbackInterface != null)
//             {
//                 @FastForward.started -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnFastForward;
//                 @FastForward.performed -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnFastForward;
//                 @FastForward.canceled -= m_Wrapper.m_KeyboardActionsCallbackInterface.OnFastForward;
//             }
//             m_Wrapper.m_KeyboardActionsCallbackInterface = instance;
//             if (instance != null)
//             {
//                 @FastForward.started += instance.OnFastForward;
//                 @FastForward.performed += instance.OnFastForward;
//                 @FastForward.canceled += instance.OnFastForward;
//             }
//         }
//     }
//     public KeyboardActions @Keyboard => new KeyboardActions(this);
//     public interface IMouseActions
//     {
//         void OnMouseClick(InputAction.CallbackContext context);
//         void OnMousePosition(InputAction.CallbackContext context);
//         void OnTouchDelta(InputAction.CallbackContext context);
//         void OnMouseWheel(InputAction.CallbackContext context);
//     }
//     public interface IKeyboardActions
//     {
//         void OnFastForward(InputAction.CallbackContext context);
//     }
// }
