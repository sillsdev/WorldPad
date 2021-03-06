// This really needs to be refactored with MasterCategoryListDlg.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.Common.Utils;
using SIL.Utils;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.LexText.Controls.MGA;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Common.Framework;
using XCore;

namespace SIL.FieldWorks.LexText.Controls
{
	/// <summary>
	/// Summary description for MsaInflectionFeatureListDlg.
	/// </summary>
	public class MsaInflectionFeatureListDlg : Form, IFWDisposable
	{
		private Mediator m_mediator;
		private FdoCache m_cache;
		// The dialog can be initialized with an existing feature structure,
		// or just with an owning object and flid in which to create one.
		private IFsFeatStruc m_fs;
		// Where to put a new feature structure if needed. Owning flid may be atomic
		// or collection. Used only if m_fs is initially null.
		int m_hvoOwner;
		int m_owningFlid;
		private IPartOfSpeech m_highestPOS;
		private Dictionary<int, IPartOfSpeech> m_poses = new Dictionary<int, IPartOfSpeech>();
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_bnHelp;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.ImageList m_imageList;
		private System.Windows.Forms.ImageList m_imageListPictures;
		private FeatureStructureTreeView m_tvMsaFeatureList;
		private System.Windows.Forms.Label labelPrompt;
		private System.ComponentModel.IContainer components;

		private const string s_helpTopic = "khtpChoose-InflFeats";
		private System.Windows.Forms.HelpProvider helpProvider;

		public MsaInflectionFeatureListDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if(FwApp.App != null) // Will be null when running tests
			{
				this.helpProvider = new System.Windows.Forms.HelpProvider();
				this.helpProvider.HelpNamespace = FwApp.App.HelpFile;
				this.helpProvider.SetHelpKeyword(this, FwApp.App.GetHelpString(s_helpTopic, 0));
				this.helpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
			}
			pictureBox1.Image = m_imageListPictures.Images[0];
		}
		#region OnLoad
		/// <summary>
		/// Overridden to defeat the standard .NET behavior of adjusting size by
		/// screen resolution. That is bad for this dialog because we remember the size,
		/// and if we remember the enlarged size, it just keeps growing.
		/// If we defeat it, it may look a bit small the first time at high resolution,
		/// but at least it will stay the size the user sets.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			Size size = this.Size;
			base.OnLoad (e);
			if (this.Size != size)
				this.Size = size;
		}
		#endregion

		/// <summary>
		/// Check to see if the object has been disposed.
		/// All public Properties and Methods should call this
		/// before doing anything else.
		/// </summary>
		public void CheckDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(String.Format("'{0}' in use after being disposed.", GetType().Name));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			m_cache = null;
			m_fs = null;
			m_highestPOS = null;
			m_poses = null;
			m_mediator = null;
			m_cache = null;

			base.Dispose( disposing );
		}

		/// <summary>
		/// Init the dialog with an existing FS. Warning: the fs passed in
		/// might get deleted if it proves to be a duplicate. Retrieve the new FS after running it.
		/// This constructor is used in MsaInflectionFeatureListDlgLauncher.HandleChooser.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="mediator"></param>
		/// <param name="fs"></param>
		public void SetDlgInfo(FdoCache cache, Mediator mediator, IFsFeatStruc fs)
		{
			CheckDisposed();

			m_fs = fs;
			RestoreWindowPosition(mediator);
			m_cache = cache;
			LoadInflFeats(fs);
			EnableLink();
		}

		/// <summary>
		/// Init the dialog with an MSA and flid that does not yet contain a feature structure.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="mediator"></param>
		/// <param name="cobj"></param>
		/// <param name="owningFlid"></param>
		public void SetDlgInfo(FdoCache cache, Mediator mediator, ICmObject cobj, int owningFlid)
		{
			CheckDisposed();

			m_fs = null;
			m_owningFlid = owningFlid;
			m_hvoOwner = cobj.Hvo;
			RestoreWindowPosition(mediator);
			m_cache = cache;
			LoadInflFeats(cobj, owningFlid);
			EnableLink();
		}
		/// <summary>
		/// Init the dialog with a POS.
		/// If a new feature structure is created, it will currently be in the ReferenceForms of the POS.
		/// Eventually we want to make a new field for this purpose. (This is used by bulk edit
		/// to store previously used feature structures.)
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="mediator"></param>
		/// <param name="pos"></param>
		public void SetDlgInfo(FdoCache cache, Mediator mediator, IPartOfSpeech pos)
		{
			CheckDisposed();

			m_fs = null;
			m_owningFlid = (int)PartOfSpeech.PartOfSpeechTags.kflidReferenceForms;
			m_hvoOwner = pos.Hvo;
			RestoreWindowPosition(mediator);
			m_cache = cache;
			LoadInflFeats(pos);
			EnableLink();
		}

		private void EnableLink()
		{
			if (m_highestPOS == null)
			{
				linkLabel1.Enabled = false;
			}
			else
			{
				linkLabel1.Enabled = true;
			}
		}

		private void RestoreWindowPosition(Mediator mediator)
		{
			m_mediator = mediator;
			if (mediator != null)
			{
				// Reset window location.
				// Get location to the stored values, if any.
				object locWnd = m_mediator.PropertyTable.GetValue("msaInflFeatListDlgLocation");
				object szWnd = m_mediator.PropertyTable.GetValue("msaInflFeatListDlgSize");
				if (locWnd != null && szWnd != null)
				{
					Rectangle rect = new Rectangle((Point)locWnd, (Size)szWnd);
					ScreenUtils.EnsureVisibleRect(ref rect);
					DesktopBounds = rect;
					StartPosition = FormStartPosition.Manual;
				}
			}
		}

		/// <summary>
		/// Load the tree items if the starting point is a feature structure.
		/// </summary>
		/// <param name="fs"></param>
		private void LoadInflFeats(IFsFeatStruc fs)
		{
			ICmObject cobj = CmObject.CreateFromDBObject(fs.Cache, fs.OwnerHVO);
			switch(cobj.ClassID)
			{
				case MoAffixAllomorph.kclsidMoAffixAllomorph:
					PopulateTreeFromPosInEntry(cobj);
					break;
				default:
					// load inflectable features of this POS and any inflectable features of its parent POS
					IPartOfSpeech pos = GetOwningPOSOfFS(fs, cobj);
					PopulateTreeFromPos(pos);
					break;
			}
			m_tvMsaFeatureList.PopulateTreeFromFeatureStructure(fs);
			FinishLoading();
		}

		/// <summary>
		/// Load the tree items if the starting point is a POS.
		/// </summary>
		/// <param name="pos"></param>
		private void LoadInflFeats(IPartOfSpeech pos)
		{
			PopulateTreeFromPos(pos);
			FinishLoading();
		}

		/// <summary>
		/// Load the tree items if the starting point is an owning MSA and flid.
		/// </summary>
		/// <param name="cobj"></param>
		/// <param name="owningFlid"></param>
		private void LoadInflFeats(ICmObject cobj, int owningFlid)
		{
			switch(cobj.ClassID)
			{
				case MoAffixAllomorph.kclsidMoAffixAllomorph:
					PopulateTreeFromPosInEntry(cobj);
					break;
				default:
					PopulateTreeFromPos(GetPosFromCmObjectAndFlid(cobj, owningFlid));
					break;
			}
			FinishLoading();
		}

		private void PopulateTreeFromPosInEntry(ICmObject cobj)
		{
			ILexEntry entry = LexEntry.CreateFromDBObject(m_cache, cobj.OwnerHVO);
			if (entry == null)
				return;
			foreach (IMoMorphSynAnalysis msa in entry.MorphoSyntaxAnalysesOC)
			{
				IPartOfSpeech pos = GetPosFromCmObjectAndFlid(msa, (int)MoDerivAffMsa.MoDerivAffMsaTags.kflidFromMsFeatures);
				PopulateTreeFromPos(pos);
			}
		}

		/// <summary>
		/// After populating the tree with items, expand them, sort them, and select one.
		/// </summary>
		private void FinishLoading()
		{
			m_tvMsaFeatureList.ExpandAll();
			m_tvMsaFeatureList.Sort();
			if (m_tvMsaFeatureList.Nodes.Count > 0)
				m_tvMsaFeatureList.SelectedNode = m_tvMsaFeatureList.Nodes[0]; // have it show first one initially
		}

		private void PopulateTreeFromPos(IPartOfSpeech pos)
		{
			if (pos != null && !m_poses.ContainsKey(pos.Hvo))
			{
				m_poses.Add(pos.Hvo, pos);
			}
			m_highestPOS = pos;
			while (pos != null)
			{
				m_tvMsaFeatureList.PopulateTreeFromInflectableFeats(pos.InflectableFeatsRC);
				ICmObject cobj = CmObject.CreateFromDBObject(pos.Cache, pos.OwnerHVO);
				m_highestPOS = pos;
				pos = cobj as IPartOfSpeech;
			}
		}

		private IPartOfSpeech GetOwningPOSOfFS(IFsFeatStruc fs, ICmObject cobj)
		{
			int owningFlid = fs.OwningFlid;
			return GetPosFromCmObjectAndFlid(cobj, owningFlid);
		}

		/// <summary>
		/// Given a (potentially) owning object, and the flid in which is does/will own
		/// the feature structure, find the relevant POS.
		/// </summary>
		/// <param name="cobj"></param>
		/// <param name="owningFlid"></param>
		/// <returns></returns>
		private IPartOfSpeech GetPosFromCmObjectAndFlid(ICmObject cobj, int owningFlid)
		{
			IPartOfSpeech pos = null;
			switch (cobj.ClassID)
			{
				case MoInflAffMsa.kclsidMoInflAffMsa:
					IMoInflAffMsa infl = cobj as IMoInflAffMsa;
					if (infl != null)
						pos = infl.PartOfSpeechRA;
					break;
				case MoDerivAffMsa.kclsidMoDerivAffMsa:
					IMoDerivAffMsa deriv = cobj as IMoDerivAffMsa;
					if (deriv != null)
					{
						if (owningFlid == (int)MoDerivAffMsa.MoDerivAffMsaTags.kflidFromMsFeatures)
							pos = deriv.FromPartOfSpeechRA;
						else if (owningFlid == (int)MoDerivAffMsa.MoDerivAffMsaTags.kflidToMsFeatures)
							pos = deriv.ToPartOfSpeechRA;
					}
					break;
				case MoStemMsa.kclsidMoStemMsa:
					IMoStemMsa stem = cobj as IMoStemMsa;
					if (stem != null)
						pos = stem.PartOfSpeechRA;
					break;
				case MoStemName.kclsidMoStemName:
					IMoStemName sn = cobj as IMoStemName;
					pos = PartOfSpeech.CreateFromDBObject(sn.Cache, sn.OwnerHVO);
					break;
				case MoAffixAllomorph.kclsidMoAffixAllomorph:
					// get entry of the allomorph and then get the msa of first sense and return its (from) POS
					ILexEntry entry = LexEntry.CreateFromDBObject(m_cache, cobj.OwnerHVO);
					if (entry == null)
						return pos;
					ILexSense sense = entry.SensesOS.FirstItem;
					if (sense == null)
						return pos;
					IMoMorphSynAnalysis msa = sense.MorphoSyntaxAnalysisRA;
					pos = GetPosFromCmObjectAndFlid(msa, (int)MoDerivAffMsa.MoDerivAffMsaTags.kflidFromMsFeatures);
					break;
			}
			return pos;
		}

		/// <summary>
		/// Get Feature Structure resulting from dialog operation
		/// </summary>
		public IFsFeatStruc FS
		{
			get
			{
				CheckDisposed();

				return m_fs;
			}
		}
		/// <summary>
		/// Get highest level POS of msa
		/// </summary>
		public IPartOfSpeech HighestPOS
		{
			get
			{
				CheckDisposed();

				return m_highestPOS;
			}
		}
		/// <summary>
		/// Get/Set prompt text
		/// </summary>
		public string Prompt
		{
			get
			{
				CheckDisposed();

				return labelPrompt.Text;
			}
			set
			{
				CheckDisposed();

				string s1;
				if (value == null)
					s1 = LexTextControls.ksFeaturesForX;
				else
					s1 = value;
				string s2;
				if (m_poses.Count == 0)
					s2 = LexTextControls.ksUnknownCategory;
				else
				{
					StringBuilder sb = new StringBuilder();
					Dictionary<int, IPartOfSpeech>.ValueCollection poses = m_poses.Values;
					bool fFirst = true;
					foreach (IPartOfSpeech pos in poses)
					{
						if (!fFirst)
							sb.Append(", ");
						sb.Append(pos.Name.BestAnalysisAlternative.Text);
						fFirst = false;
					}
					s2 = sb.ToString();
				}
				labelPrompt.Text = String.Format(s1, s2);
			}
		}
		/// <summary>
		/// Get/Set dialog title text
		/// </summary>
		public string Title
		{
			get
			{
				CheckDisposed();

				return Text;
			}
			set
			{
				CheckDisposed();

				Text = value;
			}
		}
		/// <summary>
		/// Get/Set link text
		/// </summary>
		public string LinkText
		{
			get
			{
				CheckDisposed();

				return linkLabel1.Text;
			}
			set
			{
				CheckDisposed();

				string s1;
				if (value == null)
					s1 = LexTextControls.ksAddFeaturesToX;
				else
					s1 = value;
				string s2;
				if (m_highestPOS == null)
					s2 = LexTextControls.ksUnknownCategory;
				else
					s2 = m_highestPOS.Name.AnalysisDefaultWritingSystem;
				linkLabel1.Text = String.Format(s1, s2);
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MsaInflectionFeatureListDlg));
			this.labelPrompt = new System.Windows.Forms.Label();
			this.m_imageList = new System.Windows.Forms.ImageList(this.components);
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_bnHelp = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.m_imageListPictures = new System.Windows.Forms.ImageList(this.components);
			this.m_tvMsaFeatureList = new SIL.FieldWorks.LexText.Controls.FeatureStructureTreeView(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// labelPrompt
			//
			resources.ApplyResources(this.labelPrompt, "labelPrompt");
			this.labelPrompt.Name = "labelPrompt";
			//
			// m_imageList
			//
			this.m_imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageList.ImageStream")));
			this.m_imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.m_imageList.Images.SetKeyName(0, "");
			this.m_imageList.Images.SetKeyName(1, "");
			this.m_imageList.Images.SetKeyName(2, "");
			//
			// m_btnOK
			//
			resources.ApplyResources(this.m_btnOK, "m_btnOK");
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Name = "m_btnOK";
			//
			// m_btnCancel
			//
			resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Name = "m_btnCancel";
			//
			// m_bnHelp
			//
			resources.ApplyResources(this.m_bnHelp, "m_bnHelp");
			this.m_bnHelp.Name = "m_bnHelp";
			this.m_bnHelp.Click += new System.EventHandler(this.m_bnHelp_Click);
			//
			// pictureBox1
			//
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			//
			// linkLabel1
			//
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// m_imageListPictures
			//
			this.m_imageListPictures.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListPictures.ImageStream")));
			this.m_imageListPictures.TransparentColor = System.Drawing.Color.Magenta;
			this.m_imageListPictures.Images.SetKeyName(0, "");
			//
			// m_tvMsaFeatureList
			//
			resources.ApplyResources(this.m_tvMsaFeatureList, "m_tvMsaFeatureList");
			this.m_tvMsaFeatureList.FullRowSelect = true;
			this.m_tvMsaFeatureList.HideSelection = false;
			this.m_tvMsaFeatureList.Name = "m_tvMsaFeatureList";
			//
			// MsaInflectionFeatureListDlg
			//
			this.AcceptButton = this.m_btnOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.m_btnCancel;
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.m_bnHelp);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.labelPrompt);
			this.Controls.Add(this.m_tvMsaFeatureList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MsaInflectionFeatureListDlg";
			this.ShowInTaskbar = false;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MsaInflectionFeatureListDlg_Closing);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// If OK, then make FS have the selected feature value(s).
		/// JohnT: This is a really ugly kludge, which I have only partly repaired.
		/// We need the dialog to return with m_fs set to an FsFeatStruc (if OK was clicked),
		/// since that is what the bulk edit bar wants to copy to MoStemMsas for any items
		/// it is asked to modify. Also, the new FsFeatStruc needs to be in the ReferenceForms
		/// (which is what m_owningFlid apparently always is, currently) so that it will become
		/// one of the items in the combo list and can be selected. However, Andy says this is
		/// not the intended use of ReferenceForms at all.
		/// A further ugliness is that we always make a new FsFeatStruc (unless one was passed
		/// in to one of the SegDlgInfo methods, but AFAIK that override is never used), but
		/// we then delete it if it turns out to be a duplicate. There is no other straightforward
		/// way to detect that the current choices in the dialog correspond to an existing item.
		/// This may cause problems in the new world, where we can't do this "suppress sub tasks"
		/// trick without losing our Undo stack.
		/// It may be possible in the new world to create an object without initially giving it an
		/// owner, and only persist it if it is NOT a duplicate. But even that we don't really want
		/// to be undoable, nor should it clear the undo stack. Really the list of possible choices
		/// for the combo should not be separately persisted as model data, but it should be persisted
		/// somehow...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MsaInflectionFeatureListDlg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
			{
				using (new SuppressSubTasks(m_cache)) // making and maybe then deleting the new item for the combo is not undoable
				{
					if (m_fs == null)
					{
						// Didn't have one to begin with. See whether we want to create one.
						if (CheckFeatureStructure(m_tvMsaFeatureList.Nodes))
						{
							// The last argument is meaningless since we expect this property to be owning
							// or collection.
							int hvoFs = m_cache.CreateObject(FsFeatStruc.kClassId, m_hvoOwner, m_owningFlid, 0);
							m_fs = (IFsFeatStruc) FsFeatStruc.CreateFromDBObject(m_cache, hvoFs, false);
						}
						else
						{
							return; // leave it null.
						}
					}
					// clean out any extant features in the feature structure
					foreach (IFsFeatureSpecification spec in m_fs.FeatureSpecsOC)
						m_fs.FeatureSpecsOC.Remove(spec);
					UpdateFeatureStructure(m_tvMsaFeatureList.Nodes);
					// The (usually) newly created one may be a duplicate. If we find a duplicate
					// delete the one we just made (or were passed) and return the duplicate.
					int chvo = m_cache.GetVectorSize(m_hvoOwner, m_owningFlid);
					for (int ihvo = 0; ihvo < chvo; ihvo++)
					{
						int hvo = m_cache.GetVectorItem(m_hvoOwner, m_owningFlid, ihvo);
						if (hvo == m_fs.Hvo)
							continue;
						IFsFeatStruc fs = CmObject.CreateFromDBObject(m_cache, hvo) as IFsFeatStruc;
						if (FsFeatStruc.AreEquivalent(fs, m_fs))
						{
							m_fs.DeleteUnderlyingObject();
							m_fs = fs;
							break;
						}
					}
				}
			}

			if (m_mediator != null)
			{
				m_mediator.PropertyTable.SetProperty("msaInflFeatListDlgLocation", Location);
				m_mediator.PropertyTable.SetProperty("msaInflFeatListDlgSize", Size);
			}
		}

		/// <summary>
		/// Answer true if the tree node collection, passed to UpdateFeatureStructure,
		/// will produce a non-empty feature structure.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		private bool CheckFeatureStructure(TreeNodeCollection col)
		{
			foreach (FeatureTreeNode tn in col)
			{
				if (tn.Nodes.Count > 0)
				{
					if (CheckFeatureStructure(tn.Nodes))
						return true;
				}
				else if (tn.Chosen && (0 != tn.Hvo))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Makes the feature structure reflect the values chosen in the treeview
		/// </summary>
		/// <remarks>Is public for Unit Testing</remarks>
		/// <param name="col">collection of nodes at this level</param>
		public void UpdateFeatureStructure(TreeNodeCollection col)
		{
			CheckDisposed();

			foreach (FeatureTreeNode tn in col)
			{
				if (tn.Nodes.Count > 0)
					UpdateFeatureStructure(tn.Nodes);
				else if (tn.Chosen && (0 != tn.Hvo))
				{
					IFsFeatStruc fs = m_fs;
					IFsFeatureSpecification val = null;
					// add any terminal nodes to db
					BuildFeatureStructure(tn, ref fs, ref val);
				}
			}
		}
		/// <summary>
		/// Recursively builds the feature structure based on contents of treeview node path.
		/// It recurses back up the treeview node path to the top and then builds the feature structure
		/// as it goes back down.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="fs"></param>
		/// <returns></returns>
		private void BuildFeatureStructure(FeatureTreeNode node, ref IFsFeatStruc fs, ref IFsFeatureSpecification val)
		{
			if (node.Parent != null)
				BuildFeatureStructure((FeatureTreeNode)node.Parent, ref fs, ref val);
			switch (node.Kind)
			{
				case FeatureTreeNodeInfo.NodeKind.Complex:
					IFsComplexValue complex = fs.FindOrCreateComplexValue(node.Hvo);
					val = complex as FsComplexValue;
					val.FeatureRAHvo = node.Hvo;
					if (fs.TypeRA == null)
					{
						// this is the type which contains the complex feature
						fs.TypeRAHvo = FsFeatureSystem.GetTypeFromFsComplexFeature(m_cache, node.Hvo);
					}
					fs = (IFsFeatStruc)complex.ValueOA;
					if (fs.TypeRA == null)
					{
						// this is the type of what's being embedded in the fs
						IFsComplexFeature cf = val.FeatureRA as IFsComplexFeature;
						if (cf != null)
						{
							fs.TypeRA = cf.TypeRA;
						}
					}
					break;
				case FeatureTreeNodeInfo.NodeKind.Closed:
					val = (IFsClosedValue)fs.FindOrCreateClosedValue(node.Hvo);
					val.FeatureRAHvo = node.Hvo;
					break;
				case FeatureTreeNodeInfo.NodeKind.SymFeatValue:
					IFsClosedValue closed = val as IFsClosedValue;
					if (closed != null)
						closed.ValueRAHvo = node.Hvo;
					break;
				default:
					break; // do nothing
			}
		}
		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			if (m_highestPOS == null)
				return;  // nowhere to go
			// code in the launcher handles the jump
			DialogResult = DialogResult.Yes;
			Close();
		}

		private void m_bnHelp_Click(object sender, System.EventArgs e)
		{
			ShowHelp.ShowHelpTopic(FwApp.App, s_helpTopic);
		}
	}
}
