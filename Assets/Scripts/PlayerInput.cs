using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Dracon
{
    public class PlayerInput : SentientModule
    {
        public enum KeyMap
        {
            Interaction,
            Test,
            Zoom,
            Inventory,
            Back,
            Primary
        }

        public class KeyEvents
        {
            public Action onKey, onKeyDown, onKeyUp;

            public int CheckEvents(KeyCode code)
            {
                if (code == KeyCode.Mouse0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        onKeyDown?.Invoke();
                        return 1;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        onKeyUp?.Invoke();
                        return 2;
                    }
                    if (Input.GetMouseButton(0))
                    {
                        onKey?.Invoke();
                        return 3;
                    }

                    return 0;
                }
                else if (code == KeyCode.Mouse1)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        onKeyDown?.Invoke();
                        return 1;
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        onKeyUp?.Invoke();
                        return 2;
                    }
                    if (Input.GetMouseButton(1))
                    {
                        onKey?.Invoke();
                        return 3;
                    }

                    return 0;
                }
                
                if (Input.GetKeyDown(code))
                {
                    onKeyDown?.Invoke();
                    return 1;
                }

                if (Input.GetKeyUp(code))
                {
                    onKeyUp?.Invoke();
                    return 2;
                }

                if (Input.GetKey(code))
                {
                    onKey?.Invoke();
                    return 3;
                }

                return 0;
            }
        }
    
        public Dictionary<KeyMap, KeyCode> keyMaps = new();
        public Dictionary<KeyMap, KeyEvents> keyEvents = new();

        [Space] public string lastLog;
        
        private void Update()
        {
            foreach (var evt in keyEvents)
            {
                var code = keyMaps[evt.Key];
                var result = evt.Value.CheckEvents(code);
                if (result != 0)
                {
                    lastLog = $"[{evt.Key}] [{code}] [{result}]";
                }
            }
        }
    }
}

