using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace VoiceControl
{
    public class FileProvider : BaseProvider<ICommandController>
    {
        private readonly IInterfaceFinder interfaceFinder;

        public FileProvider(IPaths pathManager, IInterfaceFinder interfaceFinder)
        {
            foreach (var file in Directory.GetFiles(pathManager.GetPath("Words")))
            {
                available.Add(Assembly.GetAssembly(typeof(FileProvider)).GetName().Name + ".File." + Path.GetFileNameWithoutExtension(file), () => new FileController(interfaceFinder,file));
            }

            this.interfaceFinder = interfaceFinder;
        }
    }
}
