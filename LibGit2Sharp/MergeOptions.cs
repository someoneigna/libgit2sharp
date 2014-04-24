﻿using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;

namespace LibGit2Sharp
{
    /// <summary>
    /// Options controlling Merge behavior.
    /// </summary>
    public sealed class MergeOptions : IConvertableToGitCheckoutOpts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeOptions"/> class.
        /// By default, a fast-forward merge will be performed if possible, and
        /// if a merge commit is created, then it will be commited.
        /// </summary>
        public MergeOptions()
        {
            CommitOnSuccess = true;
        }

        /// <summary>
        /// The Flags specifying what conditions are
        /// reported through the OnCheckoutNotify delegate.
        /// </summary>
        public CheckoutNotifyFlags CheckoutNotifyFlags { get; set; }

        /// <summary>
        /// Commit the merge if the merge is successful and this is a non-fast-forward merge.
        /// If this is a fast-forward merge, then there is no merge commit and this option
        /// will not affect the merge.
        /// </summary>
        public bool CommitOnSuccess { get; set; }

        /// <summary>
        /// The type of merge to perform.
        /// </summary>
        public FastForwardStrategy FastForwardStrategy { get; set; }

        /// <summary>
        /// How Checkout should handle writing out conflicting index entries.
        /// </summary>
        public CheckoutFileConflictStrategy FileConflictStrategy { get; set; }

        /// <summary>
        /// How to handle conflicts encountered during a merge.
        /// </summary>
        public MergeFileFavor MergeFileFavor { get; set; }

        /// <summary>
        /// Delegate that checkout progress will be reported through.
        /// </summary>
        public CheckoutProgressHandler OnCheckoutProgress { get; set; }

        /// <summary>
        /// Delegate through which checkout will notify callers of
        /// certain conditions. The conditions that are reported is
        /// controlled with the CheckoutNotifyFlags property.
        /// </summary>
        public CheckoutNotifyHandler OnCheckoutNotify { get; set; }

        #region IConvertableToGitCheckoutOpts

        CheckoutCallbacks IConvertableToGitCheckoutOpts.GenerateCallbacks()
        {
            return CheckoutCallbacks.From(OnCheckoutProgress, OnCheckoutNotify);
        }

        CheckoutStrategy IConvertableToGitCheckoutOpts.CheckoutStrategy
        {
            get
            {
                return CheckoutStrategy.GIT_CHECKOUT_SAFE_CREATE |
                       CheckoutStrategy.GIT_CHECKOUT_ALLOW_CONFLICTS |
                       GitCheckoutOptsWrapper.CheckoutStrategyFromFileConflictStrategy(FileConflictStrategy);
            }
        }

        #endregion
    }

    /// <summary>
    /// Strategy used for merging.
    /// </summary>
    public enum FastForwardStrategy
    {
        /// <summary>
        /// Default fast-forward strategy. This will perform a fast-forward merge
        /// if possible, otherwise will perform a non-fast-forward merge that
        /// results in a merge commit.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Do not fast-forward. Always creates a merge commit.
        /// </summary>
        NoFastFoward = 1, /* GIT_MERGE_NO_FASTFORWARD */

        /// <summary>
        /// Only perform fast-forward merges.
        /// </summary>
        FastForwardOnly = 2, /* GIT_MERGE_FASTFORWARD_ONLY */
    }

    /// <summary>
    /// Enum specifying how merge should deal with conflicting regions
    /// of the files.
    /// </summary>
    public enum MergeFileFavor
    {
        /// <summary>
        /// When a region of a file is changed in both branches, a conflict
        /// will be recorded in the index so that `git_checkout` can produce
        /// a merge file with conflict markers in the working directory.
        /// This is the default.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// When a region of a file is changed in both branches, the file
        /// created in the index will contain the "ours" side of any conflicting
        /// region. The index will not record a conflict.
        /// </summary>
        Ours = 1,

        /// <summary>
        /// When a region of a file is changed in both branches, the file
        /// created in the index will contain the "theirs" side of any conflicting
        /// region. The index will not record a conflict.
        /// </summary>
        Theirs = 2,

        /// <summary>
        /// When a region of a file is changed in both branches, the file
        /// created in the index will contain each unique line from each side,
        /// which has the result of combining both files. The index will not
        /// record a conflict.
        /// </summary>
        Union = 3,
    }
}
