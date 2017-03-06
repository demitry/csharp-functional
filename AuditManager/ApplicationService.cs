using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AuditManager
{
    class ApplicationService
    {
        public readonly string _directoryName;
        public readonly AuditManager _auditManager;
        public readonly Persister _persister;

        public ApplicationService(string derectoryName)
        {
            _directoryName = derectoryName;
        }

        public void RemoveMentionsAbout(string visitirName)
        {
            FileContent [] files = _persister.ReadDirectory(_directoryName);
            IReadOnlyList<FileAction> actions = _auditManager.RemoveMentionsAbout(visitirName, files);
            _persister.ApplyChanges(actions);
        }

        public void AddRecord(string visitorName, DateTime timeOfVisit)
        {
            FileInfo fileInfo = new DirectoryInfo(_directoryName)
                .GetFiles()
                .OrderByDescending(x => x.LastWriteTime)
                .First();

            FileContent file = _persister.ReadFile(fileInfo.Name);
            FileAction action = _auditManager.AddRecord(file, visitorName, timeOfVisit);
            _persister.ApplyChanges(new [] {action});
        }
    }
}

