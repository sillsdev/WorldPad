// This file was generated by C# Refactory.
// To modify this template, go to Tools/Options/C# Refactory/Code

using System.Windows.Forms;
using SIL.Utils;

using Reflector.UserInterface;

namespace XCore
{
	public abstract class CommandBarAdaptor
	{
		protected Form m_window;
		protected ImageCollection m_smallImages;
		protected ImageCollection m_largeImages;
		protected CommandBarManager m_commandBarManager = new CommandBarManager();

		public CommandBarAdaptor()
		{
			m_commandBarManager.Name = "CommandBarManager";
		}

		/// <summary>
		/// get the control which is shared by all subclasses
		/// </summary>
		/// <returns></returns>
		public bool GetCommandBarManager()
		{
			//see if a menubar has already created one of these command bar managers
			foreach(Control control in m_window.Controls)
			{
				if (control is CommandBarManager)
				{
					m_commandBarManager = (CommandBarManager)control;
					m_commandBarManager.Name = "CommandBarManager";
					return false;
				}
			}

			m_commandBarManager = new CommandBarManager();
			m_commandBarManager.Name = "CommandBarManager";
			return true;
		}

		public void FinishInit()
		{
			//get us to be drawn last so that we can truly own the top of the window.
			m_commandBarManager.SendToBack();
		}

	}
}