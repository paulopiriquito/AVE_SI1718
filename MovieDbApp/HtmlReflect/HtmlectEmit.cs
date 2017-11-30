using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HtmlReflect
{
    public interface IHtmlGetter
    {
        string ToHtml(object target);
        string ToHtml(object[] targets);
    }

    public class HtmlectEmit
    {
        private static Dictionary<Type, IHtmlGetter> gettersDictionary = new Dictionary<Type, IHtmlGetter>();
        
        private static IHtmlGetter EmitGetter(Type klassType)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicHtmlGetter");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType("HtmlGetter", TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IHtmlGetter));

            FieldBuilder fieldBuilder = typeBuilder.DefineField("html", typeof(string), FieldAttributes.Private);

            MethodBuilder ToHtmlObj = typeBuilder.DefineMethod("ToHtml", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] { typeof(object)});
            MethodBuilder ToHtmlArray = typeBuilder.DefineMethod("ToHtml", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] { typeof(object[]) });

            ILGenerator ToHtmlObjIL = ToHtmlObj.GetILGenerator();
            ILGenerator ToHtmlArrayIL = ToHtmlArray.GetILGenerator();

            //TODO IL Emit

            Type t = typeBuilder.CreateType();
            object getter = Activator.CreateInstance(t);
            return (IHtmlGetter) getter;
        }

        public string ToHtml(object target)
        {
            Type objType = target.GetType();
            IHtmlGetter getter;
            if (!gettersDictionary.TryGetValue(objType, out getter))
            {
                getter = EmitGetter(objType);
                if (getter != null)
                {
                    gettersDictionary.Add(objType,getter);
                    return getter.ToHtml(target);
                }
                else throw new NullReferenceException($"Failed to create IHtmlGetter of: {objType.FullName}");
            }
            return getter.ToHtml(target);
        }

        public string ToHtml(object[] targets)
        {
            Type objType = targets[0].GetType();
            IHtmlGetter getter;
            if (!gettersDictionary.TryGetValue(objType, out getter))
            {
                getter = EmitGetter(objType);
                if (getter != null)
                {
                    gettersDictionary.Add(objType, getter);
                    return getter.ToHtml(targets);
                }
                else throw new NullReferenceException($"Failed to create IHtmlGetter of: {objType.FullName}");
            }
            return getter.ToHtml(targets);
        }
    }
}
