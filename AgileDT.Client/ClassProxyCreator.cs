using Natasha.CSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AgileDT.Client
{
    public class ClassProxyCreator
    {
        static AssemblyCSharpBuilder sharpBuilder;
        static ClassProxyCreator()
        {
            NatashaInitializer.Initialize();
            //使用 Natasha 的 CSharp 编译器直接编译字符串
            sharpBuilder = new AssemblyCSharpBuilder();

            //给编译器指定一个随机域
            sharpBuilder.Compiler.Domain = DomainManagement.Random;

            //使用文件编译模式，动态的程序集将编译进入DLL文件中，当然了你也可以使用内存流模式。
            sharpBuilder.UseFileCompile();

            //如果代码编译错误，那么抛出并且记录日志。
            sharpBuilder.ThrowAndLogCompilerError();
            //如果语法检测时出错，那么抛出并记录日志，该步骤在编译之前。
            sharpBuilder.ThrowAndLogSyntaxError();
      
        }

        string CreateStringClass(Type source)
        {
            var ns = source.Namespace;
            var sourceClassName = source.Name;
            var newClassName = sourceClassName + "_Proxy";
            var bizMethod = Helper.GetBizMethod(source);
            var usingStr0 = CreateUsing(source.GetConstructors());
            var usingStr1 = CreateUsing(bizMethod);
            var ctrs = CreateCtrs(source, newClassName);

            var classStr = ClassTemplate.ClassTemp
                .Replace("@using", usingStr0 + usingStr1)
                .Replace("@ctrs", ctrs)
                .Replace("@ns", ns)
                .Replace("@sourceClassNAme", sourceClassName)
                .Replace("@newClassName", newClassName)
                .Replace("@bizMethodName", bizMethod.Name);

            return classStr;
        }

        string CreateCtrs(Type source, string newClassName)
        {
            var ctrs = source.GetConstructors();
            var ctrsSb = new StringBuilder();
            foreach (var item in ctrs)
            {
                var paramS = item.GetParameters();
                var str = $"public {newClassName} ( @params ) @baseCtr " +
                    $"{{}}";

                var paramsSb = new StringBuilder();
                var baseCallParams = new StringBuilder(":base(");
                foreach (var param in paramS)
                {
                    var type = "";
                    if (!param.ParameterType.IsGenericType)
                    {
                        type = param.ParameterType.Name;
                    }
                    else{
                        var idx = param.ParameterType.Name.IndexOf("`");
                        var paramType = param.ParameterType.Name.Substring(0, idx);
                        var g_args = param.ParameterType.GenericTypeArguments;
                        type = "<";
                        foreach (var arg in g_args)
                        {
                            type += arg.Name + ",";
                        }
                        type = type.TrimEnd(',');
                        type += ">";
                        type = paramType + type;
                    }
                    var name = param.Name;
                    paramsSb.AppendFormat("{0} {1},", type, name);
                    baseCallParams.AppendFormat("{0},", name);
                }
                var paramsStr = paramsSb.ToString().TrimEnd(',');
                var baseCallParamStr = baseCallParams.ToString().TrimEnd(',') + ")";
                str = str.Replace("@params", paramsStr)
                        .Replace("@baseCtr", baseCallParamStr);

                ctrsSb.AppendLine(str);
            }

            return ctrsSb.ToString();
        }

        string CreateUsing(MethodInfo method)
        {
            var callParams = method.GetParameters();
            var sb = new StringBuilder();
            foreach (var item in callParams)
            {
                var type = item.ParameterType;
                var ns = type.Namespace;

                sb.AppendLine($"using {ns};");
            }

            return sb.ToString();
        }

        string CreateUsing(ConstructorInfo[] ctrs)
        {
            var sb = new StringBuilder();
            foreach (var item in ctrs)
            {
                var paramS = item.GetParameters();
                foreach (var param in paramS)
                {
                    var ns = param.ParameterType.Namespace;
                    sb.AppendLine($"using {ns};");
                    if (param.ParameterType.IsGenericType)
                    {
                        var gTypes = param.ParameterType.GetGenericArguments();
                        foreach (var gType in gTypes)
                        {
                            sb.AppendLine($"using {gType.Namespace};");
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public Assembly CreateProxyAssembly()
        {
            var types = Helper.ScanAll();

            return CreateProxyAssembly(types);
        }

        public Assembly CreateProxyAssembly(List<Type> sources)
        {
            sources.ForEach(x => { 
                var classStr = CreateStringClass(x);
                sharpBuilder.Add(classStr);
            });
            var assembly = sharpBuilder.GetAssembly();
            return assembly;
        }
    }
}
