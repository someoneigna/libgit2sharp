﻿using System.IO;
using System.Linq;
        private static readonly string subBranchFilePath = Path.Combine("1", "branch_file.txt");

                Assert.Equal(subBranchFilePath, changes.Added.Single().Path);
                Assert.Equal(new[] { "1.txt", subBranchFilePath, "README", "branch_file.txt", "deleted_staged_file.txt", "deleted_unstaged_file.txt", "modified_staged_file.txt", "modified_unstaged_file.txt", "new.txt" },