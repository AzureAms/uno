﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Xaml;

namespace Uno.UI
{
	/// <summary>
	/// A set of uno.ui-specific helpers for Xaml
	/// </summary>
	public static class FrameworkElementHelper
	{
		/// <summary>
		/// Conditional table used in the context of Control and DataTemplates
		/// to link the lifetime generated XAML classes that hold x:Name backing
		/// fields to the top level UIElement of the template. This allows for
		/// ElementNameSubject and TargetProperty path to only keep weak references
		/// to their targets.
		/// </summary>
		private static readonly ConditionalWeakTable<DependencyObject, object> _contextAssociation = new ConditionalWeakTable<DependencyObject, object>();

		/// <summary>
		/// Set the rendering phase, defined via x:Phase.
		/// </summary>
		/// <param name="target">The target <see cref="FrameworkElement"/></param>
		/// <param name="phase">The render phase ID</param>
		public static void SetRenderPhase(FrameworkElement target, int phase)
			=> target.RenderPhase = phase;

		/// <summary>
		/// Sets the x:Phases defined by all the children controls. The control must be the root element of a DataTemplate.
		/// </summary>
		/// <param name="target">The target <see cref="FrameworkElement"/></param>
		/// <param name="declaredPhases">A set of phases used by the children controls.</param>
		public static void SetDataTemplateRenderPhases(FrameworkElement target, int[] declaredPhases)
			=> target.DataTemplateRenderPhases = declaredPhases;

		/// <summary>
		/// When true (normally because the IsUiAutomationMappingEnabled build property is set), setting the <see cref="Name"/> property
		/// programmatically will also set the appropriate test identifier for Android/iOS. Disabled by default because it may interfere
		/// with accessibility features in non-testing scenarios.
		/// On WebAssembly, settings this property also enables the ability for <see cref="Windows.UI.Xaml.Automation.AutomationProperties.AutomationIdProperty"/>
		/// to be applied to the visual tree elements.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsUiAutomationMappingEnabled { get; set; } = false;

		public static void SetBaseUri(FrameworkElement target, string uri)
		{
			if (target is { } fe)
			{
				fe.BaseUri = new Uri(uri);
			}
		}

		/// <summary>
		/// Associates an arbitrary object to the life time of the <paramref name="target"/> instance.
		/// </summary>
		/// <remarks>
		/// This method is used by the XAML generator to keep x:Name generated
		/// code alive alonside the top-level control of thete.
		/// </remarks>
		public static void AddObjectReference(DependencyObject target, object context)
			=> _contextAssociation.Add(target, context);

		/// <summary>
		/// This is the equivalent of <see cref="FeatureConfiguration.UIElement.UseInvalidateMeasurePath"/>
		/// but just for a specific element (and its descendants) in the visual tree.
		/// </summary>
		/// <remarks>
		/// This will have no effect is <see cref="FeatureConfiguration.UIElement.UseInvalidateMeasurePath"/>
		/// is set to false.
		/// </remarks>
#if !(__WASM__ || __SKIA__)
		[NotImplemented]
#endif
		public static void SetUseMeasurePathDisabled(UIElement element, bool state = true, bool eager = true, bool invalidate = true)
		{
#if __WASM__ || __SKIA__
			element.IsMeasureDirtyPathDisabled = state;

			if (eager)
			{
				using var children = element.GetChildren().GetEnumerator();
				while (children.MoveNext())
				{
					if (children.Current is { } child)
					{
						SetUseMeasurePathDisabled(child, state, eager: true, invalidate);
					}
				}
			}

			if (invalidate)
			{
				element.InvalidateMeasure();
			}
#else
			// Not implemented for this platform
#endif
		}

		public static bool GetUseMeasurePathDisabled(FrameworkElement element)
#if __WASM__ || __SKIA__
			=> element.IsMeasureDirtyPathDisabled;
#else
			=> false; // Not implemented for this platform
#endif
	}
}
