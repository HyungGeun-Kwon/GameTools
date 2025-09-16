using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DotNetHelper.MsDiKit.Common;
using DotNetHelper.MsDiKit.RegionServices;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Client.Wpf.Common.Regions
{
    public static class TabRegionAttached
    {
        private static volatile Func<IServiceProvider>? _getServiceProvider;
        
        public static void Configure(Func<IServiceProvider> getServiceProvider) => _getServiceProvider = getServiceProvider;
        
        private static IRegionService GetRegionService()
        {
            var sp = _getServiceProvider?.Invoke() ?? throw new InvalidOperationException("ServiceProvider not configured.");
            return sp.GetRequiredService<IRegionService>();
        }

        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached(
                "RegionName", 
                typeof(string), 
                typeof(TabRegionAttached), 
                new PropertyMetadata(null, OnRegionNameChanged));
        public static void SetRegionName(DependencyObject d, string? v) => d.SetValue(RegionNameProperty, v);
        public static string? GetRegionName(DependencyObject d) => (string?)d.GetValue(RegionNameProperty);

        public static readonly DependencyProperty ViewKeyProperty =
            DependencyProperty.RegisterAttached(
                "ViewKey", 
                typeof(string),
                typeof(TabRegionAttached), 
                new PropertyMetadata(null, OnViewInfoChanged));
        public static void SetViewKey(DependencyObject d, string? v) => d.SetValue(ViewKeyProperty, v);
        public static string? GetViewKey(DependencyObject d) => (string?)d.GetValue(ViewKeyProperty);

        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.RegisterAttached(
                "Parameters", 
                typeof(Parameters),
                typeof(TabRegionAttached), 
                new PropertyMetadata(null, OnViewInfoChanged));
        public static void SetParameters(DependencyObject d, Parameters? v) => d.SetValue(ParametersProperty, v);
        public static Parameters? GetParameters(DependencyObject d) => (Parameters?)d.GetValue(ParametersProperty);

        // 내부 초기화 플래그
        private static readonly DependencyProperty InitializedProperty =
            DependencyProperty.RegisterAttached(
                "_RegionHostInit", 
                typeof(bool),
                typeof(TabRegionAttached), 
                new PropertyMetadata(false));
        private static readonly DependencyProperty LastAppliedRegionProperty =
            DependencyProperty.RegisterAttached(
                "_LastRegion",
                typeof(string),
                typeof(TabRegionAttached),
                new PropertyMetadata(null));
        private static readonly DependencyProperty LastAppliedKeyProperty =
            DependencyProperty.RegisterAttached(
                "_LastKey",
                typeof(string),
                typeof(TabRegionAttached),
                new PropertyMetadata(null));
        private static readonly DependencyProperty LastAppliedParamsRefProperty =
            DependencyProperty.RegisterAttached(
                "_LastParamsRef",
                typeof(object),
                typeof(TabRegionAttached),
                new PropertyMetadata(null));

        private static void OnRegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ContentControl host) return;

            // 이전 이름 해제
            if (e.OldValue is string oldName && !string.IsNullOrWhiteSpace(oldName))
                TryUnregister(oldName);

            EnsureWireUp(host);

            // 새 이름 등록(호스트가 이미 로드되어 있다면 즉시 등록)
            if (e.NewValue is string newName && !string.IsNullOrWhiteSpace(newName) && host.IsLoaded)
                TryRegister(newName, host);

            if (host.IsLoaded && host.IsVisible && !IsInDesignMode(host))
                DebouncedSetView(host);
        }

        private static void OnViewInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ContentControl host) return;

            EnsureWireUp(host);
        }
        private static void EnsureWireUp(ContentControl host)
        {
            if ((bool)host.GetValue(InitializedProperty)) return;
            host.SetValue(InitializedProperty, true);

            host.Loaded += (_, __) =>
            {
                if (IsInDesignMode(host)) return;

                var name = GetRegionName(host);
                if (!string.IsNullOrWhiteSpace(name))
                    TryRegister(name!, host);

                // 활성화 시점 1회 적용 (디바운스)
                DebouncedSetView(host);
            };

            host.Unloaded += (_, __) =>
            {
                var name = GetRegionName(host);
                if (!string.IsNullOrWhiteSpace(name))
                    TryUnregister(name!);
            };
        }

        private static void DebouncedSetView(ContentControl host) =>
            host.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    if (!host.IsLoaded || IsInDesignMode(host)) return;
                    TrySetView(host);
                }),
                DispatcherPriority.Background);

        private static void TrySetView(ContentControl host)
        {
            var name = GetRegionName(host);
            var key = GetViewKey(host);
            var prm = GetParameters(host);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(key))
                return;

            // 중복 적용 방지
            if (!NeedsApply(host, name!, key!, prm))
                return;

            try
            {
                GetRegionService().SetView(name!, key!, prm);
                CacheApplied(host, name!, key!, prm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TabRegionAttached] SetView failed. region={name}, key={key}, ex={ex}");
            }
        }

        private static bool NeedsApply(ContentControl host, string region, string key, Parameters? prm)
        {
            var lastRegion = (string?)host.GetValue(LastAppliedRegionProperty);
            var lastKey = (string?)host.GetValue(LastAppliedKeyProperty);
            var lastParamsRef = host.GetValue(LastAppliedParamsRefProperty);

            if (!string.Equals(lastRegion, region, StringComparison.Ordinal))
                return true;
            if (!string.Equals(lastKey, key, StringComparison.Ordinal))
                return true;
            if (!ReferenceEquals(lastParamsRef, prm))
                return true;

            return false;
        }

        private static void CacheApplied(ContentControl host, string region, string key, Parameters? prm)
        {
            host.SetValue(LastAppliedRegionProperty, region);
            host.SetValue(LastAppliedKeyProperty, key);
            host.SetValue(LastAppliedParamsRefProperty, prm);
        }

        private static void TryRegister(string regionName, ContentControl host)
        {
            try
            {
                GetRegionService().RegisterRegionName(regionName, host);
            }
            catch (InvalidOperationException)
            {
                // 중복 등록 등은 무시
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TabRegionAttached] RegisterRegionName failed. region={regionName}, ex={ex}");
            }
        }

        private static void TryUnregister(string regionName)
        {
            try
            {
                GetRegionService().UnregisterRegionName(regionName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TabRegionAttached] UnregisterRegionName failed. region={regionName}, ex={ex}");
            }
        }

        private static bool IsInDesignMode(DependencyObject d) => DesignerProperties.GetIsInDesignMode(d);
    }
}
