using System;
using System.Reflection;
using System.Windows.Forms;

namespace Arya.Framework.Extensions
{
	public static class DataGridViewExtensions
	{
		#region Methods (1) 

		// Public Methods (1) 

		public static void DoubleBuffered(this DataGridView dgv, bool setting)
		{
			Type dgvType = dgv.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dgv, setting, null);
		}

		#endregion Methods 
	}
}