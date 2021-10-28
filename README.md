# File-level Comparison for Veeam Restore Points

Here are instructions on how to use this template:

* Navigate to the [main page of this repository](https://github.com/VeeamHub/veeamhub-template)
* Follow instructions for [creating a repository from a template](https://help.github.com/en/articles/creating-a-repository-from-a-template)
  * _Repository names **must** have the prefix:_ **veeam-**
  * Example: _veeam-repo-name_
* Replace **all** repository-specific links
  * `issues` link in _README.md_
  * `create an issue` link in _README.md_
  * `opening a new issue` link in _CONTRIBUTING.md_
* Remove the instructions from this section and replace it with information describing this repository.

## 📗 Documentation

**Author:** Ronn Martin (ronn.martin@veeam.com)

**Function:** This Windows-based utility leverages the Veeam Data Integration API to mount backup points for file-level comparison useful for helping determine the filesystem changes that have caused some sort of restore point size anomoly e.g. daily increment size 200% larger than normal or the like.  The utility also provides for comparing a restore point to a running workload (VM or other). This scenario is outlined in the usage section below and can help identify files added or deleted since a prior backup run for instance.

**Requires:** Veeam Powershell module (v11+), .NET Framework 4.6.2

**Operation:** Only Windows workload backups are currently supported. The utility runs with full Windows backup operator privilege which requires administrator execution permissions. If "Compare" is selected directly following the completion of the mount operations, all disks in the backup are compared. It is highly reccommended that only one disk volume or folder w/in a volume are compared at a time. The application can consume considerable RAM resources and take several minutes to run comparisons of entire backup images e.g. for a Windows 10 backup which includes MySQL, Python, Visual Studio, IIS, etc. a full comparison can take up to 10 minutes and consume 3GB+ RAM. File level differences are flagged and displayed if a file exists in only one of the restore points, if the file size changes, if the file modification date changes or if the file's encryption status changes between restore points.

**Usage:** After installing to a Windows instance that meets the above requirements the application will reside in "\Program Files\Veeam Skunkworks\Veeam Restore Point File Diff\VeeamRestorePointDiff.exe". A start menu shortcut is also added for convenience.
For normal operation, launch the application as administrator and select the Windows workload for comparison from the list of backups and affiliated workloads. Next choose two restore points to compare from the list of available restore points as well as mount server credentials.  Selecting "Next" mounts the backup points (\VeeamFLR\...) and presents the dialog for running comparisons. As noted above if all disks should be compared simply select "Compare" otherwise select the file browse dialog "..." buttons to choose the appropriate sub-folders of the mounted backups for comparison. Once comparisons have completed, any files unique to point "A" or point "B" will be displayed in the left and right columns respectively.  Any files that have changed will show in the center column. Note that the sizes displayed for the changed files will be file "A" size - file "B" size. There may be a number of files/folders which show size 0. These may be accounted for if the encryption status only has changed or if the files are symbolically linked but the "real" file changed. Otherwise the respective file sizes are rolled up to the parent folder levels to expedite identification of change source(s).

*Compare to live* - If the static **"<workload name> Live VM/workload"** option is chosen from the point selection dialog the backup restore point selected will be mounted as noted above. It will then be incumbent on the user to map a drive/folder from the live VM/workload with the file browse dialog to compare with the corresponding mounted backup filesystem. Otherwise operation proceeds as normal.

*Remote mount servers* - If the application cannot be installed on the mount server it will still function however with additional manual steps.  If the mount server is remote, the remote server volume that mounts the backups will need to be manually mapped within the file browse dialog exposed in the comparison window.  Once the remote drive is mounted the \VeeamFLR folder can be navigated to select volumes or folders for comparison.

**Troubleshooting:**
The most likely cause of failure during the process is the selection of invalid mount credentials. Refer to Veeam's Data Integration API documentation for additional details - [Veeam Helpcenter](https://helpcenter.veeam.com/docs/backup/powershell/publish-vbrbackupcontent.html?ver=110)

## ✍ Contributions

We welcome contributions from the community! We encourage you to create [issues](https://github.com/VeeamHub/{repo-name}/issues/new/choose) for Bugs & Feature Requests and submit Pull Requests. For more detailed information, refer to our [Contributing Guide](CONTRIBUTING.md).

## 🤝🏾 License

* [MIT License](LICENSE)

## 🤔 Questions

If you have any questions or something is unclear, please don't hesitate to [create an issue](https://github.com/VeeamHub/{repo-name}/issues/new/choose) and let us know!
