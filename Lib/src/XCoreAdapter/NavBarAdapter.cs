// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2004, SIL International. All Rights Reserved.
// <copyright from='2004' to='2004' company='SIL International'>
//		Copyright (c) 2004, SIL International. All Rights Reserved.
// </copyright>
#endregion
//
// File: SidebarAdapter.cs
// Authorship History: Randy Regnier
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;  //for ImageList
using System.Collections;
using System.Diagnostics;

using DevComponents.DotNetBar;

using SIL.Utils; // for ImageCollection

namespace XCore
{
	/// <summary>
	/// Creates a SidebarAdapter.
	/// </summary>
	public class SidebarAdapter : AdapterBase, IxCoreColleague
	{
		protected ArrayList m_panelSubControls;
		protected bool m_populatingList;
		protected bool 	m_suspendEvents;
		protected bool m_firstLoad = true;

		#region Properties

		/// <summary>
		/// Overrides property, so the right kind of control gets created.
		/// </summary>
		protected override Control MyControl
		{
			get
			{
				//m_control=  new NavigationPane();
				if (m_control == null)
				{
					NavigationPane navPane = new NavigationPane();

					//				m_window.SuspendLayout();

					navPane.SuspendLayout();

					navPane.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
					navPane.AllowDrop = false;
					navPane.Dock = System.Windows.Forms.DockStyle.Left;
					navPane.Images = m_largeImages.ImageList;
					navPane.Name = "navPane";
					navPane.TabIndex = 0;
					// part of the can't quit bug!:					navPane.TabStop = false;

					navPane.Width = 140;
					navPane.NavigationBarHeight = 300;
					//
					//					navPane.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
					//					navPane.TitlePanel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
					//					navPane.TitlePanel.Location = new System.Drawing.Point(0, 0);
					//					navPane.TitlePanel.Name = "panelEx1";
					//					navPane.TitlePanel.Size = new System.Drawing.Size(184, 24);
					//					navPane.TitlePanel.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
					//					navPane.TitlePanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
					//					navPane.TitlePanel.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
					//					navPane.TitlePanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
					//					navPane.TitlePanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
					//					navPane.TitlePanel.Style.GradientAngle = 90;
					//					navPane.TitlePanel.Style.MarginLeft = 4;
					//					navPane.TitlePanel.TabIndex = 0;
					navPane.TitlePanel.Text = AdapterStrings.Loading;

					//					TestAddButton(navPane, "alpha");
					//					TestAddButton(navPane,"beta");

					//	navPane.Controls.Add(navPane.TitlePanel);
					m_control = navPane;



					//					navPane.NavigationBar.Dock = System.Windows.Forms.DockStyle.Top;
					navPane.NavigationBar.SplitterVisible = true;

					navPane.ResumeLayout(false);
					//
					//					m_window.ResumeLayout(false);

				}
				return base.MyControl;
			}
		}

		private void TestAddButton(NavigationPane navPane, string t)
		{
			ButtonItem  b = new DevComponents.DotNetBar.ButtonItem();
			navPane.Items.Add(b);

			b.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
			//b.Image = ((System.Drawing.Image)(resources.GetObject("buttonItem2.Image")));
			b.ImageIndex=1;
			//b.Name = "b";
			//	b.OptionGroup = "navBar";
			b.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003;
			b.Text =t;
			//	b.Tooltip = t;

			MakePanelToGoWithButton(navPane, b, null);
		}



		/// <summary>
		/// called by xWindow when everything is all set-up.
		/// </summary>
		/// <remarks> this is needed in this adapter, though it was not in previous sidebar adapters.
		/// we use it here because the expanded event of theDotNetBar fires before it is even painted,
		/// and if the listed is showing is based on with the initial tool will be, well, that tool has not been
		/// identified and instantiated at the point the sidebar is just been created.</remarks>
		public override void FinishInit()
		{
			//note: this is obviously not enough if we ever choose to not show the first handle initially.
			//at the moment,, we do not currently support identifying the initially expanded panel anyways,
			//so this should work.
			//			ChoiceGroup group = (ChoiceGroup) this.navPane.Panels[0].Tag;
			//			group.OnDisplay(this, null);
		}

		/// <summary>
		/// Gets the sidebar.
		/// </summary>
		protected NavigationPane NavPane
		{
			get { return (NavigationPane)MyControl; }
		}

		#endregion Properties

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public SidebarAdapter()
		{
			m_panelSubControls = new ArrayList ();
		}

		#endregion Constructor


		public void Init(Mediator mediator, XmlNode configurationParameters)
		{
			m_mediator = mediator;
			mediator.AddColleague(this);
		}

		public override void PersistLayout()
		{
			NavPane.SaveLayout(LayoutPersistPath);
		}

		protected string LayoutPersistPath
		{
			get
			{
				return System.IO.Path.Combine(SettingsPath (),"NavPaneLayout.xml");
			}
		}
		/// <summary>
		/// get the location/settings of various widgets so that we can restore them
		/// </summary>
		protected void DepersistLayout()
		{
			if (System.IO.File.Exists(LayoutPersistPath))
			{
				try
				{
					NavPane.LoadLayout(LayoutPersistPath);
				}
				catch(Exception)
				{}
			}
		}


		/// <summary>
		/// return an array of all of the objects which should
		/// 1) be queried when looking for someone to deliver a message to
		/// 2) be potential recipients of a broadcast
		/// </summary>
		/// <returns></returns>
		public IxCoreColleague[] GetMessageTargets()
		{
			return new IxCoreColleague[]{this};
		}

		#region IUIAdapter implementation

		// Note: The Init method is handled by the superclass.

		/// <summary>
		/// Overrides method to create main elements of the sidebar.
		/// </summary>
		/// <param name="groupCollection">Collection of groups for this sidebar.</param>
		public override void CreateUIForChoiceGroupCollection(ChoiceGroupCollection groupCollection)
		{
			string sAreas = m_mediator.StringTbl.LocalizeAttributeValue("Areas");
			foreach (ChoiceGroup group in groupCollection)
			{
				if (group.Label == sAreas)
				{
					group.ReferenceWidget = this.NavPane.NavigationBar;
					this.NavPane.NavigationBar.Tag = group;
				}
				else
				{
					MakeListControl(group);
				}
			}
		}



		/// <summary>
		/// Overrides method to add itmes to the selected main itme in the sidebar.
		/// </summary>
		/// <param name="group">The group that defines this part of the sidebar.</param>
		public override void CreateUIForChoiceGroup(ChoiceGroup group)
		{

			if (group.ReferenceWidget ==this.NavPane.NavigationBar)
				LoadAreaButtons(group);
			else if (group.ReferenceWidget is ListView)
				PopulateList(group);

			if(m_firstLoad)
			{
				m_firstLoad = false;
				DepersistLayout();
			}
		}

		protected void LoadAreaButtons (ChoiceGroup group)
		{
			NavPane.SuspendLayout();
			NavPane.Items.Clear();

			foreach(ChoiceRelatedClass item in group)
			{
				//Debug.Assert(item is ChoiceBase, "Only things that can be made into buttons should be appearing here.");
				Debug.Assert(item is ListPropertyChoice, "Only things that can be made into buttons should be appearing here.");
				MakeAreaButton((ListPropertyChoice)item);
			}
			NavPane.ResumeLayout(true);
		}

		/// <summary>
		/// Redraw ths expanded item, so that the selected and enabled items are up to date.
		/// </summary>
		public override void OnIdle()
		{

			if(NavPane.Items.Count >0)
				return;

			if(null != this.NavPane.NavigationBar.Tag)
				((ChoiceGroup)this.NavPane.NavigationBar.Tag).OnDisplay(null, null);
			//		CreateUIForChoiceGroup((ChoiceGroup)this.NavPane.NavigationBar.Tag);


		}

		#endregion IUIAdapter implementation

		#region Other methods


		/// <summary>
		/// Create a ButtonItem for the sidebar.
		/// </summary>
		/// <param name="panelItem"></param>
		/// <param name="control"></param>
		protected void MakeAreaButton(ListPropertyChoice choice)
		{
			UIItemDisplayProperties display = choice.GetDisplayProperties();
			display.Text = display.Text.Replace("_", "");
			ButtonItem button = new ButtonItem(choice.Id, display.Text);
			button.Tag = choice;
			choice.ReferenceWidget = button;

			button.ImageIndex = m_largeImages.GetImageIndex(display.ImageLabel);

			button.Text = display.Text;
			button. Name = choice.Value;

			button.ButtonStyle = eButtonStyle.ImageAndText;

			if(!display.Enabled)
				button.Text = button.Text + " NA";

			button.Click += new EventHandler(OnClickAreaButton);

			button.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
			//button.Image = ((System.Drawing.Image)(resources.GetObject("buttonItem2.Image")));
			//	button.Name = "buttonItem2";
			button.OptionGroup = "navBar";
			button.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003;

			button.Checked =	display.Checked;

			MakePanelToGoWithButton(NavPane, button, choice);

			NavPane.Items.Add(button);
			//a button in this framework not really a Control... so I don't know how to use
			//(the same company's) balloon tip control on a sidebar button!
			//	m_mediator.SendMessage("RegisterHelpTarget", button.ContainerControl);
		}

		protected void MakePanelToGoWithButton (NavigationPane np, ButtonItem button,ListPropertyChoice choice)
		{
			m_suspendEvents = true;
			NavigationPanePanel p = new DevComponents.DotNetBar.NavigationPanePanel();
			p.Dock = System.Windows.Forms.DockStyle.Fill;
			p.Location = new System.Drawing.Point(0, 24);
			p.ParentItem = button;

			p.Style.Alignment = System.Drawing.StringAlignment.Center;
			p.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
			p.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
			p.Style.BackgroundImagePosition = DevComponents.DotNetBar.eBackgroundImagePosition.Tile;
			p.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
			p.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
			p.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
			p.Style.GradientAngle = 90;
			p.Style.WordWrap = true;
			p.StyleMouseDown.Alignment = System.Drawing.StringAlignment.Center;
			p.StyleMouseDown.WordWrap = true;
			p.StyleMouseOver.Alignment = System.Drawing.StringAlignment.Center;
			p.StyleMouseOver.WordWrap = true;
			p.TabIndex = 3;

			//			Object maker =m_mediator.PropertyTable.GetValue("PanelMaker");
			//			if (maker == null)
			//			{
			//				p.Text = "You must provide a PanelMaker in the property table to create panels";
			//				p.Name = "navigationPanePanel2";
			//			}
			//			else
			//			{
			AddControlsToPanel(choice, p);
			//			}

			//p.VisibleChanged +=new EventHandler(OnPanelVisibleChanged);
			np.Controls.Add(p);
			p.Layout+=new LayoutEventHandler(OnPanelLayout);
			m_suspendEvents = false;

		}

		private void AddControlsToPanel(ListPropertyChoice choice, NavigationPanePanel p)
		{
			ArrayList controls =AddSubControlsForButton(choice);

			p.Controls.Clear();//DON"T DISPOSE!

			//ArrayList controls =((PanelMaker)maker).GetControlsFromChoice(choice);
			//we reversed these because we want to show them from top to bottom,
			//but the way controls work in forms, we have to actually add them in reverse order.
			if (controls != null)
			{
				controls.Reverse();
				foreach(Control control in controls)
				{

					AddOneControlToPanel(control, p, controls.Count);
				}

			}
		}

		private void AddOneControlToPanel(Control control, NavigationPanePanel navPane, int controlCount)
		{
			// As per LT-3392, this section has been commented out to remove the header
			// If we change our minds, just remove the comments and everything should work again

			//PanelEx header = new PanelEx();
			//header.Text = ((ChoiceGroup)control.Tag).Label;
			//header.Dock = DockStyle.Top;

			//header.Location = new System.Drawing.Point(1, 1);
			//header.Name = "panelEx1";
			//header.Size = new System.Drawing.Size(182, 20);
			//header.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
			//header.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
			//header.Style.BackgroundImagePosition = DevComponents.DotNetBar.eBackgroundImagePosition.Tile;
			//header.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
			//header.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
			//header.Style.GradientAngle = 90;
			//header.Style.MarginLeft = 4;

			control.Dock = DockStyle.Top;

			navPane.Controls.Add( control);
			//navPane.Controls.Add(header);
		}


		/// <summary>
		/// make a control to show, for example, the list of tools, or the list of filters.
		/// </summary>
		/// <param name="group"></param>
		protected void MakeListControl(ChoiceGroup group)
		{
			ListView list= new ListView();
			list.View = View.List;
			list.Name = group.ListId;
			list.Dock = System.Windows.Forms.DockStyle.Top;
			list.MultiSelect = false;
			list.Tag = group;
			list.SmallImageList = m_smallImages.ImageList;
			list.LargeImageList = m_largeImages.ImageList;
			list.HideSelection = false;
			group.ReferenceWidget = list;
			//			foreach(ChoiceRelatedClass choice in group)
			//			{
			//				//Debug.Assert(item is ChoiceBase, "Only things that can be made into buttons should be appearing here.");
			//				Debug.Assert(choice is ListPropertyChoice, "Only things that can be made into buttons should be appearing here.");
			//				ListViewItem x =list.Items.Add(choice.Label,1);
			//				x.Tag = choice;
			//			}

			group.OnDisplay(this, null);
			PopulateList(group);

			list.SelectedIndexChanged+=new EventHandler(OnClickInPanelList);
			list.SizeChanged += new EventHandler(list_SizeChanged);

			m_panelSubControls.Add (list);
		}

		void list_SizeChanged(object sender, EventArgs e)
		{
			ListView list = sender as ListView;
			// if you shrink the height of a ListView while it is in the List view enough to cut off
			// some items, the items are shifted to another column and a horizontal scroll bar will appear
			// if necessary, if you scroll over to see the items in the new column and increase the height
			// of the ListView so that all of the items are in one column, the scrollbar will disappear but
			// not scroll back over to make the items visible. I am not sure if this is by design or not. To
			// solve this problem, we check to see if the top item is null, thus indicating that there are no
			// items currently visible, then we scroll back over to make the currently selected item visible.
			if (list.TopItem == null)
			{
				if (list.SelectedItems.Count > 0)
					list.SelectedItems[0].EnsureVisible();
				else if (list.Items.Count > 0)
					list.EnsureVisible(0);

				// This is a really bad hack, but it seems to be the only way that I know of to get around
				// this bug in XP. EnsureVisible doesn't seem to work when we get in to this weird state
				// in XP, so we go ahead and rebuild the entire list.
				if (list.TopItem == null && list.Items.Count > 0)
				{
					ListViewItem selected = null;
					if (list.SelectedItems.Count > 0)
						selected = list.SelectedItems[0];
					ListViewItem[] items = new ListViewItem[list.Items.Count];
					for (int i = 0; i < list.Items.Count; i++)
						items[i] = list.Items[i];
					list.Items.Clear();
					list.Items.AddRange(items);
					if (selected != null)
					{
						selected.Selected = true;
						selected.EnsureVisible();
					}
				}
			}
		}

		/// <summary>
		/// Populate a control to show, for example, the list of tools, or the list of filters.
		/// </summary>
		/// <param name="group"></param>
		protected void PopulateList(ChoiceGroup group)
		{
			bool wasSuspended = m_suspendEvents;
			m_suspendEvents = true;

			if(m_populatingList)
				return;

			m_populatingList=true;
			ListView list = (ListView) group.ReferenceWidget;
//			if(list.Items.Count == group.Count)
//				UpdateList(group);
//			else
			{
				list.BeginUpdate();
				list.Clear();
				foreach(ListPropertyChoice choice in group)
				{
					//Debug.Assert(item is ChoiceBase, "Only things that can be made into buttons should be appearing here.");
					//Debug.Assert(choice is ListPropertyChoice, "Only things that can be made into buttons should be appearing here.");
					ListViewItem x =list.Items.Add(choice.Label, m_smallImages.GetImageIndex(choice.ImageName));
					x.Tag = choice;
					choice.ReferenceWidget = x;
					x.Selected = choice.Checked;
				}
				list.EndUpdate();
			}
			m_populatingList=false;
			m_suspendEvents = wasSuspended;
		}
		/// <summary>
		/// Populate a control to show, for example, the list of tools, or the list of filters.
		/// </summary>
		/// <param name="group"></param>
//		protected void UpdateList(ChoiceGroup group)
//		{
//	//		Debug.WriteLine("Grp:"+group.Label);
//			ListView list = (ListView) group.ReferenceWidget;
//			foreach(ListPropertyChoice choice in group)
//			{
////				Debug.WriteLine(choice.Label);
//				//if(choice.Checked)
//					((ListViewItem)(choice.ReferenceWidget)).Selected = (choice.Checked);
////				else
////					((ListViewItem)(choice.ReferenceWidget)).Selected = (choice.Checked);
////				Debug.WriteLine("foo:");
//
//			}
//		}
		protected ArrayList AddSubControlsForButton (ListPropertyChoice choice)
		{
			ArrayList controls= new ArrayList ();
			foreach(Control control in m_panelSubControls)
			{
				//				if(control.Parent !=null)
				//				{
				//					control.Parent.Controls.Remove(control);
				//					//		//			string s = control.Parent.Name;
				//				}
				if (null != choice.ParameterNode.SelectSingleNode("descendant::listPanel[@listId='"+ control.Name+ "']"))
				{
					controls. Add (control);
					//string x = control.Parent.Name;
				}
			}
			return controls;
		}

		/// <summary>
		/// Handle the Button Click event.
		/// </summary>
		/// <param name="something">The button that was clicked.</param>
		/// <param name="args">Unused event arguments.</param>
		private void OnClickAreaButton(object something, System.EventArgs args)
		{
			ButtonItem item = (ButtonItem)something;
			ChoiceBase choice = (ChoiceBase)item.Tag;
			Debug.Assert(choice != null);
			m_control.SuspendLayout();
			choice.OnClick(item, null);
			m_control.ResumeLayout(true);

		}

		/// <summary>
		///	do an update of the panels assigned to this button in order to, for
		///	example, show the tools that go with the currently selected area button.
		/// </summary>
		/// <param name="panel"></param>
		private void RefreshPanel(NavigationPanePanel panel)
		{
			//			if (panel != null)
			//			{
			//				AddControlsToPanel((ListPropertyChoice)panel.ParentItem.Tag, panel);
			//				//AddSubControlsForButton((ListPropertyChoice)panel.ParentItem.Tag);
			//				panel.Refresh();
			//
			//				((NavigationPane)(m_control)).RecalcLayout();
			//				//				this.MyControl.Refresh();
			//				foreach(Control subcontrol in panel. Controls)
			//				{
			//					if (subcontrol. Tag == null)
			//						continue;
			//					ChoiceGroup group = (ChoiceGroup)subcontrol.Tag;
			//
			//					//this will cause any listeners to be able to update the list,
			//					//and then the list will turn around and request that we
			//					//update the display of the list.
			//					group.OnDisplay(this,null);
			//
			//				}
			//			}
		}

		/// <summary>
		/// triggered when the user clicks on one of the list used in the navigation bar, e.g., the tools list.
		/// </summary>
		/// <param name="something"></param>
		/// <param name="args"></param>
		private void OnClickInPanelList(object something, System.EventArgs args)
		{
			if(			m_suspendEvents)
				return;

			ListView list = (ListView)something;
			if (list.SelectedIndices == null || list.SelectedIndices. Count == 0)
				return;

			ChoiceBase control = (ChoiceBase)list.SelectedItems[0].Tag;
			Debug.Assert(control != null);
			control.OnClick(this, null);
		}

		#endregion Other methods


		/// <summary>
		/// When something changes the property areaChoice (via something like a menubar),
		/// this ensures that the matching button
		/// is highlighted.
		/// </summary>
		public void OnPropertyChanged(string propertyName)
		{

			switch(propertyName)
			{
					//nb:  this is a complete hack (and even this doesn't work right now)
					//this adapter should not need to know about the specifics like that there is a
					//property with his name.

					//NB:this processing could be moved to be on idle eventrather than
					//waiting for this property to change, but then it will
					//waste this time even more often.
				case "currentContentControl":
					if(m_populatingList)
						break;

					//NB: this is a huge waste of effort, but believe it or not
					//I cannot find out
					// 1) which controls are currently visible, nor
					// 2)which controls belong to the current button, nor
					// 3) which list box belongs to the current control
					//	(having a very bad day here).
					//Therefore I have to service all of them!!!
					foreach(Control control in m_panelSubControls)
					{
//						if(control.Visible == false)
//							break;
						ChoiceGroup g = (ChoiceGroup)control.Tag;
						g.OnDisplay(this, null);
					}
					break;
				case "areaChoice":
					string areaName = m_mediator. PropertyTable.GetStringProperty("areaChoice", null);
					//	((ChoiceGroup)this.NavPane.NavigationBar.Tag).OnDisplay(null, null);//!!!!  test
					//RefreshPanel(this.NavPane.SelectedPanel);
					foreach(ButtonItem button in this.NavPane.Items)
					{
						if (button.Name == areaName)
						{
							button.Checked= true;
							break;
						}
					}
					break;

				default: break;
			}
		}

#if nono
		//this did not work out well because this event, though it is a result of clicking and
		//area button, actually comes before the event that tells us the button was changed!
		//therefore, stuff that would set up the list dynamically depending on the area will get it wrong.
		//therefore, we stopped using this and moved to putting all of the handling into the
		//OnClick event for the Area button.
		private void OnPanelVisibleChanged(object sender, EventArgs e)
		{
			NavigationPanePanel panel = (NavigationPanePanel) sender;
			if (!panel.Visible)
				return;
			foreach(Control control in panel. Controls)
			{
				if (control. Tag == null)
					continue;
				ChoiceGroup group = (ChoiceGroup)control.Tag;
				group.OnDisplay(this,null);

			}
		}
#endif

		private void OnPanelLayout(object sender, LayoutEventArgs e)
		{
			NavigationPanePanel p = ((NavigationPanePanel)sender);

			if (p.Controls.Count == 1 && !(p.Controls[0] is PanelEx)) // No header, expand to full height
			{
				p.Controls[0].Height = p.Height;
				return;
			}

			if (p.Controls.Count < 2)//just a header
				return;
			//assume all headers are the same height
			int headerHeight =p.Controls[1].Height; //these are in reverse order so the  last header is actually slot 1 rather than 0

			foreach(Control control in p.Controls)
			{
				//assume that anything of this class is a header
				//(might be a bad assumption someday)
				if (!(control is PanelEx))
				{
					control.Height = (p.Height - (headerHeight*(p.Controls.Count/2)))/(p.Controls.Count/2);
				}
			}
		}
	}
}