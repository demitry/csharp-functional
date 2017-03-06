using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AuditManager
{
    public class Persister
    {
        public FileContent ReadFile(string fileName)
        {
            return  new FileContent(fileName, File.ReadAllLines(fileName));
        }

        public FileContent[] ReadDirectory(string directoryName)
        {
            return Directory.GetFiles(directoryName).Select(ReadFile).ToArray();
        }

        public void ApplyChanges(IReadOnlyList<FileAction> actions)
        {
            foreach (var action in actions)
            {
                switch (action.Type)
                {
                    case ActionType.Delete:
                    case ActionType.Create:
                    {
                        File.WriteAllLines(action.FileName, action.Content);
                            continue;
                    }    
                    case ActionType.Update:
                    {
                        File.Delete(action.FileName);
                            continue;
                    }
                    default:
                        throw new InvalidOperationException();
                }
                
            }
        }
    }
}