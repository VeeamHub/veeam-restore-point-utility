using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//for Powershell add system.management.automation.dll reference from \windows\assembly\gac_msil\s...
using System.Management.Automation;
using System.Management.Automation.Runspaces;


namespace VeeamFileDiff
{
    public class VeeamAppData
    {
        private Runspace myRunSpace;
        private RunspacePool rsp;
        public Runspace VBRRunSpace
        {
            get { return myRunSpace; }
            set { myRunSpace = value; }
        }

        public RunspacePool VBRRunSpacePool
        {
            get { return rsp; }
            set { rsp = value; }
        }

        private String workloadName;
        public String machineName
        {
            get { return workloadName; }
            set { workloadName = value; }
        }

        private String mntAID,mntBID;
        public String restoPtIDA
        {
            get { return mntAID; }
            set { mntAID = value; }
        }
        public String restoPtIDB
        {
            get { return mntBID; }
            set { mntBID = value; }
        }

        public class creds
        {
            public String userName;
            public String userDescription;

            public creds(String user, String desc)
            {
                userName = user;
                userDescription = desc;
            }
        }

        public creds mntCreds;
        public VeeamAppData() { }
    }
}
