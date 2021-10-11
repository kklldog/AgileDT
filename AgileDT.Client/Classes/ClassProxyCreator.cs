using Natasha.CSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace AgileDT.Client.Classes
{
    public class ClassProxyCreator
    {
        AssemblyCSharpBuilder sharpBuilder;
        public ClassProxyCreator(string libName)
        {
            NatashaInitializer.Initialize();
            //使用 Natasha 的 CSharp 编译器直接编译字符串
            sharpBuilder = new AssemblyCSharpBuilder(libName);

            //给编译器指定一个随机域
            sharpBuilder.Compiler.Domain = DomainManagement.Random;

            //使用文件编译模式，动态的程序集将编译进入DLL文件中，当然了你也可以使用内存流模式。
            sharpBuilder.UseFileCompile();

            //如果代码编译错误，那么抛出并且记录日志。
            sharpBuilder.ThrowAndLogCompilerError();
            //如果语法检测时出错，那么抛出并记录日志，该步骤在编译之前。
            sharpBuilder.ThrowAndLogSyntaxError();

        }

        public string CreateStringClass(Type source)
        {
            var bizMethod = Helper.GetBizMethod(source);
            if (!bizMethod.IsVirtual)
            {
                throw new Exception("business method is not a virtual method .");
            }
            var ns = source.Namespace;
            var sourceClassName = source.Name;
            var newClassName = sourceClassName + "_agiledt_proxy";
            var returnType = bizMethod.ReturnType;
            var usingStr0 = CreateUsing(source.GetConstructors());
            var usingStr1 = CreateUsing(bizMethod);
            var usingStr2 = new StringBuilder();
            CreateUsing(returnType , usingStr2);
            var ctrs = CreateCtrs(source, newClassName);
            var method = MethodCodeStype(bizMethod);
            var rtType = CreateReutrunType(returnType);

            var classStr = ClassTemplate.ClassTemp
                .Replace("@using", usingStr0 + usingStr1 + usingStr2)
                .Replace("@ctrs", ctrs)
                .Replace("@ns", ns)
                .Replace("@returnType", rtType)
                .Replace("@methodName", method)
                .Replace("@sourceClassName", sourceClassName)
                .Replace("@newClassName", newClassName)
                .Replace("@bizMethodName", bizMethod.Name)
                .Replace("@bizMethodCallParams", string.Join(',', bizMethod.GetParameters().Select(x => x.Name).ToArray()));

            if (rtType == "void")
            {
                classStr = classStr.Replace("void ret;", "").Replace("ret =", "").Replace("return ret;", "");
            }

            return classStr;
        }

        string  CreateReutrunType(Type returnType)
        {
            var name = TypeCodeStyle(returnType);

            if (name == "Void")
            {
                name = "void";
            }

            return name;
        }

        string TypeCodeStyle(Type type)
        {
            var typeSb = new StringBuilder();
            var str = "";
            if (type.IsGenericType)
            {
                var idx = type.Name.IndexOf("`");
                var paramType = type.Name.Substring(0, idx);
                var g_args = type.GenericTypeArguments;
                typeSb.Append("<");
                foreach (var arg in g_args)
                {
                    var name = TypeCodeStyle(arg);
                    typeSb.Append(name + ",");
                }
                str = typeSb.ToString().TrimEnd(',');
                str += ">";
                str = paramType + str;
            }
            else
            {
                str = type.Name;
            }

            return str;
        }

        /// <summary>
        /// 把一个方法调用的入参换成代码模式 比如：string name , List<string> names
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string ParamCodeStyle(ParameterInfo param)
        {
            var type = "";
            var name = "";
            name = param.Name;
            type = TypeCodeStyle(param.ParameterType);

            return $"{type} {name},";
        }

        string MethodCodeStype(MethodInfo method)
        {
            var name = method.Name;
            var str = "@name(@params)";

            var paramsCode = new StringBuilder();

            foreach (var param in method.GetParameters())
            {
                paramsCode.Append(ParamCodeStyle(param));
            }

            var paramstr = paramsCode.ToString();
            paramstr = paramstr.TrimEnd(',');

            return str
                .Replace("@name", name)
                .Replace("@params", paramstr);
        }

        string CreateCtrs(Type source, string newClassName)
        {
            var ctrs = source.GetConstructors();
            var ctrsSb = new StringBuilder();
            foreach (var item in ctrs)
            {
                var methodParams = item.GetParameters();
                var str = $"public {newClassName} ( @params ) @baseCtr " +
                    $"{{}}";

                var ctrCodeSb = new StringBuilder();
                var baseCtrCallParams = new StringBuilder(":base(");
                foreach (var param in methodParams)
                {
                    ctrCodeSb.AppendFormat(ParamCodeStyle(param));
                    baseCtrCallParams.AppendFormat("{0},", param.Name);
                }
                var ctrCodeStr = ctrCodeSb.ToString().TrimEnd(',');
                var baseCtrCallParamsStr = baseCtrCallParams.ToString().TrimEnd(',') + ")";
                str = str.Replace("@params", ctrCodeStr)
                        .Replace("@baseCtr", baseCtrCallParamsStr);

                ctrsSb.AppendLine(str);
            }

            return ctrsSb.ToString();
        }

        StringBuilder CreateUsing(Type type, StringBuilder sb)
        {
            var ns = type.Namespace;
            sb.AppendLine($"using {ns};");
            if (!type.IsGenericType)
            {
                // to do 
            }
            else
            {
                foreach (var item in type.GetGenericArguments())
                {
                    CreateUsing(item, sb);
                }
            }

            return sb;
        }

        string CreateUsing(MethodInfo method)
        {
            var sb = new StringBuilder();
            var callParams = method.GetParameters();
            foreach (var item in callParams)
            {
                var type = item.ParameterType;
                CreateUsing(type, sb);
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
                    CreateUsing(param.ParameterType, sb);
                }
            }

            return sb.ToString();
        }

        public Assembly CreateProxyAssembly(List<Type> sources)
        {
            sources.ForEach(x =>
            {
                var classStr = CreateStringClass(x);
                sharpBuilder.Add(classStr);
            });
            var assembly = sharpBuilder.GetAssembly();
            return assembly;
        }
    }
}
