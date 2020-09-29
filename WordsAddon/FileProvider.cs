using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace VoiceControl
{
    public class FileProvider : BaseProvider<ICommandControllerDefinition>
    {
        private readonly IInterfaceFinder interfaceFinder;

        public FileProvider(IPathManager pathManager, IInterfaceFinder interfaceFinder)
        {
            foreach (var file in Directory.GetFiles(pathManager.GetPath("Words")))
            {
                available.Add(Assembly.GetAssembly(typeof(FileProvider)).GetName().Name + ".File." + Path.GetFileNameWithoutExtension(file), () => new FileContoler(interfaceFinder,file));
            }

            this.interfaceFinder = interfaceFinder;
        }
    }
}
