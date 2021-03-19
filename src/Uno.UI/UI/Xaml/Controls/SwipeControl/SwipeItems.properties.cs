// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// DO NOT EDIT! This file was generated by CustomTasks.DependencyPropertyCodeGen

namespace Windows.UI.Xaml.Controls
{
	public partial class SwipeItems
	{
		GlobalDependencyProperty SwipeItemsProperties.s_ModeProperty{ null };

SwipeItemsProperties.SwipeItemsProperties()
{
    EnsureProperties();
}

void SwipeItemsProperties.EnsureProperties()
{
    if (!s_ModeProperty)
    {
        s_ModeProperty =
            InitializeDependencyProperty(
                "Mode",
                winrt.name_of<winrt.SwipeMode>(),
                winrt.name_of<winrt.SwipeItems>(),
                false /* isAttached */,
                ValueHelper<winrt.SwipeMode>.BoxValueIfNecessary(winrt.SwipeMode.Reveal),
                winrt.PropertyChangedCallback(&OnModePropertyChanged));
    }
}

void SwipeItemsProperties.ClearProperties()
{
    s_ModeProperty = null;
}

void SwipeItemsProperties.OnModePropertyChanged(
    winrt.DependencyObject & sender,
    winrt.DependencyPropertyChangedEventArgs & args)
{
    var owner = sender.as<winrt.SwipeItems>();
    winrt.get_self<SwipeItems>(owner).OnPropertyChanged(args);
}

void SwipeItemsProperties.Mode(winrt.SwipeMode & value)
{
    [[gsl.suppress(con)]]
    {
    (SwipeItems)(this).SetValue(s_ModeProperty, ValueHelper<winrt.SwipeMode>.BoxValueIfNecessary(value));
    }
}

winrt.SwipeMode SwipeItemsProperties.Mode()
{
    return ValueHelper<winrt.SwipeMode>.CastOrUnbox((SwipeItems)(this).GetValue(s_ModeProperty));
}
}
}
