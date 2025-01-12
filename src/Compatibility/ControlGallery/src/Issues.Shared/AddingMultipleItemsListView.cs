﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;
#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Microsoft.Maui.Controls.Compatibility.UITests;
#endif

namespace Microsoft.Maui.Controls.ControlGallery.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Adding Multiple Items to a ListView", PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ListView)]
#endif
	public class AddingMultipleItemsListView : TestContentPage
	{
		protected override void Init()
		{
			Title = "Hours";
			var exampleViewModel = new ExampleViewModel();
			BindingContext = exampleViewModel;

			var listView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(CustomViewCell)),
				HeightRequest = 400,
				VerticalOptions = LayoutOptions.Start
			};

			listView.SetBinding(ListView.ItemsSourceProperty, new Binding("Jobs", BindingMode.TwoWay));

			var addOneJobButton = new Button
			{
				Text = "Add One"
			};
			addOneJobButton.SetBinding(Button.CommandProperty, new Binding("AddOneCommand"));

			var addTwoJobsButton = new Button
			{
				Text = "Add Two"
			};
			addTwoJobsButton.SetBinding(Button.CommandProperty, new Binding("AddTwoCommand"));

			var layout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Spacing = 15,
				Children = {
					listView,
					addOneJobButton,
					addTwoJobsButton
				}
			};
			Content = layout;
		}

		[Preserve(AllMembers = true)]
		public class CustomViewCell : ViewCell
		{
			public CustomViewCell()
			{
				var jobId = new Label
				{
					FontSize = 20,
					WidthRequest = 105,
					VerticalOptions = LayoutOptions.Center,

					HorizontalOptions = LayoutOptions.StartAndExpand
				};
				jobId.SetBinding(Label.TextProperty, "JobId");

				var jobName = new Label
				{
					VerticalOptions = LayoutOptions.Center,
					WidthRequest = 175,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				};
				jobName.SetBinding(Label.TextProperty, "JobName");

				var hours = new Label
				{
					WidthRequest = 45,
					VerticalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = TextAlignment.End,
					HorizontalOptions = LayoutOptions.EndAndExpand,

				};
				hours.SetBinding(Label.TextProperty, new Binding("Hours", BindingMode.OneWay, new DoubleStringConverter()));

				var hlayout = new StackLayout
				{
					Children = {
						jobId,
						jobName,
						hours
					},
					Orientation = StackOrientation.Horizontal,
				};

				View = hlayout;
			}
		}

#if UITEST
		[Test]
		[FailsOnMauiIOS]
		public void AddingMultipleListViewTests1AllElementsPresent()
		{
			RunningApp.WaitForElement(q => q.Marked("Big Job"));
			RunningApp.WaitForElement(q => q.Marked("Smaller Job"));
			RunningApp.WaitForElement(q => q.Marked("Add On Job"));
			RunningApp.WaitForElement(q => q.Marked("Add One"));
			RunningApp.WaitForElement(q => q.Marked("Add Two"));
			RunningApp.WaitForElement(q => q.Marked("3672"));
			RunningApp.WaitForElement(q => q.Marked("6289"));
			RunningApp.WaitForElement(q => q.Marked("3672-41"));
			RunningApp.WaitForElement(q => q.Marked("2"));
			RunningApp.WaitForElement(q => q.Marked("2"));
			RunningApp.WaitForElement(q => q.Marked("23"));

			RunningApp.Screenshot("All elements are present");
		}

		[Test]
		public void AddingMultipleListViewTests2AddOneElementToList()
		{
			RunningApp.Tap(q => q.Marked("Add One"));

			RunningApp.WaitForElement(q => q.Marked("1234"), timeout: TimeSpan.FromSeconds(2));
			RunningApp.Screenshot("One more element exists");
		}

		[Test]
		[FailsOnMauiIOS]
		public void AddingMultipleListViewTests3AddTwoElementToList()
		{
			RunningApp.Screenshot("Click 'Add Two'");
			RunningApp.Tap(q => q.Marked("Add Two"));

			RunningApp.WaitForElement(q => q.Marked("9999"), timeout: TimeSpan.FromSeconds(2));
			RunningApp.WaitForElement(q => q.Marked("8888"), timeout: TimeSpan.FromSeconds(2));
			RunningApp.Screenshot("Two more element exist");
		}
#endif
	}
}
