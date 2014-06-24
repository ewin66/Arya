using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace Arya.Framework.Common.ComponentModel
{
    public class ExportDesignerVerbSite : IMenuCommandService, ISite
    {

        // our target object
        protected object _Component;

        public ExportDesignerVerbSite(object component)
		{
            _Component = component;
		}

        public void AddCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void AddVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            throw new NotImplementedException();
        }

        public void RemoveCommand(MenuCommand command)
        {
            throw new NotImplementedException();
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            throw new NotImplementedException();
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            throw new NotImplementedException();
        }

        public DesignerVerbCollection Verbs
        {
            get
            {
                var Verbs = new DesignerVerbCollection();
                // Use reflection to enumerate all the public methods on the object
                MethodInfo[] mia = _Component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo mi in mia)
                {
                    // Ignore any methods without a [Browsable(true)] attribute
                    object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (attrs == null || attrs.Length == 0)
                        continue;
                    if (!((BrowsableAttribute)attrs[0]).Browsable)
                        continue;
                    // Add a DesignerVerb with our VerbEventHandler
                    // The method name will appear in the command pane
                    Verbs.Add(new DesignerVerb(mi.Name, new EventHandler(VerbEventHandler)));
                }
                return Verbs;
            }
        }

        private void VerbEventHandler(object sender, EventArgs e)
        {
            // The verb is the sender
            var verb = sender as DesignerVerb;
            // Enumerate the methods again to find the one named by the verb
            MethodInfo[] mia = _Component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo mi in mia)
            {
                object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;
                if (!((BrowsableAttribute)attrs[0]).Browsable)
                    continue;
                if (verb.Text == mi.Name)
                {
                    // Invoke the method on our object (no parameters)
                    mi.Invoke(_Component, null);
                    return;
                }
            }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IMenuCommandService))
                return this;
            return null;
        }

        public IComponent Component
        {
            get { throw new NotImplementedException(); }
        }

        public IContainer Container
        {
            get { return null; }
        }

        public bool DesignMode
        {
            get { return true; }
        }

        [Browsable(false)]
        public string Name
        {
            get { return  _Component.ToString(); }
            set { throw new NotImplementedException(); }
        }
    }
}
