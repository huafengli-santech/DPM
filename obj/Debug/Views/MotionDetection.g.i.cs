﻿#pragma checksum "..\..\..\Views\MotionDetection.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "0AD6E0ABCF3BAC93587350611430E52436B85DFBCD682E846BB0798D74B2C35E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using DPM_Utility;
using DPM_Utility.Controls;
using LiveCharts.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DPM_Utility.Views {
    
    
    /// <summary>
    /// MotionDetection
    /// </summary>
    public partial class MotionDetection : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border wholeBorder;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel States_WarpPanel;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel dynamicLedWrapPanel;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer scrList;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel Lines_WarpPanel;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox cNameTextbox;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox eNameTextbox;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox axisorbufferTextBox;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Views\MotionDetection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DPM_Utility.Controls.IconButton addButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DPM Utility;component/views/motiondetection.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\MotionDetection.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.wholeBorder = ((System.Windows.Controls.Border)(target));
            
            #line 14 "..\..\..\Views\MotionDetection.xaml"
            this.wholeBorder.Loaded += new System.Windows.RoutedEventHandler(this.Border_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.States_WarpPanel = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 3:
            this.dynamicLedWrapPanel = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 4:
            this.scrList = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 5:
            this.Lines_WarpPanel = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 6:
            this.cNameTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.eNameTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.axisorbufferTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.addButton = ((DPM_Utility.Controls.IconButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

