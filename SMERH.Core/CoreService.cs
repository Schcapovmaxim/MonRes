using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SMERH.Core
{
    public class CoreService : IDisposable
    {
        public event EventHandler<RegistryChangedEventArgs> RegistryChanged;

        private readonly List<RegistryKey> _monitoredKeys = new List<RegistryKey>();
        private bool _isMonitoring = false;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegNotifyChangeKeyValue(
            IntPtr hKey,
            bool bWatchSubtree,
            RegChangeNotifyFilter dwNotifyFilter,
            IntPtr hEvent,
            bool fAsynchronous);

        [Flags]
        private enum RegChangeNotifyFilter
        {
            Key = 0x00000001,
            Attribute = 0x00000002,
            Value = 0x00000004,
            Security = 0x00000008,
        }

        public void AddKeyToMonitor(RegistryKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!_monitoredKeys.Contains(key))
            {
                _monitoredKeys.Add(key);
            }
        }

        public void StartMonitoring()
        {
            if (_isMonitoring) return;

            _isMonitoring = true;
            foreach (var key in _monitoredKeys)
            {
                var thread = new System.Threading.Thread(() => MonitorRegistryKey(key));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void MonitorRegistryKey(RegistryKey key)
        {
            using (var keyHandle = GetRegistryKeyHandle(key))
            {
                if (keyHandle.IsInvalid)
                    throw new InvalidOperationException("Failed to get registry key handle");

                IntPtr eventHandle = IntPtr.Zero;

                try
                {
                    eventHandle = CreateEvent(IntPtr.Zero, true, false, null);
                    if (eventHandle == IntPtr.Zero)
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                    while (_isMonitoring)
                    {
                        int result = RegNotifyChangeKeyValue(
                            keyHandle.DangerousGetHandle(),
                            true,
                            RegChangeNotifyFilter.Key | RegChangeNotifyFilter.Value,
                            eventHandle,
                            true);

                        if (result != 0)
                            throw new System.ComponentModel.Win32Exception(result);

                        if (WaitForSingleObject(eventHandle, 1000) == 0)
                        {
                            OnRegistryChanged(new RegistryChangedEventArgs(key.Name, null, RegistryChangeType.Modified));
                            ResetEvent(eventHandle);
                        }
                    }
                }
                finally
                {
                    if (eventHandle != IntPtr.Zero)
                        CloseHandle(eventHandle);
                }
            }
        }

        private SafeRegistryHandle GetRegistryKeyHandle(RegistryKey key)
        {
            var fieldInfo = typeof(RegistryKey).GetField("hkey",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (SafeRegistryHandle)fieldInfo.GetValue(key);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ResetEvent(IntPtr hEvent);

        public void StopMonitoring()
        {
            _isMonitoring = false;
        }

        protected virtual void OnRegistryChanged(RegistryChangedEventArgs e)
        {
            RegistryChanged?.Invoke(this, e);
        }

        public void Dispose()
        {
            StopMonitoring();
            foreach (var key in _monitoredKeys)
            {
                key?.Dispose();
            }
            _monitoredKeys.Clear();
        }
    }

    public class RegistryChangedEventArgs : EventArgs
    {
        public string KeyPath { get; }
        public string ValueName { get; }
        public RegistryChangeType ChangeType { get; }

        public RegistryChangedEventArgs(string keyPath, string valueName, RegistryChangeType changeType)
        {
            KeyPath = keyPath;
            ValueName = valueName;
            ChangeType = changeType;
        }
    }

    public enum RegistryChangeType
    {
        Created,
        Deleted,
        Modified
    }

    public class SafeRegistryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeRegistryHandle() : base(true) { }

        protected override bool ReleaseHandle()
        {
            return RegCloseKey(handle) == 0;
        }

        [DllImport("advapi32.dll")]
        private static extern int RegCloseKey(IntPtr hKey);
    }
}