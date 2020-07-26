﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VoiceControl
{

    class FileContoler : INamedCommandController,IDoesChange
    {
        private readonly IInterfaceFinder interfaceFinder;
        private readonly string path;

        public event Action<object> Changed;

        public FileContoler(IInterfaceFinder interfaceFinder,string path)
        {
            this.interfaceFinder = interfaceFinder;
            this.path = path;
            ObservePath(path);
        }

        public void ObservePath(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path =Path.GetDirectoryName( path);
            watcher.Filter = Path.GetFileName(path);
            watcher.Changed += (s,e)=> Changed?.Invoke(this); 
            watcher.EnableRaisingEvents = true;
        }
     

        public string Name => Path.GetFileNameWithoutExtension(path);
         
        public interface ISetting
        {
            void Apply(ISettingsBuilder builder);
        }
        public interface ISettingsBuilder
        {
            IControllerSettings Settings { get; }

            Dictionary<string ,IOrder> Orders { get; }

            string Value { get; }
        }
        public class SettingBuilder : ISettingsBuilder
        {
            public IControllerSettings Settings { get; set; }

            public Dictionary<string, IOrder> Orders { get; set; }

            public string Value { get; set; }
        }

        public class RepetitionsSetting : ISetting
        {
            public void Apply(ISettingsBuilder builder)
            {
                builder.Settings.RepetitionCount = int.Parse(builder.Value);
            }
        }
        public class AfterSetting : ISetting
        {
            public void Apply(ISettingsBuilder builder)
            {
                builder.Orders[nameof(KeyOrder)] = new AfterOrder(builder.Orders[nameof(KeyOrder)], ()=>SendKeys.SendWait(builder.Value));
            }
        }

        public class BeforeSetting : ISetting
        {
            public void Apply(ISettingsBuilder builder)
            {
                builder.Orders[nameof(KeyOrder)] = new BeforeOrder(builder.Orders[nameof(KeyOrder)], () => SendKeys.SendWait(builder.Value));
            }
        }

        public interface IOrder
        {
            Action<string> Action { get; set; }
        }

        public class KeyOrder:IOrder
        {
            public Action<string> Action { get; set; } = x => SendKeys.SendWait(x);
        }

        public class AfterOrder : IOrder
        {
            public AfterOrder(IOrder previous,Action after)
            {
                Action = x =>
                {
                    previous.Action(x);
                            after();
                };
            }
            public Action<string> Action { get; set; }
        }
        public class BeforeOrder : IOrder
        {
            public BeforeOrder(IOrder previous, Action before)
            {
                Action = x =>
                {
                    before();
                    previous.Action(x);
                };
            }
            public Action<string> Action { get; set; }
        }

        public void Build(IBuilder builder)
        {
            Categorizer categorizer = new Categorizer(new FileMappingCreator().CreateMapping(path));
            Dictionary<string, IOrder> orders=interfaceFinder.InstantiateAllTypes<IOrder>().ToDictionary(x => x.GetType().Name);
            Dictionary<string, ISetting> settings=interfaceFinder.InstantiateAllTypes<ISetting>().ToDictionary(x => x.GetType().Name.ToLower());
                
            foreach (var item in categorizer.Settings)
            {
                if (settings.TryGetValue(item.Key+"setting",out var setting))
                {
                    setting.Apply(new SettingBuilder() { Orders = orders, Settings = builder.Settings, Value = item.Value });
                }
                else
                {
                    Console.WriteLine($"Could not find setting: {item.Key}");
                }
                
            }
            foreach (var item in categorizer.Words)
            {
                builder.Commands.AddCommand(item.Key, (i,s) =>orders[nameof(KeyOrder)].Action(IntegrateParameters( item.Value,i,s )));
            }
        }
        public string IntegrateParameters(string trigger, List<int>numbers, List<string> text)
        {
            string localResult = trigger;
            for (int i = 0; i < numbers.Count; i++)
            {
                localResult = localResult.Replace("$i" + i, numbers[i].ToString());
            }
            for (int i = 0; i < text.Count; i++)
            {
                localResult = localResult.Replace("$s" + i, text[i].ToString());
            }
            return localResult;
        }
    }
}