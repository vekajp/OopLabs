using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Backups.BackupAbstractModel;
using BackupsExtra.ExtraModel;

namespace BackupsExtra.ApplicationState
{
    public class XmlSaver : IBackupJobSaver
    {
        private string _path;
        public XmlSaver()
        {
            _path = "temp.xml";
        }

        public void Save(ApplicationContext context)
        {
            using var fs = new FileStream(_path, FileMode.Truncate);
            var serializer = new XmlSerializer(typeof(List<BackupSerializationDto>));
            serializer.Serialize(fs, context.BackupJobs.Select(x => new BackupSerializationDto(x)).ToList());
        }

        public ApplicationContext Load()
        {
            using var fs = new FileStream(_path, FileMode.Open);
            var serializer = new XmlSerializer(typeof(List<BackupSerializationDto>));
            var context = ApplicationContext.Instance();
            var dto = (List<BackupSerializationDto>)serializer.Deserialize(fs);
            context.BackupJobs = dto?.Select(x => x.GetJob()).ToList();
            return context;
        }
    }
}