﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    /// <summary>
    /// The collection of <see cref="Remote"/> in a <see cref="Repository"/>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RemoteCollection : IEnumerable<Remote>
    {
        private readonly Repository repository;

        /// <summary>
        /// Needed for mocking purposes.
        /// </summary>
        protected RemoteCollection()
        { }

        internal RemoteCollection(Repository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the <see cref="Remote"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the remote to retrieve.</param>
        /// <returns>The retrived <see cref="Remote"/> if it has been found, null otherwise.</returns>
        public virtual Remote this[string name]
        {
            get { return RemoteForName(name, false); }
        }

        internal Remote RemoteForName(string name, bool shouldThrowIfNotFound = true)
        {
            Ensure.ArgumentNotNullOrEmptyString(name, "name");

            using (RemoteSafeHandle handle = Proxy.git_remote_load(repository.Handle, name, shouldThrowIfNotFound))
            {
                return handle == null ? null : Remote.BuildFromPtr(handle, this.repository);
            }
        }

        /// <summary>
        /// Update properties of a remote.
        /// </summary>
        /// <param name="remote">The remote to update.</param>
        /// <param name="actions">Delegate to perform updates on the remote.</param>
        /// <returns>The updated remote.</returns>
        public virtual Remote Update(Remote remote, params Action<RemoteUpdater>[] actions)
        {
            var updater = new RemoteUpdater(this.repository, remote);

            foreach (Action<RemoteUpdater> action in actions)
            {
                action(updater);
            }

            return this[remote.Name];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<Remote> GetEnumerator()
        {
            return Proxy
                .git_remote_list(repository.Handle)
                .Select(n => this[n])
                .GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a <see cref="Remote"/> with the specified name and for the repository at the specified location.
        /// <para>
        ///   A default fetch refspec will be added for this remote.
        /// </para>
        /// </summary>
        /// <param name="name">The name of the remote to create.</param>
        /// <param name="url">The location of the repository.</param>
        /// <returns>A new <see cref="Remote"/>.</returns>
        public virtual Remote Add(string name, string url)
        {
            Ensure.ArgumentNotNullOrEmptyString(name, "name");
            Ensure.ArgumentNotNullOrEmptyString(url, "url");

            using (RemoteSafeHandle handle = Proxy.git_remote_create(repository.Handle, name, url))
            {
                return Remote.BuildFromPtr(handle, this.repository);
            }
        }

        /// <summary>
        /// Creates a <see cref="Remote"/> with the specified name and for the repository at the specified location.
        /// </summary>
        /// <param name="name">The name of the remote to create.</param>
        /// <param name="url">The location of the repository.</param>
        /// <param name="fetchRefSpec">The refSpec to be used when fetching from this remote.</param>
        /// <returns>A new <see cref="Remote"/>.</returns>
        public virtual Remote Add(string name, string url, string fetchRefSpec)
        {
            Ensure.ArgumentNotNullOrEmptyString(name, "name");
            Ensure.ArgumentNotNullOrEmptyString(url, "url");
            Ensure.ArgumentNotNull(fetchRefSpec, "fetchRefSpec");

            using (RemoteSafeHandle handle = Proxy.git_remote_create_with_fetchspec(repository.Handle, name, url, fetchRefSpec))
            {
                return Remote.BuildFromPtr(handle, this.repository);
            }
        }

        /// <summary>
        /// Determines if the proposed remote name is well-formed.
        /// </summary>
        /// <param name="name">The name to be checked.</param>
        /// <returns>true is the name is valid; false otherwise.</returns>
        [Obsolete("This method will be removed in the next release. Please use Remote.IsValidName(string) instead.")]
        public virtual bool IsValidName(string name)
        {
            return Remote.IsValidName(name);
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "Count = {0}", this.Count());
            }
        }
    }
}
