﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SamplesApp.UITests.TestFramework;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace SamplesApp.UITests.Windows_UI_Xaml_Controls.TextBlockTests
{
	[TestFixture]
	public class TextBlockTests_Tests : SampleControlUITestBase
	{
		[Test]
		[AutoRetry]
		[ActivePlatforms(Platform.iOS, Platform.Android)] // Disabled for Browser because of missing top level .All() support
		public void When_Visibility_Changed_During_Arrange()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.TextBlock_Visibility_Arrange");

			QueryEx textBlock = new QueryEx(q => q.All().Marked("SubjectTextBlock"));

			_app.WaitForElement(textBlock);

			_app.WaitForDependencyPropertyValue(textBlock, "Text", "It worked!");
		}

		[Test]
		[AutoRetry]
		public void When_Multiple_Controls_And_Some_Are_Collapsed_Arrange()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.TextBlock_LineHeight_MultipleControls");

			var leftControls = new[]
			{
				_app.Marked("stackPanel1"), _app.Marked("txt1_1"), _app.Marked("rect1"), _app.Marked("txt1_2")
			};

			var rightControls = new[]
			{
				_app.Marked("stackPanel2"), _app.Marked("txt2_1"), _app.Marked("rect2"), _app.Marked("txt2_2")
			};

			_app.WaitForElement(rightControls.Last());

			using var _ = new AssertionScope();

			for (var i = 0; i < leftControls.Length; i++)
			{
				var left = _app.GetLogicalRect(leftControls[i]);
				var right = _app.GetLogicalRect(rightControls[i]);

				using var __ = new AssertionScope("ctl" + i);

				const float tolerance = 1f;
				left.Width.Should().BeApproximately(right.Width, tolerance, "Width");
				left.Height.Should().BeApproximately(right.Height, tolerance, "Height");
				left.Y.Should().BeApproximately(right.Y, tolerance, "Y position");
			}
		}

		[Test]
		[AutoRetry]
		public void When_Foreground_Changed_With_Visibility()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.TextBlock_Foreground_While_Collapsed");

			_app.WaitForText("FunnyTextBlock", "Look at me now");

			using var blueBefore = TakeScreenshot("Before - blue", ignoreInSnapshotCompare: true);

			_app.FastTap("ChangeTextBlockButton");

			using var blackBefore = TakeScreenshot("Before - black");

			var textRect = _app.GetPhysicalRect("FunnyTextBlock");

			ImageAssert.AreNotEqual(blueBefore, blackBefore, textRect);

			_app.FastTap("ChangeTextBlockButton");

			//_app.WaitForNoElement("FunnyTextBlock"); // This times out on WASM because view is considered to be still there when collapsed - https://github.com/unoplatform/Uno.UITest/issues/25

			_app.FastTap("ChangeTextBlockButton");

			_app.WaitForElement("FunnyTextBlock");

			using var blueAfter = TakeScreenshot("After - blue");

			ImageAssert.AreEqual(blueBefore, blueAfter, textRect);
		}

		[Test]
		[AutoRetry]
		public async Task When_TextDecoration_Changed()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.TextBlock_Decorations");

			var text01 = _app.Marked("text01");
			var text02 = _app.Marked("text02");

			using var before = TakeScreenshot("Before", ignoreInSnapshotCompare: true);

			text01.SetDependencyPropertyValue("TextDecorations", "1"); // Underline
			text02.SetDependencyPropertyValue("TextDecorations", "2"); // Strikethrough

			using var after = TakeScreenshot("Updated");

			ImageAssert.AreNotEqual(before, after);

			text01.SetDependencyPropertyValue("TextDecorations", "0"); // None
			text02.SetDependencyPropertyValue("TextDecorations", "0"); // None

			using var restored = TakeScreenshot("Restored");

			ImageAssert.AreEqual(before, restored);
		}

		[Test]
		[AutoRetry]
		public void When_FontWeight_Changed()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.TextBlock_FontWeight_Dynamic");

			var testBlock = _app.Marked("testBlock");

			testBlock.SetDependencyPropertyValue("FontWeight", "Thin");
			var rectBefore = _app.GetLogicalRect("testBlock");
			var widthBefore = rectBefore.Width;
			var heightBefore = rectBefore.Height;

			testBlock.SetDependencyPropertyValue("FontWeight", "Heavy");
			var rectAfter = _app.GetLogicalRect("testBlock");
			var widthAfter = rectAfter.Width;
			var heightAfter = rectAfter.Height;

			using var _ = new AssertionScope();
			widthBefore.Should().BeLessThan(widthAfter);
			heightBefore.Should().Be(heightAfter);
		}

		[Test]
		[AutoRetry]
		public void When_Foreground_Brush_Color_Changed()
		{
			Run("Uno.UI.Samples.Content.UITests.TextBlockControl.TextBlock_BrushColorChanging");

			var textColor = _app.Marked("textColor");

			var rect = _app.GetPhysicalRect("textResult");
			var xCheck = rect.CenterX;
			var yCheck = rect.CenterY;

			using var beforeColorChanged = TakeScreenshot("beforeColor", ignoreInSnapshotCompare: true);
			ImageAssert.HasColorAt(beforeColorChanged, xCheck, yCheck, Color.Red);

			textColor.SetDependencyPropertyValue("Text", "Blue");

			using var afterColorChanged = TakeScreenshot("afterColor");

			ImageAssert.AreNotEqual(afterColorChanged, beforeColorChanged);

			ImageAssert.HasColorAt(afterColorChanged, xCheck, yCheck, Color.Blue);
		}

		[Test]
		[AutoRetry]
		public void When_MaxLines_Changed_With_TextWrapping()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.SimpleText_MaxLines_Different_Font_Size");
			var maxLineContainer = _app.Marked("container3");

			var rectBefore = _app.GetLogicalRect("container3");
			var heightBefore = rectBefore.Height;

			maxLineContainer.SetDependencyPropertyValue("MaxLines", "2");
			var rectAfter = _app.GetLogicalRect("container3");
			var heightAfter = rectAfter.Height;

			heightAfter.Should().BeGreaterThan(heightBefore);
		}

		[Test]
		[AutoRetry]
		public void When_MaxLines_Changed_Without_TextWrapping()
		{
			Run("UITests.Shared.Windows_UI_Xaml_Controls.TextBlockControl.SimpleText_MaxLines_Different_Font_Size");
			var maxLineContainer = _app.Marked("container3");
			maxLineContainer.SetDependencyPropertyValue("TextWrapping", "NoWrap");

			var rectBefore = _app.GetLogicalRect("container3");
			var heightBefore = rectBefore.Height;

			maxLineContainer.SetDependencyPropertyValue("MaxLines", "2");
			var rectAfter = _app.GetLogicalRect("container3");
			var heightAfter = rectAfter.Height;

			heightAfter.Should().Be(heightBefore);
		}

		[Test]
		[AutoRetry()]
		public void When_Text_is_Constrained_Then_Clipping_is_Applied()
		{
			Run("Uno.UI.Samples.Content.UITests.TextBlockControl.TextBlock_ConstrainedByContainer");


			// 1) Take a screenshot of the whole sample
			// 2) Switch opacity of text to zero
			// 3) Take new screenshot
			// 4) Compare Right zone -- must be identical
			// 5) Compare Bottom zone -- must be identical
			// 6) Do the same for subsequent text block in sample (1 to 5)
			//
			// +-sampleRootPanel------------+--------------+
			// |                            |              |
			// |    +-borderX---------------+              |
			// |    | [textX]Lorem ipsum... |              |
			// |    +-----------------------+  Right zone  |
			// |    |                       |              |
			// |    |    Bottom zone        |              |
			// |    |                       |              |
			// +----+-----------------------+--------------+

			// (1)
			using var sampleScreenshot = this.TakeScreenshot("fullSample", ignoreInSnapshotCompare: true);
			var sampleRect = _app.GetPhysicalRect("sampleRootPanel");

			using var _ = new AssertionScope();


			Test("text1", "border1");
			Test("text2", "border2");
			Test("text3", "border3");
			Test("text4", "border4");
			Test("text5", "border5");

			void Test(string textControl, string borderControl)
			{
				var textRect = _app.GetPhysicalRect(borderControl);

				// (2)
				_app.Marked(textControl).SetDependencyPropertyValue("Opacity", "0");

				// (3)
				using var afterScreenshot = this.TakeScreenshot("sample-" + textControl, ignoreInSnapshotCompare: true);

				// (4)
				using (var s = new AssertionScope("Right zone"))
				{
					var rect1 = new AppRect(
						x: textRect.Right,
						y: sampleRect.Y,
						width: sampleRect.Right - textRect.Right,
						height: sampleRect.Height)
						.DeflateBy(1f);

					ImageAssert.AreEqual(sampleScreenshot, rect1, afterScreenshot, rect1);
				}

				// (5)
				using (var s = new AssertionScope("Bottom zone"))
				{
					var rect2 = new AppRect(
						x: textRect.X,
						y: textRect.Bottom,
						width: textRect.Width,
						height: sampleRect.Height - textRect.Bottom)
						.DeflateBy(1f);

					ImageAssert.AreEqual(sampleScreenshot, rect2, afterScreenshot, rect2);
				}
			}
		}

		[Test]
		[AutoRetry]
		public void When_MaxLines_Then_AlignmentPositionIsCorrect()
		{
			Run("Uno.UI.Samples.Content.UITests.TextBlockControl.SimpleText_MaxLines_Two_With_Wrap");

			_app.Marked("fontsize").SetDependencyPropertyValue("Value", "5");
			_app.Marked("slider").SetDependencyPropertyValue("Value", "375");
			_app.Marked("sliderV").SetDependencyPropertyValue("Value", "50");

			using var _ = new AssertionScope();

			CheckAlignmentAndWidth("border1", "textwrap");
			CheckAlignmentAndWidth("border2", "textwrapwords");
			CheckAlignmentAndWidth("border3", "textwrapellipsis");
			CheckAlignmentAndWidth("border4", "textwrapwordsellipsis");

			void CheckAlignmentAndWidth(string borderName, string textName)
			{
				const float precision = 1.15f;

				// Alignment=STRETCH
				var (borderRect, textRect) = GetRects(borderName, textName);

				textRect.X.Should().BeApproximately(borderRect.X, precision, "X - stretch");
				textRect.Width.Should().BeApproximately(borderRect.Width, precision, "Width - stretch");

				// Alignment=CENTER
				_app.Marked(textName).SetDependencyPropertyValue("HorizontalAlignment", "Center");
				(borderRect, textRect) = GetRects(borderName, textName);
				textRect.X.Should()
					.BeApproximately(
						borderRect.X + (borderRect.Width - textRect.Width)/2f,
						precision,
						"X - center");
				textRect.Width.Should().BeLessThan(borderRect.Width, "Width - center");

				// Alignment=LEFT
				_app.Marked(textName).SetDependencyPropertyValue("HorizontalAlignment", "Left");
				(borderRect, textRect) = GetRects(borderName, textName);
				textRect.X.Should().BeApproximately(borderRect.X, precision, "X - left");
				textRect.Width.Should().BeLessThan(borderRect.Width, "Width - left");

				// Alignment=RIGHT
				_app.Marked(textName).SetDependencyPropertyValue("HorizontalAlignment", "Right");
				(borderRect, textRect) = GetRects(borderName, textName);
				textRect.Right.Should().BeApproximately(borderRect.Right, precision, "Right - right");
				textRect.Width.Should().BeLessThan(borderRect.Width, "Width - right");
			}

			(IAppRect borderRect, IAppRect textRect) GetRects(string borderName, string textName)
			{
				var borderRect = _app.GetLogicalRect(borderName);
				var textRect = _app.GetLogicalRect(textName);
				return (borderRect, textRect);
			}
		}

		[Test]
		[AutoRetry]
		public void When_MaxLines_Changed_On_Stack_Container()
		{
			Run("Uno.UI.Samples.Content.UITests.TextBlockControl.SimpleText_MaxLines_Multiple_Containers");

			var stackTextBlockName = "stackTextBlock";
			var maxLineSlider = _app.Marked("slider");
			
			_app.WaitForElement(maxLineSlider);

			var numberOfLines = 1;
			maxLineSlider.SetDependencyPropertyValue("Value", $"{numberOfLines}");
			var rectBefore = _app.GetLogicalRect(stackTextBlockName);
			var lineHeight = rectBefore.Height;

			numberOfLines = 3;
			maxLineSlider.SetDependencyPropertyValue("Value", $"{numberOfLines}");

			var rectAfter = _app.GetLogicalRect(stackTextBlockName);
			var textBlockHeight = rectAfter.Height;

			var actualNumberOfLines = (int)Math.Ceiling(textBlockHeight / lineHeight);
			Assert.IsTrue(actualNumberOfLines == numberOfLines,
				"Results \n" +
				$"Expected Number of lines: {numberOfLines}. \n" +
				$"Actual Number of lines:{actualNumberOfLines}. \n" +
				$"Line height: {lineHeight}. \n");
		}

		[Test]
		[AutoRetry]
		public void When_MaxLines_Changed_On_Grid_Container()
		{
			Run("Uno.UI.Samples.Content.UITests.TextBlockControl.SimpleText_MaxLines_Multiple_Containers");

			var gridTextBlockName = "gridTextBlock";
			var maxLineSlider = _app.Marked("slider");

			_app.WaitForElement(maxLineSlider);

			var numberOfLines = 1;
			maxLineSlider.SetDependencyPropertyValue("Value", $"{numberOfLines}");
			var rectBefore = _app.GetLogicalRect(gridTextBlockName);
			var lineHeight = rectBefore.Height;

			numberOfLines = 2;
			maxLineSlider.SetDependencyPropertyValue("Value", $"{numberOfLines}");

			var rectAfter = _app.GetLogicalRect(gridTextBlockName);
			var textBlockHeight = rectAfter.Height;

			var actualNumberOfLines = (int)Math.Ceiling(textBlockHeight / lineHeight);
			Assert.IsTrue(actualNumberOfLines == numberOfLines,
				"Results \n" +
				$"Expected Number of lines: {numberOfLines}. \n" +
				$"Actual Number of lines:{actualNumberOfLines}. \n" +
				$"Line height: {lineHeight}. \n");
		}

		[Test]
		[AutoRetry]
		[ActivePlatforms(Platform.Browser)]
		public void When_Padding_Is_Changed_Then_Cache_Is_Missed()
		{
			Run("UITests.Windows_UI_Xaml_Controls.TextBlockControl.TextBlock_MeasureCache");

			_app.Marked("text").SetDependencyPropertyValue("Padding", "40");

			_app.WaitForElement("text");

			var w1 = _app.GetLogicalRect("textBorder").Width;
			var misses1 = _app.Marked("misses").GetDependencyPropertyValue<int>("Text");

			_app.Marked("text").SetDependencyPropertyValue("Padding", "10");

			_app.WaitForElement("text");

			var w2 = _app.GetLogicalRect("textBorder").Width;
			var misses2 = _app.Marked("misses").GetDependencyPropertyValue<int>("Text");

			_app.Marked("text").SetDependencyPropertyValue("Padding", "40");

			_app.WaitForElement("text");

			var w3 = _app.GetLogicalRect("textBorder").Width;
			var misses3 = _app.Marked("misses").GetDependencyPropertyValue<int>("Text");

			using var _ = new AssertionScope();

			w1.Should().BeGreaterThan(w2);
			w3.Should().Be(w1);
		}
	}
}
