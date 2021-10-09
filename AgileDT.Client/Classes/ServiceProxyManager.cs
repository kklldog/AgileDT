using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AgileDT.Client.Classes
{
    public class ServiceProxyManager
    {
        public static ServiceProxyManager Instance { get; }

        private ClassProxyCreator _classProxyCreator = new ClassProxyCreator("agile_dt_proxy_lib");
        private List<Type> _sourceTypes = new List<Type>();
        private List<Type> _sourceInterfaces = new List<Type>();

        private List<Type> _newTypes = new List<Type>();

        private Dictionary<Type, Type> _interfaceProxiesMap = new Dictionary<Type, Type>();

        private ServiceProxyManager() { }

        static ServiceProxyManager()
        {
            Instance = new ServiceProxyManager();
        }

        /// <summary>
        /// 扫描所有程序集，找到实现IEventService的类，并为这些类生成代理类
        /// </summary>
        public Assembly ScanAndBuild()
        {
            _sourceTypes.Clear();
            _sourceInterfaces.Clear();
            _newTypes.Clear();
            _interfaceProxiesMap.Clear();

            _sourceTypes = Helper.ScanAll();

            var ass = _classProxyCreator.CreateProxyAssembly(_sourceTypes);

            _newTypes = ass.GetTypes().ToList();

            var ieventT = typeof(IEventService);
            foreach (var type in _newTypes)
            {
                var ites = type.GetInterfaces();
                var it = ites.FirstOrDefault(x => ieventT.IsAssignableFrom(x));
                if (it == null)
                {
                    continue;
                }

                _sourceInterfaces.Add(it);
                _interfaceProxiesMap.Add(it,type);
            }

            return ass;
        }

        /// <summary>
        /// 获取原始服务类
        /// </summary>
        /// <returns></returns>
        public List<Type> GetSourceTypes()
        {
            return _sourceTypes;
        }

        /// <summary>
        /// 获取新的代理类
        /// </summary>
        /// <returns></returns>
        public List<Type> GetProxies()
        {
            return _newTypes;
        }

        /// <summary>
        /// 获取原始类的接口集合
        /// </summary>
        /// <returns></returns>
        public List<Type> GetSourceInterfaces()
        {
            return _sourceInterfaces;
        }

        /// <summary>
        /// 获取接口与新的代理类的对应关系
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type,Type> GetInterfaceProxyMap()
        {
            return _interfaceProxiesMap;
        }
    }
}
