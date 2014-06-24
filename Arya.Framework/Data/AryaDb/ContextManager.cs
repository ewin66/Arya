using System;

namespace Arya.Framework.Data.AryaDb
{
    public sealed class ContextManager
    {
        // ReSharper disable InconsistentNaming
        private static readonly ContextManager _instance = new ContextManager();
        // ReSharper restore InconsistentNaming

        private static readonly object Padlock = new object();
        private AryaDbDataContext _currentProjectContext;

        //Do not remove this.
        static ContextManager() { }

        private ContextManager() { }

        public static ContextManager Instance
        {
            get { return _instance; }
        }

        public AryaDbDataContext CurrentProjectContext
        {
            get
            {
                if (_currentProjectContext == null)
                {
                    throw new InvalidOperationException(
                        "Project Context is not initiated, please call the InitContext() method to initialize the project");
                }
                return _currentProjectContext;
            }
        }

        /// <summary>
        /// Initializes the singleton context to the provided project & disposes the old context if it exists.
        /// </summary>
        /// <param name="projectID">Project to which the context needs to switch</param>
        /// <param name="userID">User that is going to perform the actions</param>
        public void InitContext(Guid projectID, Guid userID)
        {
            lock (Padlock)
            {
                if (_instance._currentProjectContext != null)
                    _instance._currentProjectContext.Dispose();
                _instance._currentProjectContext = new AryaDbDataContext(projectID, userID);
            }
        }
    }
}