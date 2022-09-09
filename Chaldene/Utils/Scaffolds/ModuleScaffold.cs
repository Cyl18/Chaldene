﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chaldene.Data.Messages;
using Chaldene.Modules;

namespace Chaldene.Utils.Scaffolds;

/// <summary>
/// 模块化拓展方法
/// </summary>
public static class ModuleScaffold
{
    /// <summary>
    /// 获取泛型参数同一个命名空间下的所有模块
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<IModule> GetModules<T>(this T module) where T : IModule
    {
        var basic = typeof(T);
        
        var types = Assembly.GetAssembly(basic).GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface)
            .Where(x => x.GetInterfaces().Any(x => x == typeof(IModule)))
            .Where(x => x!.FullName!.Contains(basic.Namespace!))
            .ToList();

        foreach (var type in types)
            yield return Activator.CreateInstance(type) as IModule;
    }

    /// <summary>
    /// 传播订阅到模块
    /// </summary>
    /// <param name="modules"></param>
    /// <param name="base"></param>
    public static void Raise(this IEnumerable<IModule> modules, MessageReceiverBase @base)
    {
        foreach (var module in modules)
        {
            if (module.IsEnable is not false)
            {
                module.Execute(@base);
            }
        }
    }
}