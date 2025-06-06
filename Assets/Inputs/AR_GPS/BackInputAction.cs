//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Inputs/AR_GPS/BackInputAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @BackInputAction: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @BackInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BackInputAction"",
    ""maps"": [
        {
            ""name"": ""Navigation"",
            ""id"": ""c5372364-1802-4cba-abe6-629d1a9314fe"",
            ""actions"": [
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""6d6676f8-1ca4-4fe5-acc3-00224d0cd658"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4f3ec3f0-bb84-4d3e-822b-b3cce7fe1ef9"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e43d54f-2839-42ce-a1e1-ca27a2d5874e"",
                    ""path"": ""<Android>/back"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Navigation
        m_Navigation = asset.FindActionMap("Navigation", throwIfNotFound: true);
        m_Navigation_Back = m_Navigation.FindAction("Back", throwIfNotFound: true);
    }

    ~@BackInputAction()
    {
        UnityEngine.Debug.Assert(!m_Navigation.enabled, "This will cause a leak and performance issues, BackInputAction.Navigation.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Navigation
    private readonly InputActionMap m_Navigation;
    private List<INavigationActions> m_NavigationActionsCallbackInterfaces = new List<INavigationActions>();
    private readonly InputAction m_Navigation_Back;
    public struct NavigationActions
    {
        private @BackInputAction m_Wrapper;
        public NavigationActions(@BackInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Back => m_Wrapper.m_Navigation_Back;
        public InputActionMap Get() { return m_Wrapper.m_Navigation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NavigationActions set) { return set.Get(); }
        public void AddCallbacks(INavigationActions instance)
        {
            if (instance == null || m_Wrapper.m_NavigationActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_NavigationActionsCallbackInterfaces.Add(instance);
            @Back.started += instance.OnBack;
            @Back.performed += instance.OnBack;
            @Back.canceled += instance.OnBack;
        }

        private void UnregisterCallbacks(INavigationActions instance)
        {
            @Back.started -= instance.OnBack;
            @Back.performed -= instance.OnBack;
            @Back.canceled -= instance.OnBack;
        }

        public void RemoveCallbacks(INavigationActions instance)
        {
            if (m_Wrapper.m_NavigationActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(INavigationActions instance)
        {
            foreach (var item in m_Wrapper.m_NavigationActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_NavigationActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public NavigationActions @Navigation => new NavigationActions(this);
    public interface INavigationActions
    {
        void OnBack(InputAction.CallbackContext context);
    }
}
